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
    public static class SampleCarAndSessionState
    {
        public static void Sample()
        {
            var iracing = new iRacingConnection();

            iracing.Replay.MoveToStartOfRace();
            iracing.Replay.SetSpeed(16);

            foreach (var data in iRacing.GetDataFeed().AtSpeed(2))
            {
                Trace.WriteLine("Session State: {0}".F(data.Telemetry.SessionState));
                Trace.WriteLine("Session Flags: {0}".F(data.Telemetry.SessionFlags));

                Trace.WriteLine("Pace Car Location: {0}".F(data.Telemetry.CarIdxTrackSurface[0]));
                Trace.WriteLine("Under pace car: {0}".F(data.Telemetry.UnderPaceCar));

                Trace.WriteLine("Position:{0}".F(data.Telemetry.PlayerCarPosition));

                Trace.WriteLine("Session Flags: {0}".F(data.Telemetry.SessionFlags.ToString()));
                Trace.WriteLine("Engine Warnings: {0}".F(data.Telemetry.EngineWarnings.ToString()));

                Trace.WriteLine("\n\n");
                Thread.Sleep(2000);
            }
        }
    }
}
