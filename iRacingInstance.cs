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
using iRacingSDK;

namespace iRacingSDK
{
	public partial class iRacingConnection : IDisposable
	{
        public readonly Replay Replay;
        public bool IsConnected { get; private set; }
        
        DataFeed dataFeed = null;
        bool isRunning = false;
        iRacingMemory iRacingMemory;
        internal bool IsRunning { get { return isRunning; } }
        
        public iRacingConnection()
        {
            this.Replay = new Replay(this);
            this.iRacingMemory = new iRacingMemory();
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
            bool wasConnected = iRacingMemory.Accessor != null;
            Trace.WriteLineIf(!wasConnected, "Waiting to connect to iRacing application", "INFO");

            while (!iRacingMemory.IsConnected())
			{
				yield return DataSample.YetToConnected;
				Thread.Sleep(10);
			}
            
            Trace.WriteLineIf(!wasConnected, "Connected to iRacing application", "INFO");
		}

		IEnumerable<DataSample> AllSamples()
		{
            if( dataFeed == null )
                dataFeed = new DataFeed(iRacingMemory.Accessor);

            var nextTickCount = 0;
            var lastTickTime = DateTime.Now;
            DataSample lastDataSample = null;
			while(true)
			{
                iRacingMemory.WaitForData();

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
