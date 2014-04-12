using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Win32.Synchronization;

namespace SpikeIRacing
{

	public unsafe class iRacingDataFeed : IDisposable
	{
		public iRacingDataFeed()
		{
		}

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
			} finally
			{
				accessor.SafeMemoryMappedViewHandle.ReleasePointer();
			}
		}
	
		byte* ptrHeader;
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
