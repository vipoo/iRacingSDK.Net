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

namespace SpikeIRSDK
{
    class MainClass
    {
        public unsafe static void Main(string[] args)
        {
            foreach (var data in iRacing.GetDataFeed())
            {
                if (!data.IsConnected)
                {
                    Console.Clear();
                    Console.WriteLine("Waiting to connect ...");
                    continue;
                }

                var numberOfDrivers = data.SessionInfo.DriverInfo.Drivers.Length;

                var positions = data.Telementary.Cars
                    .Take(numberOfDrivers)
                    .Where(c => c.Index != 0)
                    .OrderByDescending(c => c.Lap + c.DistancePercentage)
                    .ToArray();

                Console.Clear();
                foreach (var p in positions)
                {
                    Console.Write(p.Driver.UserName);
                    Console.Write(" ");
                    Console.Write(p.Lap);
                    Console.Write(" ");

                    Console.WriteLine(p.DistancePercentage);
                }

                Thread.Sleep(2000);
            }
        }
    }			
}
