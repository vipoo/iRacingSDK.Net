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

namespace iRacingSDK
{
    public static partial class DataSampleExtensions
    {
        /// <summary>
        /// Filters out DataSamples that have a ReplayFrameNumber of 0.
        /// </summary>
        public static IEnumerable<DataSample> VerifyReplayFrames(this IEnumerable<DataSample> samples)
        {
            bool hasLoggedBadSample = false;

            foreach (var data in samples)
            {
                if( data.Telemetry.ReplayFrameNum != 0)
                    yield return data;
                else if( !hasLoggedBadSample )
                {
                    hasLoggedBadSample = true;

                    Trace.WriteLine("Received bad sample.  No ReplayFrameNumber.", "DEBUG");
                    Trace.WriteLine(data.Telemetry.ToString(), "DEBUG");
                }
            }
        }
    }
}
