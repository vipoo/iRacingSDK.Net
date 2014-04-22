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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingSDK
{
    public static partial class DataSampleExtensions
    {
        public class DataSampleForSectorChange : DataSample
        {
            public int[] CarSectorIdx; //0 -> Start/Finish, 1 -> 33%, 2-> 66%
        }

        public static IEnumerable<DataSample> OnDriverCrossesSector(this IEnumerable<DataSample> samples)
        {
            var lastRaceLaps = -1;

            foreach (var data in samples)
            {
                if (lastRaceLaps != data.Telemetry.RaceLaps)
                {
                    lastRaceLaps = data.Telemetry.RaceLaps;
                    yield return new DataSampleForSectorChange
                    {
                        IsConnected = true,
                        SessionData = data.SessionData,
                        Telemetry = data.Telemetry,
                        CarSectorIdx = new int[1]
                    };
                }

                if (data.Telemetry.CarIdxLapDistPct.Any(p => p > 0.33)) //Not quite - on transition
                {
                    yield return new DataSampleForSectorChange
                    {
                        IsConnected = true,
                        SessionData = data.SessionData,
                        Telemetry = data.Telemetry,
                        CarSectorIdx = new int[1]
                    };
                    //return at sector 1
                }


                if (data.Telemetry.CarIdxLapDistPct.Any(p => p > 0.22)) //Not quite - on transition
                {
                    yield return new DataSampleForSectorChange
                    {
                        IsConnected = true,
                        SessionData = data.SessionData,
                        Telemetry = data.Telemetry,
                        CarSectorIdx = new int[1]
                    };
                    //return at sector 1
                }
            }
        }
    }
}
