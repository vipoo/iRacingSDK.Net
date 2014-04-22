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
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace iRacingSDK
{
	public class Replay
    {
		int messageId;

		public Replay()
		{
			messageId = Win32.Messages.RegisterWindowMessage("IRSDK_BROADCASTMSG");
		}


        public void SetSpeed(double p)
        {
            SendMessage(BroadcastMessage.ReplaySetPlaySpeed, (short)p, 0);
		}

        public void MoveToStart()
        {
			ReplaySearch(ReplaySearchMode.ToStart);

            WaitAndVerify(data => data.Telemetry.ReplayFrameNum != 0);
        }

        void WaitAndVerify(Func<DataSample, bool> verifyFn)
        {
            WaitAndVerify(verifyFn, () => { });
        }

        void WaitAndVerify(Func<DataSample, bool> verifyFn, Action action)
        {
            var timeout = DateTime.Now + TimeSpan.FromMilliseconds(1000);
            var data = iRacing.GetDataFeed().First();
            while (verifyFn(data) && DateTime.Now < timeout)
            {
                action();
                data = iRacing.GetDataFeed().First();
                Thread.Sleep(1);
            }
            System.Diagnostics.Debug.Assert(!verifyFn(data));

        }
        public void MoveToNextSession()
        {
            var data = iRacing.GetDataFeed().First();
            if (data.Telemetry.SessionNum == data.SessionData.SessionInfo.Sessions.Length)
                return;

			ReplaySearch(ReplaySearchMode.NextSession);

            WaitAndVerify(data2 => data.Telemetry.SessionNum + 1 != data2.Telemetry.SessionNum);
        }

        public void MoveToStartOfRace()
        {
            MoveToStart();
            var data = iRacing.GetDataFeed().First();

            var session = data.SessionData.SessionInfo.Sessions.FirstOrDefault(s => s.SessionType == "Race");
            if( session == null )
                throw new Exception("No race session found in this replay");
            
            WaitAndVerify(d => d.Telemetry.SessionNum != session.SessionNum, () => MoveToNextSession());
        }

        public void MoveToParadeLap()
        {
            MoveToStartOfRace();

            foreach( var data in iRacing.GetDataFeed())
            {
                if (data.Telemetry.SessionState == SessionState.Racing)
                    break;

                this.SetSpeed(16);
            }

            this.SetSpeed(0);
        }

        /// <summary>
        /// Select the camera onto a car and position to 4 seconds before the incident
        /// </summary>
        public void MoveToNextIncident()
        {
            ReplaySearch(ReplaySearchMode.NextIncident);
        }

		public void MoveToNextLap()
		{
			ReplaySearch(ReplaySearchMode.NextLap);
		}

        public void MoveToNextFrame()
        {
            ReplaySearch(ReplaySearchMode.NextFrame);
        }

        public void CameraOnDriver(short carNumber, short group, short camera = 0)
        {
            SendMessage(BroadcastMessage.CameraSwitchNum, carNumber, group, camera);
        }

        public void CameraOnPositon(short carPosition, short group, short camera = 0)
        {
            SendMessage(BroadcastMessage.CameraSwitchNum, carPosition, group, camera);
        }

		void ReplaySearch(ReplaySearchMode mode)
		{
			SendMessage(BroadcastMessage.ReplaySearch, (short)mode);
		}

        DateTime lastMessagePostedTime = DateTime.Now;

		void SendMessage(BroadcastMessage message, short var1 = 0, short var2 = 0, short var3 = 0)
		{
            var var23 = FromShorts(var2, var3);
            var msgVar1 = FromShorts((short)message, var1);

            if (!Win32.Messages.SendNotifyMessage(Win32.Messages.HWND_BROADCAST, messageId, msgVar1, var23))
                throw new Exception(String.Format("Error in broadcasting message {0}", message));
		}

        static int FromShorts(short lowPart, short highPart)
        {
            return ((int)highPart << 16) | (ushort)lowPart;
        }
    }
}
