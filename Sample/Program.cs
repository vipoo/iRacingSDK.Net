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
using System.Linq;
using iRacingSDK;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using YamlDotNet.RepresentationModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace iRacingSDKSample
{
    class MainClass
    {        
        public static void Main(string[] args)
        {
            foreach (var data in iRacing.GetDataFeed().TakeWhile(d => !d.IsConnected))
            {
                Console.WriteLine("Waiting to connect ...");
                continue;
            }

			GetData_Main(args);
            //ChangeCamDriver();
            //VerifyDataStream();
        }

        private static void VerifyDataStream()
        {
            var lastTickCount = 0;
            var lastFrame = 0;

            var firstData = iRacing.GetDataFeed().First();
            var offset = firstData.Telemetry.SessionTime - (float)firstData.Telemetry.ReplayFrameNum / 60f;

            foreach( var data in iRacing.GetDataFeed() )
            {
                if( data.Telemetry.TickCount != lastTickCount+1 )
                {
                    Console.WriteLine("Tick count glitch {0}, {1}", lastTickCount, data.Telemetry.TickCount);
                }

                if( data.Telemetry.ReplayFrameNum != lastFrame+1)
                {
                    Console.WriteLine("Frame number count glitch {0}, {1}", lastFrame, data.Telemetry.ReplayFrameNum);
                }

                var newTimeDelta = (int)((float)data.Telemetry.ReplayFrameNum/ 60f + offset - data.Telemetry.SessionTime) * 1000;

                if( newTimeDelta != 0)
                    Console.WriteLine("Frame time {0}", newTimeDelta);

                Thread.Sleep(13);
                lastTickCount = data.Telemetry.TickCount;
                lastFrame = data.Telemetry.ReplayFrameNum;
            }
        }

        public static void ChangeCamDriver()
        {
            var data = iRacing.GetDataFeed().First();
            var camera = data.SessionData.CameraInfo.Groups.First(g => g.GroupName == "TV3");

            while(true)
            {
                data = iRacing.GetDataFeed().First();
             
                var ordered = data.Telemetry.CarIdxDistance
                    .Select((d, i) => new { CarIdx = i, Distance = d })
                    .Skip(1)
                    .Where(d => d.Distance > 0)
                    .OrderByDescending(d => d.Distance)
                    .Where(d => d.Distance <= data.SessionData.SessionInfo.Sessions[2].ResultsLapsComplete + 1.01)
                    .ToArray();

                Trace.WriteLine(data.Telemetry.SessionState);

                var next = ordered.FirstOrDefault();

                if (next == null)
                    break;

                var number = data.SessionData.DriverInfo.Drivers[next.CarIdx].CarNumber;

                iRacing.Replay.CameraOnDriver((short)number, (short)camera.GroupNum, 0);

                Thread.Sleep(1000);
            }
            
            //var camera = data.SessionData.CameraInfo.Groups.First(g => g.GroupName == "Pit Lane");

            //var driverNumber = 1; // data.SessionData.DriverInfo.Drivers.Skip(1).First().CarNumber;

            //iRacing.Replay.CameraOnDriver((short)driverNumber, (short)camera.GroupNum, 0);
        }

        public unsafe static void GetData_Main(string[] args)
        {
            int[] lastLaps = new int[64];

            foreach (var data in iRacing.GetDataFeed().WithCorrectedPercentages().WithCorrectedDistances())
            {
                

                if( data.Telemetry.SessionState == SessionState.CoolDown)
                {
                    Trace.WriteLine("Finished.");
                    break;
                }

                if (data.Telemetry.RaceLaps <= data.SessionData.SessionInfo.Sessions[2].ResultsLapsComplete)
                {
                    for( int i = 0; i < 10; i++)
                        lastLaps[i] = data.Telemetry.CarIdxLap[i];
                }
                else
                for( int i = 0; i < 10; i++)
                {
                    if( lastLaps[i] != data.Telemetry.CarIdxLap[i])
                    {
                        lastLaps[i] = data.Telemetry.CarIdxLap[i];
                        var name = data.SessionData.DriverInfo.Drivers[i].UserName;
                        var position = data.SessionData.SessionInfo.Sessions[2].ResultsPositions.First(r => r.CarIdx == i).Position;

                       // if (lastLaps[i] == data.SessionData.SessionInfo.Sessions[2].ResultsLapsComplete + 1)
                            Trace.WriteLine(string.Format("Driver {0} Cross line in position {1}", name, position));
                    }
                }
            }

            return;

            foreach (var data in iRacing.GetDataFeed().WithCorrectedPercentages().AtSpeed(16).RaceOnly())
            {
                Console.Clear();

                Console.WriteLine("Session Data");





                Console.WriteLine("Telemtary");

				foreach(var kv in data.Telemetry)
                {
                    Console.WriteLine("{0} = {1}", kv.Key, kv.Value);
                }

                var session = data.SessionData.SessionInfo.Sessions.First(s => s.SessionNum == data.Telemetry.SessionNum);
                Console.WriteLine("SessionLaps = {0}", session.SessionLaps);
                Console.WriteLine("SessionTime = {0}", session.SessionTime);
                Console.WriteLine("SessionTimeRemaing = {0}", data.Telemetry.SessionTimeRemain);
                Console.WriteLine("SessionLapRemining = {0}", data.Telemetry.SessionLapsRemain);
                
                Thread.Sleep(2000);
            }
        }
    }			
}
