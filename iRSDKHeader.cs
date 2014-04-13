using System;
using System.Runtime.InteropServices;

namespace iRacingSDK
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
	
}
