using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace SpikeIRacing
{

	public struct VarBufWithIndex
	{
		public int tickCount;
		public int bufOffset;
		public int index;
	}
	
}
