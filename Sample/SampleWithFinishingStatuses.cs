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

using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Sample
{
    public static class SampleWithFinishingStatuses
    {
        public static void Sample()
        {
            var iracing = new iRacingConnection();

            iracing.Replay.MoveToStartOfRace();
            iracing.Replay.SetSpeed(8);

            var time = new TimeSpan();
            iracing.Replay.SetSpeed(1);



            foreach (var data in iracing.GetDataFeed().WithFinishingStatus())
            {
                if( data.Telemetry.SessionTimeSpan > time)
                {
                    MyListener.Clear();

                    
   

                    foreach (var c in data.Telemetry.Cars.Where(c => !c.Details.IsPaceCar))
                        Trace.WriteLine(string.Format("{0,-20}\tCheckedFlag: {1}\tRetired: {2}\tData:{3}", c.Details.UserName, c.HasSeenCheckeredFlag, c.HasRetired, c.HasData));

                    Trace.WriteLine("");
                    Trace.WriteLine(string.Format("IsFinalLap: {0}, LeaderHasFinished: {1}", data.Telemetry.IsFinalLap, data.Telemetry.LeaderHasFinished));
                    Trace.WriteLine("");
                    Trace.WriteLine("");

                    time = data.Telemetry.SessionTimeSpan + (5*8).Seconds();
                }
            }
        }
    }
}
