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
using System.Linq;

namespace iRacingSDK
{
	public partial class Telemetry : Dictionary<string, object>
	{
        public Car CamCar { get { return Cars.FirstOrDefault(c => c.Index == CamCarIdx); } }

        private float[] carIdxDistance;
        public float[] CarIdxDistance
        {
            get
            {
                if (carIdxDistance == null)
                    carIdxDistance = Enumerable.Range(0, this.SessionData.DriverInfo.Drivers.Length)
                        .Select(CarIdx => this.CarIdxLap[CarIdx] + this.CarIdxLapDistPct[CarIdx] )
                        .ToArray();

                return carIdxDistance;
            }
        }

		Car[] cars;
		public Car[] Cars
		{
			get
			{
				if(cars != null)
					return cars;

                cars = new Car[this.SessionData.DriverInfo.Drivers.Length];
                for (int i = 0; i < this.SessionData.DriverInfo.Drivers.Length; i++)
					cars[i] = new Car {
						Index = i,
						Driver = SessionData.DriverInfo.Drivers[i],
						DistancePercentage = CarIdxLapDistPct[i],
						Lap	= CarIdxLap[i]
					};

				return cars;
			}
		}
	}

	public class Car
	{
		public int Index { get; internal set; }
		public int Lap { get; internal set; }
		public float DistancePercentage { get; internal set; }
		public SessionData._DriverInfo._Drivers Driver { get; internal set; }
	}
}
