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

using System;
using System.Collections.Generic;
using System.Threading;
using Win32.Synchronization;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;

namespace iRacingSDK
{
    public static class iRacing
    {
        static iRacingInstance instance;

        static iRacing()
        {
            instance = new iRacingInstance();
        }

        public static Replay Replay { get { return instance.Replay; } }

        public static bool IsConnected { get { return instance.IsConnected; } }

        public static IEnumerable<DataSample> GetDataFeed()
        {
            return instance.GetDataFeed();
        }

        public static void StartListening()
        {
            instance.StartListening();
        }

        public static void StopListening()
        {
            instance.StopListening();
        }

        public static event DataSampleEventHandler NewData
        {
            add
            {
                instance.NewData += value;
            }
            remove
            {
                instance.NewData -= value;
            }
        }
    }

	public partial class iRacingInstance : IDisposable
	{
        public readonly Replay Replay;
        public bool IsConnected { get; private set; }
        
        DataFeed dataFeed = null;
        bool isRunning = false;
        iRacingConnection iRacingConnection;
        internal bool IsRunning { get { return isRunning; } }
        
        public iRacingInstance()
        {
            this.Replay = new Replay(this);
            this.iRacingConnection = new iRacingConnection();
        }

        public IEnumerable<DataSample> GetDataFeed()
        {
            if (isRunning)
                throw new Exception("Can not call GetDataFeed concurrently.");

            isRunning = true;
            try
            {
                foreach (var notConnectedSample in WaitForInitialConnection())
                {
                    IsConnected = false;
                    yield return notConnectedSample;
                }

                foreach (var sample in AllSamples())
                {
                    IsConnected = sample.IsConnected;
                    yield return sample;
                }
            }
            finally
            {
                isRunning = false;
            }
        }

		IEnumerable<DataSample> WaitForInitialConnection()
		{
            bool wasConnected = iRacingConnection.Accessor != null;
            Trace.WriteLineIf(!wasConnected, "Waiting to connect to iRacing application", "INFO");
			
            while(!iRacingConnection.IsConnected())
			{
				yield return DataSample.YetToConnected;
				Thread.Sleep(10);
			}
            
            Trace.WriteLineIf(!wasConnected, "Connected to iRacing application", "INFO");
		}

		IEnumerable<DataSample> AllSamples()
		{
            if( dataFeed == null )
			    dataFeed = new DataFeed(iRacingConnection.Accessor);

            var nextTickCount = 0;
            var lastTickTime = DateTime.Now;
            DataSample lastDataSample = null;
			while(true)
			{
                iRacingConnection.WaitForData();

                var data = dataFeed.GetNextDataSample();
                if (data != null)
                {
                    data.LastSample = lastDataSample;
                    if (lastDataSample != null)
                        lastDataSample.LastSample = null;
                    lastDataSample = data;

                    if (data.IsConnected)
                    {
                        if (data.Telemetry.TickCount == nextTickCount - 1)
                            continue; //Got the same sample - try again.

                        if (data.Telemetry.TickCount != nextTickCount && nextTickCount != 0)
                            Debug.WriteLine(string.Format("Warning dropped DataSample from {0} to {1}. Over time of {2}",
                                nextTickCount, data.Telemetry.TickCount-1, (DateTime.Now - lastTickTime).ToString(@"s\.fff")), "WARN");

                        nextTickCount = data.Telemetry.TickCount + 1;
                        lastTickTime = DateTime.Now;
                    }
                    yield return data;
                }
			}
		}
	}
}
