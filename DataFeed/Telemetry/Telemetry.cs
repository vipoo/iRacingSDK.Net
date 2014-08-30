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
        public SessionData._SessionInfo._Sessions Session { get { return SessionData.SessionInfo.Sessions[SessionNum]; } }

        public Car CamCar { get { return Cars[CamCarIdx]; } }

        Car[] cars;
        public Car[] Cars
        {
            get
            {
                if (cars != null)
                    return cars;

                return cars = Enumerable.Range(0, this.SessionData.DriverInfo.Drivers.Length).Select(i => new Car(this, i)).ToArray();
            }
        }

        public IEnumerable<Car> RaceCars
        {
            get
            {
                return Cars.Where(c => !c.IsPaceCar);
            }
        }

        public bool UnderPaceCar
        {
            get
            {
                return this.CarIdxTrackSurface[0] == TrackLocation.OnTrack;
            }
        }
    }
}
