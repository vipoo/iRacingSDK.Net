using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    public static class SampleFastestLap
    {
        public static void Sample()
        {
            FastLap lastFastestLap = null;
            var iracing = new iRacingConnection();

            Trace.WriteLine("Moving to start of race");
            iracing.Replay.MoveToStartOfRace();
            Trace.WriteLine("Watching for fastest laps");

            foreach (var data in iracing.GetDataFeed().AtSpeed(16).WithFastestLaps())
            {
                if (lastFastestLap != data.Telemetry.FastestLap)
                    Trace.WriteLine(string.Format("{0} - {1}", data.Telemetry.FastestLap.Driver.UserName, data.Telemetry.FastestLap.Time));

                lastFastestLap = data.Telemetry.FastestLap;
            }
        }
    }
}
