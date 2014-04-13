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
