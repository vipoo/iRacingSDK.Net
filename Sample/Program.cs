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
using Sample;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace iRacingSDKSample
{
    class MainClass
    {
        [STAThread]

        public static void Main(string[] args)
        {
            
            GuiSamples();
            //EventDataExample();

            //View_FinihsingStatus();

			//GetData_Main(args);
            //ChangeCamDriver();
            //VerifyDataStream();

            //VerifyLapSectors();

            //VerifyDriverDistances();

            //Recorder();
        }

        static void GuiSamples()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static void iRacing_NewData(DataSample data)
        {
            Console.WriteLine("Received Data {0}", data.Telemetry.TickCount);
        }

        static void Recorder()
        {
            var writer = new BinaryFormatter();

            using(var file = new FileStream(@"c:\recordedSamples.bin", FileMode.Create, FileAccess.Write, FileShare.None))
                foreach( var d in iRacing.GetDataFeed())
                    writer.Serialize(file, d);
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

                var next = ordered.FirstOrDefault();

                if (next == null)
                    break;

                var number = data.SessionData.DriverInfo.CompetingDrivers[next.CarIdx].CarNumberRaw;

                iRacing.Replay.CameraOnDriver((short)number, (short)camera.GroupNum, 0);

                Thread.Sleep(1000);
            }
        }

        public unsafe static void View_FinihsingStatus()
        {
            int[] lastLaps = new int[64];

            //WithCorrectedPercentages().WithCorrectedDistances()
            foreach (var data in iRacing.GetDataFeed().WithCorrectedPercentages().WithFinishingStatus())
            {
                //Console.Clear();

//                Console.WriteLine("Final Lap is {0}", data.Telemetry.IsFinalLap);
  //              Console.WriteLine("Is Finished {0}", data.Telemetry.LeaderHasFinished);

                for (int i = 1; i < data.SessionData.DriverInfo.CompetingDrivers.Length; i++)
                {
                    //Console.WriteLine("{0} lap is {1}", data.SessionData.DriverInfo.Drivers[i].UserName, data.Telemetry.CarIdxLap[i]);
                    //Console.WriteLine("{0} is finished {1}", data.SessionData.DriverInfo.Drivers[i].UserName, data.Telemetry.HasSeenCheckeredFlag[i]);

   //                 Console.WriteLine("{0} is on {1}", data.SessionData.DriverInfo.Drivers[i].UserName, data.Telemetry.CarIdxTrackSurface[i]);
     //               Console.WriteLine("{0} has no data {1}", data.SessionData.DriverInfo.Drivers[i].UserName, data.Telemetry.HasNoData(i));

                    //Console.WriteLine("{0} is retired {1}", data.SessionData.DriverInfo.Drivers[i].UserName, data.Telemetry.HasRetired[i]);

                    if (data.Telemetry.CarIdxTrackSurface[i] == TrackLocation.NotInWorld)
                    {
                        Console.WriteLine("{0} has no data at {1}", data.SessionData.DriverInfo.CompetingDrivers[i].UserName, data.Telemetry.SessionTime, data.Telemetry.ReplayFrameNum);
                    }
                }

                //Thread.Sleep(1000);
            }
        }

        public unsafe static void GetData_Main(string[] args)
        {
            int[] lastLaps = new int[64];

            //WithCorrectedPercentages().WithCorrectedDistances()
            foreach (var data in iRacing.GetDataFeed().WithCorrectedPercentages().WithFinishingStatus())
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
                        var name = data.SessionData.DriverInfo.CompetingDrivers[i].UserName;
                        var position = data.SessionData.SessionInfo.Sessions[2].ResultsPositions.First(r => r.CarIdx == i).Position;

                       // if (lastLaps[i] == data.SessionData.SessionInfo.Sessions[2].ResultsLapsComplete + 1)
                            Trace.WriteLine(string.Format("Driver {0} Cross line in position {1}", name, position));
                    }
                }
            }

            return;

#pragma warning disable CS0162 // Unreachable code detected
            foreach (var data in iRacing.GetDataFeed().WithCorrectedPercentages().AtSpeed(16).RaceOnly())
#pragma warning restore CS0162 // Unreachable code detected
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
