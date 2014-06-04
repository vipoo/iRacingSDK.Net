using iRacingSDK;
using System.Diagnostics;

namespace Sample
{
    public static class SampleLapSector
    {
        public static void Sample()
        {
            var iracing = new iRacingConnection();

            iracing.Replay.MoveToStartOfRace();
            iracing.Replay.SetSpeed(16);
            
            var lastSector = new LapSector();

            foreach (var data in iRacing.GetDataFeed().AtSpeed(16))
            {
                if (data.Telemetry.RaceLapSector != lastSector)
                    Trace.WriteLine(string.Format("Lap: {0} Sector: {1}", data.Telemetry.RaceLapSector.LapNumber, data.Telemetry.RaceLapSector.Sector));

                lastSector = data.Telemetry.RaceLapSector;
            }
        }
    }
}
