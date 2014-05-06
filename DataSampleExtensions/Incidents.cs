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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iRacingSDK
{
    public static partial class DataSampleExtensions
    {
        public static IEnumerable<DataSample> RaceIncidents(this IEnumerable<DataSample> samples)
        {
            iRacing.Replay.SetSpeed(0);
            iRacing.Replay.Wait();

            var sessionNumber = -1;

            var capturedIncidents = new List<DataSample>();

            DataSample data = null;
            while( data == null || data.Telemetry.SessionState != SessionState.CoolDown)
            {
                var frameNumber = data == null ? 0 : data.Telemetry.ReplayFrameNum;
                iRacing.Replay.MoveToNextIncident();
                data = samples.First();

                
                var retryCount = 4;
                while (data.Telemetry.ReplayFrameNum == frameNumber && retryCount >= 0)
                {
                    retryCount++;
                    Thread.Sleep(600); //Wait a bit more to ensure iRacing has moved to the incident
                    data = samples.First();
                }

                if (retryCount < 0)
                    break;

                if (sessionNumber == -1)
                    sessionNumber = data.Telemetry.SessionNum;

                capturedIncidents.Add(data);
            }

            iRacing.Replay.SetSpeed(0);
            iRacing.Replay.MoveToEnd();

            while (data.Telemetry.SessionNum == sessionNumber && data.Telemetry.RaceLaps > 0)
            {
                var frameNumber = data == null ? 0 : data.Telemetry.ReplayFrameNum;
                iRacing.Replay.MoveToPrevIncident();
                data = samples.First();

                var retryCount = 4;
                while (data.Telemetry.ReplayFrameNum == frameNumber && retryCount >= 0)
                {
                    retryCount++;
                    Thread.Sleep(600); //Wait a bit more to ensure iRacing has moved to the incident
                    data = samples.First();
                }

                if (retryCount < 0)
                    break;

                if (!capturedIncidents.Any(d => d.Telemetry.ReplayFrameNum == data.Telemetry.ReplayFrameNum))
                {
                    Trace.WriteLine(string.Format("Found new incident in reverse {0}", data.Telemetry.ReplayFrameNum));
                    capturedIncidents.Add(data);
                }
                else
                    Trace.WriteLine(string.Format("Found previously found incident {0}", data.Telemetry.ReplayFrameNum));
            }

            return capturedIncidents.OrderBy( d => d.Telemetry.ReplayFrameNum).ToList();
        }
    }
}

