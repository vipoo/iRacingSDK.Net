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
using System.Diagnostics;

namespace iRacingSDK
{
	public class Replay
    {
		int messageId;

		public Replay()
		{
			messageId = Win32.Messages.RegisterWindowMessage("IRSDK_BROADCASTMSG");
            currentMessageTask = new Task(() =>{});
            currentMessageTask.Start();
		}

        public void SetSpeed(double p)
        {
            Trace.WriteLine(string.Format("Setting speed to {0}", p), "INFO");
            SendMessage(BroadcastMessage.ReplaySetPlaySpeed, (short)p, 0);
		}

        public void MoveToStart()
        {
            currentMessageTask.Wait();

			ReplaySearch(ReplaySearchMode.ToStart);

            WaitAndVerify(data => data.Telemetry.ReplayFrameNum != 0);
        }

        public void MoveToEnd()
        {
            currentMessageTask.Wait();

            ReplaySearch(ReplaySearchMode.ToEnd);

            currentMessageTask.Wait();
        }

        void WaitAndVerify(Func<DataSample, bool> verifyFn)
        {
            WaitAndVerify(verifyFn, () => { });
        }

        void WaitAndVerify(Func<DataSample, bool> verifyFn, Action action, int wait = 5000)
        {
            if (iRacing.IsRunning)
                return;

            var timeout = DateTime.Now + TimeSpan.FromMilliseconds(wait);
            var data = iRacing.GetDataFeed().First();
            while (verifyFn(data) && DateTime.Now < timeout)
            {
                action();
                data = iRacing.GetDataFeed().First();
                Thread.Sleep(10);
            }
            System.Diagnostics.Debug.Assert(!verifyFn(data));
        }
        public void MoveToNextSession()
        {
            currentMessageTask.Wait();

            var data = iRacing.GetDataFeed().First();
            if (data.Telemetry.SessionNum == data.SessionData.SessionInfo.Sessions.Length)
                return;

			ReplaySearch(ReplaySearchMode.NextSession);

            WaitAndVerify(data2 => data.Telemetry.SessionNum + 1 != data2.Telemetry.SessionNum);
        }


        public void MoveToFrame(int frameNumber, ReplayPositionMode mode = ReplayPositionMode.Begin)
        {
            currentMessageTask.Wait();

            Trace.WriteLine(string.Format("Moving to frame {0}", frameNumber), "INFO");

            SendMessage(BroadcastMessage.ReplaySetPlayPosition, (short)mode, frameNumber);

            currentMessageTask.Wait();

            WaitAndVerify(data => data.Telemetry.ReplayFrameNum != frameNumber, 
                () => SendMessage(BroadcastMessage.ReplaySetPlayPosition, (short)mode, frameNumber),
                2000);

            currentMessageTask.Wait();

            Trace.WriteLine(string.Format("Moved to frame {0}", frameNumber), "INFO");
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

        public void MoveToQualifying()
        {
            MoveToStart();
            var data = iRacing.GetDataFeed().First();

            var session = data.SessionData.SessionInfo.Sessions.FirstOrDefault(s => s.SessionType.ToLower().Contains("qualify"));
            if (session == null)
                throw new Exception("No qualifying session found in this replay");

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
            currentMessageTask.Wait();

            ReplaySearch(ReplaySearchMode.NextIncident);

            currentMessageTask.Wait();
        }

        /// <summary>
        /// Select the camera onto a car and position to a previous incident marker
        /// </summary>
        public void MoveToPrevIncident()
        {
            currentMessageTask.Wait();

            ReplaySearch(ReplaySearchMode.PrevIncident);

            currentMessageTask.Wait();
        }

		public void MoveToNextLap()
		{
            currentMessageTask.Wait();

			ReplaySearch(ReplaySearchMode.NextLap);

            currentMessageTask.Wait();
        }

        public void MoveToNextFrame()
        {
            currentMessageTask.Wait();

            ReplaySearch(ReplaySearchMode.NextFrame);

            currentMessageTask.Wait();
        }

        public void MoveToPrevFrame()
        {
            currentMessageTask.Wait();

            ReplaySearch(ReplaySearchMode.PrevFrame);

            currentMessageTask.Wait();
        }

        public void CameraOnDriver(short carNumber, short group, short camera = 0)
        {
            SendMessage(BroadcastMessage.CameraSwitchNum, carNumber, group, camera);
        }

        public void CameraOnPositon(short carPosition, short group, short camera = 0)
        {
            SendMessage(BroadcastMessage.CameraSwitchPos, carPosition, group, camera);
        }

		void ReplaySearch(ReplaySearchMode mode)
		{
			SendMessage(BroadcastMessage.ReplaySearch, (short)mode);
		}

        DateTime lastMessagePostedTime = DateTime.Now;
        Task currentMessageTask;

        void SendMessage(BroadcastMessage message, short var1 = 0, int var2 = 0)
        {
            var msgVar1 = FromShorts((short)message, var1);

            var lastTask = currentMessageTask;
            currentMessageTask = new Task(() =>
            {
                lastTask.Wait();
                lastTask.Dispose();
                lastTask = null;

                var timeSinceLastMsg = DateTime.Now - lastMessagePostedTime;
                var throttleTime = (int)(500d - timeSinceLastMsg.TotalMilliseconds);
                if (throttleTime > 0)
                {
                    Trace.WriteLine(string.Format("Throttle message {0} delivery to iRacing by {1} millisecond", message, throttleTime), "DEBUG");
                    Thread.Sleep(throttleTime);
                }
                lastMessagePostedTime = DateTime.Now;

                if (!Win32.Messages.SendNotifyMessage(Win32.Messages.HWND_BROADCAST, messageId, msgVar1, var2))
                    throw new Exception(String.Format("Error in broadcasting message {0}", message));
            });

            currentMessageTask.Start();
        }

		void SendMessage(BroadcastMessage message, short var1, short var2, short var3)
		{
            var var23 = FromShorts(var2, var3);
            SendMessage(message, var1, var23);
        }

        static int FromShorts(short lowPart, short highPart)
        {
            return ((int)highPart << 16) | (ushort)lowPart;
        }

        public void Wait()
        {
            currentMessageTask.Wait();
        }
    }
}
