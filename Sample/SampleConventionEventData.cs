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
    /// <summary>
    /// This sample demonstrates a typical way to access your data using a conventional
    /// event model.
    /// </summary>
    public static class SampleConventionalEventData
    {
        public static void Sample()
        {
            var ieventRacing = new iRacingEvents(1.Seconds());

            ieventRacing.Connected += ieventRacing_Connected;
            ieventRacing.Disconnected += ieventRacing_Disconnected;
            ieventRacing.NewData += ieventRacing_NewData;

            Trace.WriteLine("This sample show how to access game data through an event paradigm.");
            
            //The following statement would be best in your form_load handler
            ieventRacing.StartListening();

            //Simulate a form opened for 10 seconds.
            Thread.Sleep(10000);

            //Stop listening when your app shuts down (say inside your form_unload handler).
            ieventRacing.StopListening();

            Trace.WriteLine("Sample finished.");
        }

        static void ieventRacing_NewData(DataSample data)
        {
            //You can access your game data here
            var x = data.Telemetry.SessionTimeSpan;
            var z = data.Telemetry["AirTemp"];

            var y = data.SessionData.WeekendInfo.TrackDisplayName;
            var r = data.Telemetry.SessionData.Raw;


            Trace.WriteLine("Time: {0}".F(x));
        }

        static void ieventRacing_Disconnected()
        {
            Trace.WriteLine("Notified by iRacingEvents of application data disconnection from event handler");
        }

        static void ieventRacing_Connected()
        {
            Trace.WriteLine("Notified by iRacingEvents of application data connection from event handler");
        }
    }
}
