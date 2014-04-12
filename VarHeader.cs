using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace iRacingData
{

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct VarHeader
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
	
}
