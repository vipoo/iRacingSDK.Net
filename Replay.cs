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

namespace iRacingSDK
{
	public class Replay
    {
		int messageId;

		public Replay()
		{
			messageId = Win32.Messages.RegisterWindowMessage("IRSDK_BROADCASTMSG");
		}

		public static int FromShorts(short lowPart, short highPart)
		{
			return ((int)highPart << 16) | (ushort)lowPart;
		}

        public void MoveToStart()
        {
			ReplaySearch(ReplaySearchMode.ToStart);
        }

        public void MoveToNextSession()
        {
			ReplaySearch(ReplaySearchMode.NextSession);
        }

		private void ReplaySearch(ReplaySearchMode mode)
		{
			SendMessage(BroadcastMessage.ReplaySearch, (short)mode);
		}

		private void SendMessage(BroadcastMessage message, short var1 = 0, short var2 = 0, short var3 = 0)
		{
			var var23 = FromShorts(var2, var3);
			var msgVar1 = FromShorts((short)message, var1);

			if(!Win32.Messages.SendNotifyMessage(Win32.Messages.HWND_BROADCAST, messageId, msgVar1, var23))
				throw new Exception(String.Format("Error in broadcasting message {0}", message));

		}
    }
}
