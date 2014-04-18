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
	public class Telemetry : Dictionary<string, object>
	{
		internal _SessionInfo SessionInfo { get; set; }

		public int[] 	CarIdxLap				{ get { return (int[])		this["CarIdxLap"]; 			} }
		public float[] 	CarIdxLapDistPct 		{ get { return (float[])	this["CarIdxLapDistPct"]; 	} }
        public int      SessionNum              { get { return (int)        this["SessionNum"];         } }
		public double   SessionTime 			{ get { return (double)		this["SessionTime"]; 		} }

        public int SessionLapsRemain { get { return (int)this["SessionLapsRemain"]; } }
        public double SessionTimeRemain { get { return (double)this["SessionTimeRemain"]; } }
        public int CamCarIdx { get { return (int)this["CamCarIdx"]; } }

        public Car CamCar { get { return Cars.FirstOrDefault(c => c.Index == CamCarIdx); } }

		Car[] cars;
		public Car[]   Cars
		{
			get
			{
				if(cars != null)
					return cars;

				cars = new Car[CarIdxLap.Length];
				for(int i = 0; i < CarIdxLap.Length; i++)
					cars[i] = new Car {
						Index = i,
						Driver = SessionInfo.DriverInfo.Drivers.FirstOrDefault(d => d.CarIdx == i),
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
		public Driver Driver {	get; internal set; }
	}
}
