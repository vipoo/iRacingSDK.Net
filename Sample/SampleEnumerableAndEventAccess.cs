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
using System;
using System.Diagnostics;

namespace Sample
{
    public static class SampleEnumerableAndEventAccess
    {
        static System.Collections.Concurrent.ConcurrentQueue<string> traceMessages = new System.Collections.Concurrent.ConcurrentQueue<string>();

        public static void Sample()
        {
            var instance1 = new iRacingEvents();
            instance1.NewData += instance1_NewData;
            instance1.StartListening();

            var iracingInstance = new iRacingConnection();

            var start = DateTime.Now;
            foreach (var data in iracingInstance.GetDataFeed())
            {
                if (DateTime.Now - start > TimeSpan.FromSeconds(1))
                    break;

                traceMessages.Enqueue(string.Format("Enumerable Data Tick {0}", data.Telemetry.TickCount));
            }

            instance1.StopListening();

            foreach (var m in traceMessages)
                Trace.WriteLine(m);
        }

        static void instance1_NewData(DataSample data)
        {
            traceMessages.Enqueue(string.Format("Event Data Tick {0}", data.Telemetry.TickCount));
        }
    }
}
