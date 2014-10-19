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
using System.Threading;

namespace Sample
{
    public class SampleRaceEnd
    {
        public static void Sample()
        {
            var iracing = new iRacingConnection();

            Trace.WriteLine("Moving to 4 minutes before end of replay");

            iracing.Replay.MoveToFrame(4 * 60 * 60, ReplayPositionMode.End);
            iracing.Replay.SetSpeed(2);
            Thread.Sleep(3000);

            int lastCount = 0;

            foreach (var data in iracing.GetDataFeed().AtSpeed(8)
                .WithFinishingStatus()
                .TakeUntil(5.Seconds()).After(data => data.Telemetry.RaceCars.All( c => c.HasSeenCheckeredFlag || c.HasRetired ))
                .TakeUntil(2.Seconds()).AfterReplayPaused()
                )
            {
                var count = data.Telemetry.RaceCars.Count(c => c.HasSeenCheckeredFlag || c.HasRetired);

                if (lastCount != count)
                {
                    foreach( var x in data.Telemetry.RaceCars)
                        Trace.WriteLine(string.Format("{0,20}\tHasSeenCheckedFlag: {1}\tHasRetired: {2}", x.Details.UserName, x.HasSeenCheckeredFlag, x.HasRetired));

                    Trace.WriteLine(string.Format("{0} finishers", count));
                }
                lastCount = count;
            }

            iracing.Replay.SetSpeed(0);
            Trace.WriteLine("Finished.");
        }
    }
}
