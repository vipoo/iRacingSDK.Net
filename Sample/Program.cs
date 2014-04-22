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

namespace SpikeIRSDK
{

    class MainClass
    {
         [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
         private static extern int RegisterWindowMessage(string lpString);

         [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
         static extern bool SendNotifyMessageA(IntPtr hWnd, int Msg, int wParam, int lParam);

        static IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        public static int FromShorts(short lowPart, short highPart)
        {
            return ((int)highPart << 16) | (ushort)lowPart;
        }
        
        public static void Main(string[] args)
        {
            CaptureDriverPositionPerLap();
			// GetData_Main(args);
        }

        public static void CaptureDriverPositionPerLap()
        {
            foreach( var data in iRacing.GetDataFeed().TakeWhile(d => !d.IsConnected))
            {
                Console.WriteLine("Waiting to connect ...");
                continue;
            }

            var data2 = iRacing.GetDataFeed().First();

            //Goto to race start

            iRacing.Replay.MoveToStart();
            data2 = iRacing.GetDataFeed().First();
			Console.WriteLine(data2.Telemetry.SessionFlags);

			while(data2.SessionData.SessionInfo.Sessions[data2.Telemetry.SessionNum].SessionType != "Race")
			{
				iRacing.Replay.MoveToNextSession();
				data2 = iRacing.GetDataFeed().First();
				Console.WriteLine(data2.Telemetry.SessionFlags);

			}

			foreach( var data in iRacing.GetDataFeed() )
				Console.WriteLine("{0}, {1}", data.Telemetry.SessionTime, data.Telemetry.SessionState);
				
            //Goto lap 0!
            
        }

		/*  public static void Spike()
        {
            var IRSDK_BROADCASTMSGNAME = "IRSDK_BROADCASTMSG";

            var msgId = RegisterWindowMessage(IRSDK_BROADCASTMSGNAME);

            var msg = (short)irsdk_BroadcastMsg.irsdk_BroadcastCamSwitchPos;
            short var1 = 1;
            short var2 = 1;
            short var3 = 0;

            var var23 = FromShorts(var2, var3);
            var msgVar1 = FromShorts(msg, var1);

            var r = SendNotifyMessageA(HWND_BROADCAST, msgId, msgVar1, var23);
            Console.WriteLine(r);

            msg = (short)irsdk_BroadcastMsg.irsdk_BroadcastReplaySetPlaySpeed;
            var1 = 16;
            var2 = 1; //true
            var3 = 0;

            var23 = FromShorts(var2, var3);
            msgVar1 = FromShorts(msg, var1);

/*x
1048579
var2
1


            r = SendNotifyMessageA(HWND_BROADCAST, msgId, msgVar1, var23);
            Console.WriteLine(r);

            //irsdk_broadcastMsg(irsdk_BroadcastReplaySetPlaySpeed, 16, true, 0);
        }
	*/
        public unsafe static void GetData_Main(string[] args)
        {
			foreach (var data in iRacing.GetDataFeed())
            {
                if (!data.IsConnected)
                {
                    Console.WriteLine("Waiting to connect ...");
                    continue;
                }

                Console.Clear();

                Console.WriteLine("Session Data");
                //Console.WriteLine(data.SessionData.Raw);

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
                
                //return;
                Thread.Sleep(2000);
            }
        }
    }			
}
