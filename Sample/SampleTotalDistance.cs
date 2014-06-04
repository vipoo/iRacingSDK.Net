using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sample
{
    public static class SampleTotalDistane
    {
        public static void Sample()
        {
            var iracing = new iRacingConnection();

            iracing.Replay.MoveToStart();
            iracing.Replay.SetSpeed(1);

            foreach (var data in iracing.GetDataFeed())
            {
                Trace.WriteLine("Driver Distances");
                Trace.WriteLine("================");

                foreach (var c in data.Telemetry.Cars.OrderBy(d => d.TotalDistance))
                {
                    Trace.WriteLine(string.Format("{0} at {1}", c.UserName, c.TotalDistance));
                }

                Thread.Sleep(4000);
            }
        }
    }
}
