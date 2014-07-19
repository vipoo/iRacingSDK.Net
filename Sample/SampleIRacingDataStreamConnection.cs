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
using System.Diagnostics;
using System.Threading;

namespace Sample
{
    public static class SampleIRacingDataStreamConnection
    {
        public static void Sample()
        {
            var iracing = new iRacingConnection();
            var ieventRacing = new iRacingEvents();

            ieventRacing.Connected += ieventRacing_Connected;
            ieventRacing.Disconnected += ieventRacing_Disconnected;
            ieventRacing.StartListening();

            try
            {
                var isStarted = false;
                var isConnected = false;
                var isDisconnected = true;
                var i = 0;

                foreach (var d in iracing.GetDataFeed())
                {
                    if (!isStarted)
                    {
                        Trace.WriteLine(string.Format("Starting data steam - IsConnected = {0}", d.IsConnected));
                        isStarted = true;
                        isConnected = d.IsConnected;
                        isDisconnected = !isConnected;
                    }

                    if (!isConnected && d.IsConnected)
                    {
                        Trace.WriteLine("Connected to iRacing data stream");
                        isConnected = true;
                        isDisconnected = false;
                    }

                    if (!isDisconnected && !d.IsConnected)
                    {
                        Trace.WriteLine("Application is disconnected");
                        isConnected = false;
                        isDisconnected = true;
                    }

                    if (i++ % 600 == 0)
                    {
                        Trace.WriteLine(string.Format("Data Stream IsConnected = {0}", d.IsConnected));
                    }
                }
            }
            finally
            {
                ieventRacing.StopListening();
            }
        }

        static void ieventRacing_Disconnected()
        {
            Trace.WriteLine("Notified of application data disconnection from event handler");
        }

        static void ieventRacing_Connected()
        {
            Trace.WriteLine("Notified of application data connection from event handler");
        }
    }
}
