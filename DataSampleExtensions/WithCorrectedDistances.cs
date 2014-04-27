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
using System.Threading.Tasks;

namespace iRacingSDK
{
    public static partial class DataSampleExtensions
    {
        /// <summary>
        /// filter the DataSamples, and correct for when a car blips off due to data loss - and reports laps/distances as -1
        /// Ensures the lap/distances measures are only progressing upwards
        /// Does not support streaming across sessions, where laps/distrance will naturally go down
        /// Also does not support is gaming is playing replay in reverse.
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        public static IEnumerable<DataSample> WithCorrectedDistances(this IEnumerable<DataSample> samples)
        {
            var maxDistance = new float[64];

            foreach (var data in samples)
            {
                for (int i = 0; i < data.SessionData.DriverInfo.Drivers.Length; i++)
                {
                    var distance = data.Telemetry.CarIdxLap[i] + data.Telemetry.CarIdxLapDistPct[i];
                    if (distance > maxDistance[i])
                        maxDistance[i] = distance;

                    if (data.Telemetry.CarIdxLap[i] == -1)
                    {
                        Trace.WriteLine(string.Format("Adjusting distance for {0} back to {1}", i, distance));

                        data.Telemetry.CarIdxLap[i] = (int)maxDistance[i];
                        data.Telemetry.CarIdxLapDistPct[i] = maxDistance[i] - (int)maxDistance[i];
                    }
                }

                yield return data;
            }
        }
    }
}
