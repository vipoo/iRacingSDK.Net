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

using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iRacingSDK
{
    public static partial class DataSampleExtensions
    {
        /// <summary>
        /// Set the CarIdxPitStopCount field for each enumerted datasample's telemetry
        /// </summary>
        public static IEnumerable<DataSample> WithPitStopCounts(this IEnumerable<DataSample> samples)
        {
            int[] carIdxPitStopCount = new int[64];

            foreach (var data in samples)
            {
                if (data.LastSample != null)
                    PopulateCarIdxPitStopCount(data.LastSample.Telemetry, data.Telemetry, carIdxPitStopCount);

                data.Telemetry.CarIdxPitStopCount = (int[])carIdxPitStopCount.Clone();

                yield return data;
            }
        }

        static void PopulateCarIdxPitStopCount(Telemetry last, Telemetry telemetry, int[] carIdxPitStopCount)
        {
            for (var i = 0; i < telemetry.CarIdxTrackSurface.Length; i++)
                if (last.CarIdxTrackSurface[i] != TrackLocation.InPitStall && telemetry.CarIdxTrackSurface[i] == TrackLocation.InPitStall)
                {
                    carIdxPitStopCount[i] += 1;
                    TraceInfo.WriteLine("{0} Driver {1} has pitted {2} times", telemetry.SessionTimeSpan, telemetry.Cars[i].Details.UserName, carIdxPitStopCount[i]);
                }
        }
    }
}
