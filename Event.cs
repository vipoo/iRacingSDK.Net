using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Win32.Synchronization;

namespace Win32
{
	namespace Synchronization
	{
		internal static class Event
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
	}
}

