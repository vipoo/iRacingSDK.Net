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
using System.Diagnostics;
using System.Threading;

namespace Sample
{
    public static class SamplePerformanceMeasurement
    {
        public static void Sample()
        {
            var iracing = new iRacingConnection();
            var i = 0;

            foreach (var data in iracing.GetDataFeed())
                if (i++ % 100 == 0)
                {
                    Trace.WriteLine("Processing Time: {0} micro seconds".F(iracing.ProcessingTime));
                    Trace.WriteLine("Waiting Time: {0} micro seconds".F(iracing.WaitingTime));
                    Trace.WriteLine("Yield Time: {0} micro seconds".F(iracing.YieldTime));
                    Trace.WriteLine("");
                }
        }
    }
}
