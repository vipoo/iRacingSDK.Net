using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Win32.Synchronization;

namespace SpikeIRacing
{

	public static class iRSDKHeaderExtensions
	{
		public static bool HasChangedSinceReading(this iRSDKHeader header, VarBufWithIndex buf)
		{
			return header.varBuf[buf.index].tickCount != buf.tickCount;
		}

		public static VarBufWithIndex FindLatestBuf(this iRSDKHeader header)
		{
			return header.varBuf
				.Take(header.numBuf)
					.Select((b, i) => new VarBufWithIndex() { tickCount = b.tickCount, bufOffset =  b.bufOffset, index = i })
					.OrderByDescending(b => b.tickCount)
				.FirstOrDefault();
		}
	}
}
