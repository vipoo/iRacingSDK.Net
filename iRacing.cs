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
	public partial class iRacing
	{
        public static readonly Replay Replay = new Replay();
        static DataFeed dataFeed = null;

        static bool isRunning = false;

        public static IEnumerable<DataSample> GetDataFeed()
        {
            if (isRunning)
                throw new Exception("Can not call GetDataFeed concurrently.");

            isRunning = true;
            try
            {
                foreach (var notConnectedSample in WaitForInitialConnection())
                    yield return notConnectedSample;

                foreach (var sample in AllSamples())
                    yield return sample;
            }
            finally
            {
                isRunning = false;
            }
        }

		static IEnumerable<DataSample> WaitForInitialConnection()
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

		static IEnumerable<DataSample> AllSamples()
		{
            if( dataFeed == null )
			    dataFeed = new DataFeed(iRacingConnection.Accessor);

            var nextTickCount = 0;
            DataSample lastDataSample = null;
			while(true)
			{
                if (!iRacingConnection.WaitForData())
                    yield return DataSample.YetToConnected;

				var data = dataFeed.GetNextDataSample();
                if (data != null)
                {
                    data.LastSample = lastDataSample;
                    if (lastDataSample != null)
                        lastDataSample.LastSample = null;
                    lastDataSample = data;

                    if (data.IsConnected)
                    {
                        if (data.Telemetry.TickCount != nextTickCount && nextTickCount != 0)
                            Debug.WriteLine(string.Format("Warning tick count glitch - {0}, {1}", data.Telemetry.TickCount, nextTickCount), "WARN");
                     
                        nextTickCount = data.Telemetry.TickCount + 1;
                    }
                    yield return data;
                }
			}
		}
	}
}
