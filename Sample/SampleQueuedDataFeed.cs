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
using System.Threading;

namespace Sample
{
    public static class SampleQueuedDataFeed
    {
        /// <summary>
        /// Demonstrate the use of the GetQueuedDataFeed
        /// This method is similiar to GetDataFeed, except it buffers the DataSamples from iRacing
        /// This allow loops that occasionally take a little longer then 1/60th of a second.
        /// But it also means that the DataSamples yield into the enumeration may be a little out of date.
        /// </summary>
        public static void Sample()
        {
            var iracing = new iRacingConnection();

            iracing.Replay.MoveToStart();
            iracing.Replay.SetSpeed(1);

            var lastSector = new LapSector();
            var i = 0;

            foreach (var data in iracing.GetBufferedDataFeed(10))
            {
                if (data.Telemetry.RaceLapSector != lastSector)
                    Trace.WriteLine(string.Format("Lap: {0} Sector: {1}", data.Telemetry.RaceLapSector.LapNumber, data.Telemetry.RaceLapSector.Sector));

                if (i > 1)
                    Debug.Assert(data.LastSample != null, "LastSample should not be null");

                lastSector = data.Telemetry.RaceLapSector;

                if (i++ % 10 == 1)
                {
                    Trace.WriteLine("Pausing retrieval of data samples - simulating work");
                    Thread.Sleep(100);
                }
            }
        }
    }
}
