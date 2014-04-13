using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace iRacingSDK
{

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
	
}
