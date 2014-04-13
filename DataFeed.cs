// This file is part of iRacingSDK.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingSDK.Net
//
// iRacingSDK is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingSDK is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingSDK.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Win32.Synchronization;
using iRacingSDK;

namespace iRacingSDK
{
	public unsafe class DataFeed : IDisposable
	{
		public bool Connect()
		{
			try
			{
			OpenDataValidEvent();
			OpenMemoryMappedFile();
			} catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
			return true;
		}

		public void OnSessionInfo(Action<string> newSessionDataFn)
		{

		}

		public IEnumerable<Dictionary<string,object>> DataFeed
		{
			get
			{
				while(true)
				{
					var values = GetNextDataSample();
					if(values != null )
						yield return values;
				}
			}
		}

		public void Dispose()
		{
			irsdkMappedMemory.Dispose();
			accessor.Dispose();
		}

		IntPtr dataValidEvent;
		MemoryMappedFile irsdkMappedMemory;
		MemoryMappedViewAccessor accessor;
		iRSDKHeader header;
		VarHeader[] varHeaders;
		Dictionary<string, object> values;
		public string SessionInfo { get; private set; }

		void OpenDataValidEvent()
		{
			dataValidEvent = Event.OpenEvent(Event.EVENT_ALL_ACCESS | Event.EVENT_MODIFY_STATE, false, "Local\\IRSDKDataValidEvent");
			if(dataValidEvent != IntPtr.Zero)
				return;

			int le = Marshal.GetLastWin32Error();
			throw new ApplicationException(String.Format("Unable to connect to event signals of iRacing"));
		}

		void OpenMemoryMappedFile()
		{
			irsdkMappedMemory = MemoryMappedFile.OpenExisting("Local\\IRSDKMemMapFileName");
			accessor = irsdkMappedMemory.CreateViewAccessor();
		}

		unsafe Dictionary<string, object> GetNextDataSample()
		{
			var r = Event.WaitForSingleObject(dataValidEvent, 100);

			byte* ptr = null;

			accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
			try
			{
				ReadHeader(ref ptr);
				ReadSessionInfo(ref ptr);
				return ReadVariables();
			}
			finally
			{
				accessor.SafeMemoryMappedViewHandle.ReleasePointer();
			}
		}
	
		byte* ptrHeader = null;
		void RereadHeader()
		{
			byte* ptr = ptrHeader;
			ReadHeader(ref ptr);
		}

		unsafe void ReadHeader(ref byte *ptr)
		{
			ptrHeader = ptr;
			header = (iRSDKHeader)System.Runtime.InteropServices.Marshal.PtrToStructure(new IntPtr(ptr), typeof(iRSDKHeader));

			var size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(VarHeader));

			varHeaders = new VarHeader[header.numVars];
			ptr += header.varHeaderOffset;

			for(var i = 0; i < header.numVars; i++)
			{
				varHeaders[i] = (VarHeader)Marshal.PtrToStructure(new IntPtr(ptr), typeof(VarHeader));
				ptr += size;
			}
		}

		unsafe void ReadSessionInfo(ref byte* ptr)
		{
			var sessionInfoData = new byte[header.sessionInfoLen];
			accessor.ReadArray<byte>(header.sessionInfoOffset, sessionInfoData, 0, header.sessionInfoLen);
			SessionInfo = System.Text.Encoding.Default.GetString(sessionInfoData).TrimEnd(new char[] { '\0' });
		}

		unsafe Dictionary<string, object> ReadVariables()
		{
			var buf = header.FindLatestBuf();

			values = ReadAllValues(accessor, buf.bufOffset, varHeaders);
			//Thread.Sleep(42);
			RereadHeader();

			if(header.HasChangedSinceReading(buf))
			{
				Console.WriteLine("Data Changed!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				return null;
			}

			values.Add("TickCount", buf.tickCount);
			return values;
		}

		static T[] GetArrayData<T>(MemoryMappedViewAccessor accessor, int size, int offset) where T:struct
		{
			var data = new T[size];
			accessor.ReadArray<T>(offset, data, 0, size);
			return data;
		}

		static Dictionary<string, object> ReadAllValues(MemoryMappedViewAccessor accessor, int buffOffset, VarHeader[] varHeaders)
		{
			var result = new Dictionary<string, object>();

			var maps = new Dictionary<VarType, Func<int, object>>() {
				{ VarType.irInt, (offset) => accessor.ReadInt32(offset) },
				{ VarType.irBitField, (offset) => accessor.ReadInt32(offset) },
				{ VarType.irDouble, (offset) => accessor.ReadDouble(offset) },
				{ VarType.irBool, (offset) => accessor.ReadBoolean(offset) },
				{ VarType.irFloat, (offset) => accessor.ReadSingle(offset) }
			};

			var arryMaps = new Dictionary<VarType, Func<int, int, object>>() {
				{ VarType.irInt, (size, offset) => GetArrayData<int>(accessor, size, offset) },
				{ VarType.irBitField, (size, offset) => GetArrayData<int>(accessor, size, offset) },
				{ VarType.irDouble, (size, offset) => GetArrayData<double>(accessor, size, offset) },
				{ VarType.irFloat, (size, offset) => GetArrayData<float>(accessor, size, offset) },
				{ VarType.irBool, (size, offset) => GetArrayData<bool>(accessor, size, offset) }

			};

			for(var i = 0; i < varHeaders.Length; i++)
			{
				var varHeader = varHeaders[i];
				var offset = buffOffset + varHeader.offset;

				if(varHeader.type == VarType.irChar)
					throw new NotSupportedException();

				object value;
				if(varHeader.count != 1)
					value = arryMaps[varHeader.type](varHeader.count, offset);
				else
					value = maps[varHeader.type](offset);

				result.Add(varHeader.name, value);
			}

			return result;
		}
	}
}
