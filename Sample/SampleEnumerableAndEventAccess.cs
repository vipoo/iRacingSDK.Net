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
            var instance1 = new iRacingConnection();
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
