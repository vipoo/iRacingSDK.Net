// This file is part of iRacingSDK.
//
// Copyright © 2020 Merlin Cooper  (https://github.com/MerlinCooper/iRacingSDK.Net)
// based on iRacingSDK © Dean Netherton
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
    public static class SampleCameraInfo
    {
        public static void Sample()
        {

            var iracing = new iRacingConnection();

            foreach (var data in iracing.GetDataFeed()
                .WithCorrectedPercentages()
                .WithCorrectedDistances()
                .WithPitStopCounts())
            {

                Trace.WriteLine("CamGroupNumber selected: {0}".F(data.Telemetry.CamGroupNumber));
                Thread.Sleep(1000);
            }
        }
    }
}
