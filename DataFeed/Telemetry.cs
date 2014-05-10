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
		TimeSpan? sessionTimeSpan;
		public TimeSpan SessionTimeSpan 
		{
			get
			{ 
				if( sessionTimeSpan == null)
					sessionTimeSpan = TimeSpan.FromSeconds(SessionTime);

				return sessionTimeSpan.Value;
			}
		}

        public Car CamCar { get { return Cars[CamCarIdx]; } }

        float[] carIdxDistance;
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

        int[] positions;
        public int[] Positions
        {
            get
            {
                if (positions != null)
                    return positions;

                positions = new int[SessionData.DriverInfo.Drivers.Length];

                var runningOrder = CarIdxDistance
                    .Select((d, idx) => new { CarIdx = idx, Distance = d})
                    .Where( c => c.CarIdx != 0 )
                    .OrderByDescending(c => c.Distance)
                    .Select((c, order) => new { CarIdx = c.CarIdx, Position = order + 1 });

                positions[0] = int.MaxValue;
                foreach( var runner in runningOrder )
                    positions[runner.CarIdx] = runner.Position;

                return positions;
            }
        }

		Car[] cars;
		public Car[] Cars
		{
			get
			{
				if(cars != null)
					return cars;

                return cars = Enumerable.Range(0, this.SessionData.DriverInfo.Drivers.Length).Select(i => new Car(this, i)).ToArray();
			}
		}
	}
}
