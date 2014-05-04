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
    public partial class Telemetry : Dictionary<string, object>
    {
        public bool[] HasSeenCheckeredFlag;
        public bool IsFinalLap;
        public bool LeaderHasFinished;
        public bool[] HasRetired;

        public bool HasData(int carIdx)
        {
            return this.CarIdxTrackSurface[carIdx] != TrackLocation.NotInWorld;
        }
    }

    public static partial class DataSampleExtensions
    {
        /// <summary>
        /// Assignes the telemetry fields HasSeenCheckeredFlag and IsFinalLap
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        public static IEnumerable<DataSample> WithFinishingStatus(this IEnumerable<DataSample> samples)
        {
            var hasSeenCheckeredFlag = new bool[64];
            var timeWhenDataStopped = new double[64];

            foreach (var data in samples)
            {
                data.Telemetry.IsFinalLap = data.Telemetry.RaceLaps >= data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum].ResultsLapsComplete;

                if (data.Telemetry.RaceLaps > data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum].ResultsLapsComplete)
                    data.Telemetry.LeaderHasFinished = true;

                /*
                for (int i = 0; i < data.SessionData.DriverInfo.Drivers.Length; i++)
                {
                    if (data.Telemetry.HasData(i))
                        timeWhenDataStopped[i] = data.Telemetry.SessionTime;
                    else
                    {
                        if( timeWhenDataStopped[i])
                    }

                }*/

                    if (data.LastSample != null && data.Telemetry.LeaderHasFinished)
                        for (int i = 1; i < data.SessionData.DriverInfo.Drivers.Length; i++)
                            if (data.LastSample.Telemetry.CarIdxLapDistPct[i] > 0.90 && data.Telemetry.CarIdxLapDistPct[i] < 0.10)
                                hasSeenCheckeredFlag[i] = true;

                data.Telemetry.HasSeenCheckeredFlag = hasSeenCheckeredFlag;
                
                yield return data;
            }
        }
    }
}
