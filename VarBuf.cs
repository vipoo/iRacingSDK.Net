using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace iRacingData
{

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct VarBuf
	{
		public int tickCount;
		public int bufOffset;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public int[] pad;
	}
	
}
