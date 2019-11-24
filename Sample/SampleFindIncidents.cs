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
    public class SampleFindIncidents
    {
        public static void Sample()
        {
            var iracing = new iRacingConnection();

            iRacing.Replay.MoveToStartOfRace();
            iRacing.Replay.MoveToFrame(-600, ReplayPositionMode.Current);
            
            var incidentSamples = iRacing.GetDataFeed().RaceIncidents2(100);

            foreach( var i in incidentSamples)
            {
                Trace.WriteLine(string.Format("Found new incident at frame {0} for {1}", i.Telemetry.SessionTimeSpan, i.Telemetry.CamCar.Details.UserName), "DEBUG");
            }

            Trace.WriteLine("Sample Finished");
        }
    }
}
