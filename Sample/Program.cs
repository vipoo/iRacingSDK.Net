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

			//GetData_Main(args);
            //ChangeCamDriver();
            VerifyDataStream();
        }

        private static void VerifyDataStream()
        {
            var lastTickCount = 0;
            var lastFrame = 0;

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

                Thread.Sleep(14);
                lastTickCount = data.Telemetry.TickCount;
                lastFrame = data.Telemetry.ReplayFrameNum;
            }
        }

        public static void ChangeCamDriver()
        {
            var data = iRacing.GetDataFeed().First();
            var camera = data.SessionData.CameraInfo.Groups.First(g => g.GroupName == "TV1");

            var driverNumber = data.SessionData.DriverInfo.Drivers.Skip(1).First().CarNumber;

            iRacing.Replay.CameraOnDriver((short)driverNumber, (short)camera.GroupNum, 0);
        }

        public unsafe static void GetData_Main(string[] args)
        {
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
