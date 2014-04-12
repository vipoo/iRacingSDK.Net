using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;

namespace SpikeIRacing
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct iRSDKHeader
	{
		//12 bytes: offset = 0
		public int ver;
		public int status;
		public int tickRate;
		//12 bytes: offset = 12
		public int sessionInfoUpdate;
		public int sessionInfoLen;
		public int sessionInfoOffset;
		//8 bytes: offset = 24
		public int numVars;
		public int varHeaderOffset;
		//16 bytes: offset = 32
		public int numBuf;
		public int bufLen;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public int[] pad1;
		//128 bytes: offset = 48
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public VarBuf[] varBuf;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct VarBuf
	{
		public int tickCount;
		public int bufOffset;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public int[] pad;
	}

	public class Defines
	{
		public const int MaxString = 32;
		public const int MaxDesc = 64;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct VarHeader
	{
		//16 bytes: offset = 0
		public VarType type;
		//offset = 4
		public int offset;
		//offset = 8
		public int count;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		public int[] pad;
		//32 bytes: offset = 16
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MaxString)]
		public string name;
		//64 bytes: offset = 48
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MaxDesc)]
		public string desc;
		//32 bytes: offset = 112
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MaxString)]
		public string unit;
	}

	public enum VarType
	{
		irChar,
		irBool,
		irInt,
		irBitField,
		irFloat,
		irDouble}

	;

	[Flags]
	public enum SyncObjectAccess : uint
	{
		DELETE = 0x00010000,
		READ_CONTROL = 0x00020000,
		WRITE_DAC = 0x00040000,
		WRITE_OWNER = 0x00080000,
		SYNCHRONIZE = 0x00100000,
		EVENT_ALL_ACCESS = 0x001F0003,
		EVENT_MODIFY_STATE = 0x00000002,
		MUTEX_ALL_ACCESS = 0x001F0001,
		MUTEX_MODIFY_STATE = 0x00000001,
		SEMAPHORE_ALL_ACCESS = 0x001F0003,
		SEMAPHORE_MODIFY_STATE = 0x00000002,
		TIMER_ALL_ACCESS = 0x001F0003,
		TIMER_MODIFY_STATE = 0x00000002,
		TIMER_QUERY_STATE = 0x00000001
	}

	public static class EventStuff
	{
		public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
		public const uint SYNCHRONIZE = 0x00100000;
		public const uint EVENT_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x3);
		public const uint EVENT_MODIFY_STATE = 0x0002;
		public const long ERROR_FILE_NOT_FOUND = 2L;

		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);

		[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern Int32 WaitForSingleObject(IntPtr Handle, Int32 Wait);
	}

	class MainClass
	{
		public unsafe static void Main(string[] args)
		{
			var iRacing = new iRacingDataFeed();
			if(!iRacing.Connect())
				throw new Exception("Unable to connect to iRacing server");


			iRacing.OnSessionInfo( s => Console.WriteLine("New session data "));
			var sessionInfo = iRacing.SessionInfo;

			Console.WriteLine(sessionInfo);

			foreach(var data in iRacing.DataFeed)
			{
				Console.WriteLine("Tick, Session Time: " + data["TickCount"] + ", " + data["SessionTime"]);
			}
		}
	}

	public class iRacingDataFeed : IDisposable
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
			dataValidEvent = EventStuff.OpenEvent(EventStuff.EVENT_ALL_ACCESS | EventStuff.EVENT_MODIFY_STATE, false, "Local\\IRSDKDataValidEvent");
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
			var r = EventStuff.WaitForSingleObject(dataValidEvent, 100);

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
	
		unsafe void ReadHeader(ref byte *ptr)
		{
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

		Dictionary<string, object> ReadVariables()
		{
			var found = 0;
			for(int x = 0; x < header.numBuf; x++)
			{
				if(header.varBuf[x].tickCount > header.varBuf[found].tickCount)
					found = x;
			}
			var buffOffset = header.varBuf[found].bufOffset;

			var tickCount = header.varBuf[found].tickCount;
			values = ReadAllValues(accessor, buffOffset, varHeaders);
			if(header.varBuf[found].tickCount != tickCount)
			{
				Console.WriteLine("Data Changed!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				return null;
			}

			values.Add("TickCount", tickCount);
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

