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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using iRacingSDK.Support;

namespace Sample
{
    public static class SampleTotalDistane
    {
        public static void Sample()
        {
            var iracing = new iRacingConnection();

            iracing.Replay.MoveToStartOfRace();
            iracing.Replay.SetSpeed(1);

            var i = 0;

            foreach (var data in iracing.GetDataFeed()
                .WithCorrectedPercentages()
                .WithCorrectedDistances()
                .WithFinishingStatus())
            {
                i++;
                if( i % 200 == 0)
                {
                    MyListener.Clear();

                    var car = data.Telemetry.Cars
                        .Where(c => c.TotalDistance > 0)
                        .Where(c => !c.HasSeenCheckeredFlag)
                        .Where(c => !c.Details.IsPaceCar)
                        .Where(c => c.HasData)
                        .Where(c => c.Details.Driver != null)
                        .Where(c => c.TrackSurface == TrackLocation.OnTrack)
                        .OrderByDescending(c => c.TotalDistance)
                        .ThenBy(c => c.OfficialPostion == 0 ? int.MaxValue : c.OfficialPostion)
                        .FirstOrDefault();

                    Trace.WriteLine("=============================================================");
                    Trace.WriteLine(string.Format("Next Finisher is {0}", car == null ? "null" : car.Details.UserName));

                    Trace.WriteLine("Driver Distances");
                    Trace.WriteLine("================");

                    Trace.WriteLine("RaceDistance: {0}".F(data.Telemetry.RaceDistance));
                    Trace.WriteLine("");

                    foreach (var c in data.Telemetry.Cars.OrderByDescending(d => d.TotalDistance).ThenBy(c => c.OfficialPostion == 0 ? int.MaxValue : c.OfficialPostion))
                        Trace.WriteLine(string.Format("{0}, dist: {1}, pos: {2}, official pos: {3}", c.Details.UserName, c.TotalDistance, c.Position, c.OfficialPostion));
                }
            }
        }
    }
}
