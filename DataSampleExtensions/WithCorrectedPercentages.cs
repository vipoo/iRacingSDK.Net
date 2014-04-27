﻿// This file is part of iRacingSDK.
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
        /// Work around bug in iRacing data stream, where cars lap percentage is reported slightly behind 
        /// actual frame - so that as cars cross the line, their percentage still is in the 99% range
        /// a frame later there percentage drops to near 0%
        /// Fix is to watch for lap change - and zero percentage if greater than 90%
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        public static IEnumerable<DataSample> WithCorrectedPercentages(this IEnumerable<DataSample> samples)
        {
            var lastLaps = new int[64];
            var zeroOnFrame = new int[64];

            for (var i = 0; i < 64; i++)
                lastLaps[i] = 0;

            foreach (var data in samples)
            {
                var count = data.SessionData.DriverInfo.Drivers.Length;
                var telemetry = data.Telemetry;

                for (int i = 0; i < count; i++)
                    FixPercentagesOnLapChange(lastLaps, zeroOnFrame, telemetry, i);

                yield return data;
            }
        }

        static void FixPercentagesOnLapChange(int[] lastLaps, int[] zeroOnFrame, Telemetry telemetry, int i)
        {
            if (telemetry.CarIdxLap[i] == -1)
                return;

            var carIdxLapDistPct = telemetry.CarIdxLapDistPct[i];

            if (zeroOnFrame[i] == telemetry.ReplayFrameNum && carIdxLapDistPct > 0.90)
            {
                telemetry.CarIdxLapDistPct[i] = 0;
            }
            else
            {
                var carIdxLap = telemetry.CarIdxLap[i];
                var lastLap = lastLaps[i];

                if (lastLap == carIdxLap)
                    return;
             
                if (carIdxLap == lastLap + 1 && carIdxLapDistPct > 0.90)
                {
                    telemetry.CarIdxLapDistPct[i] = 0;
                    zeroOnFrame[i] = telemetry.ReplayFrameNum;
                }
                else
                    lastLaps[i] = carIdxLap;
            }
        }
    }
}
