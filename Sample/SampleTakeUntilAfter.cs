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

using System.Linq;
using iRacingSDK;
using iRacingSDK.Support;
using System.Diagnostics;

namespace Sample
{
    public class SampleTakeUntilAfter
    {
        public static void Sample()
        {
            var iracing = new iRacingConnection();

            Trace.WriteLine("Moving to start of race");
            iracing.Replay.MoveToStartOfRace();
            Trace.WriteLine("Watching first 2 laps + plus 5 seconds");

            int lastLap = -1;

            foreach (var data in iracing.GetDataFeed().AtSpeed(8)
                .TakeUntil(20.Seconds()).Of(data => data.Telemetry.RaceLaps == 2)
                .TakeUntil(20.Seconds()).AfterReplayPaused())
            {
                if (lastLap != data.Telemetry.RaceLaps)
                    Trace.WriteLine(string.Format("Lap: {0}", data.Telemetry.RaceLaps));
            
                lastLap = data.Telemetry.RaceLaps;
            }

            iracing.Replay.SetSpeed(0);
            Trace.WriteLine("Finished.");
        }
    }
}
