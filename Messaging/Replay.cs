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
    public class Replay : iRacingMessaging
    {
        iRacingConnection iRacingInstance;

        public Replay(iRacingConnection iRacingInstance)
        {
            this.iRacingInstance = iRacingInstance;
        }

        public void SetSpeed(double p)
        {
            Trace.WriteLine(string.Format("Setting speed to {0}", p), "INFO");
            SendMessage(BroadcastMessage.ReplaySetPlaySpeed, (short)p, 0);
        }

        public void MoveToStart()
        {
            Wait();

            ReplaySearch(ReplaySearchMode.ToStart);

            WaitAndVerify(data => data.Telemetry.ReplayFrameNum > 100);
        }

        public void MoveToEnd()
        {
            Wait();

            ReplaySearch(ReplaySearchMode.ToEnd);

            Wait();
        }

        public void MoveToNextSession()
        {
            Wait();

            var data = iRacing.GetDataFeed().First();
            if (data.Telemetry.SessionNum == data.SessionData.SessionInfo.Sessions.Length)
                return;

            ReplaySearch(ReplaySearchMode.NextSession);

            WaitAndVerify(data2 => data.Telemetry.SessionNum + 1 != data2.Telemetry.SessionNum);
        }

        public void MoveToFrame(int frameNumber, ReplayPositionMode mode = ReplayPositionMode.Begin)
        {
            DataSample data = null;

            Wait();

            Trace.WriteLine(string.Format("Moving to frame {0} with mode {1}", frameNumber, mode), "INFO");

            SendMessage(BroadcastMessage.ReplaySetPlayPosition, (short)mode, frameNumber);

            Wait();

            if (mode == ReplayPositionMode.Begin)
                data = WaitAndVerify(d => Math.Abs(d.Telemetry.ReplayFrameNum - frameNumber) > 32, 6000);

            Wait();

            if (data != null)
                frameNumber = data.Telemetry.ReplayFrameNum;

            Trace.WriteLine(string.Format("Moved to frame {0}", frameNumber), "INFO");
        }

        public void MoveToStartOfRace()
        {
            MoveToStart();
            var data = iRacing.GetDataFeed().First();

            var session = data.SessionData.SessionInfo.Sessions.FirstOrDefault(s => s.SessionType == "Race");
            if (session == null)
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

            foreach (var data in iRacing.GetDataFeed())
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
            Wait();

            ReplaySearch(ReplaySearchMode.NextIncident);

            Wait();
        }

        /// <summary>
        /// Select the camera onto a car and position to a previous incident marker
        /// </summary>
        public void MoveToPrevIncident()
        {
            Wait();

            ReplaySearch(ReplaySearchMode.PrevIncident);

            Wait();
        }

        public void MoveToNextLap()
        {
            Wait();

            ReplaySearch(ReplaySearchMode.NextLap);

            Wait();
        }

        public void MoveToNextFrame()
        {
            Wait();

            ReplaySearch(ReplaySearchMode.NextFrame);

            Wait();
        }

        public void MoveToPrevFrame()
        {
            Wait();

            ReplaySearch(ReplaySearchMode.PrevFrame);

            Wait();
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

        DataSample WaitAndVerify(Func<DataSample, bool> verifyFn, int wait = 5000)
        {
            return WaitAndVerify(verifyFn, () => { }, wait);
        }

        DataSample WaitAndVerify(Func<DataSample, bool> verifyFn)
        {
            return WaitAndVerify(verifyFn, () => { });
        }

        DataSample WaitAndVerify(Func<DataSample, bool> verifyFn, Action action, int wait = 5000)
        {
            if (iRacingInstance.IsRunning)
                return null;

            var timeout = DateTime.Now + TimeSpan.FromMilliseconds(wait);
            var data = iRacing.GetDataFeed().First();
            while ((!data.IsConnected || verifyFn(data)) && DateTime.Now < timeout)
            {
                action();
                data = iRacing.GetDataFeed().First();
                Thread.Sleep(100);
            }

            System.Diagnostics.Debug.Assert(!verifyFn(data));

            return data;
        }
    }
}
