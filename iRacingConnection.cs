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
using System.Collections.Generic;
using System.Threading;
using Win32.Synchronization;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;

namespace iRacingSDK
{
	static class iRacingConnection
	{
		public static MemoryMappedViewAccessor Accessor {get; private set; }
		static IntPtr dataValidEvent;
		static MemoryMappedFile irsdkMappedMemory;

		public static bool IsConnected()
		{
			if(Accessor != null)
				return true;

			var dataValidEvent =  Event.OpenEvent(Event.EVENT_ALL_ACCESS | Event.EVENT_MODIFY_STATE, false, "Local\\IRSDKDataValidEvent");
			if(dataValidEvent == IntPtr.Zero)
				return false;

            MemoryMappedFile irsdkMappedMemory = null;
            try
            {
                irsdkMappedMemory = MemoryMappedFile.OpenExisting("Local\\IRSDKMemMapFileName");
            }
            catch(Exception)
            { }

			if(irsdkMappedMemory == null)
				return false;

			var accessor = irsdkMappedMemory.CreateViewAccessor();
			if(accessor == null)
			{
				irsdkMappedMemory.Dispose();
				return false;
			}

			iRacingConnection.irsdkMappedMemory = irsdkMappedMemory;
			iRacingConnection.dataValidEvent = dataValidEvent;
			Accessor = accessor;
			return true;
		}

		public static bool WaitForData()
		{
            return Event.WaitForSingleObject(dataValidEvent, 100) == 0;
		}
    }
}
