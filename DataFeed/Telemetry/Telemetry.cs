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
using System.Text;
using iRacingSDK.Support;

namespace iRacingSDK
{
    public class CarArray : IEnumerable<Car>
    {
        Car[] cars;
        
        public CarArray(Car[] cars)
        {
            this.cars = cars;
        }

        public Car this[long carIdx]
        {
            get
            {
                if (carIdx < 0)
                    throw new Exception("Attempt to load car details for negative car index {0}".F(carIdx));

                if (carIdx >= cars.Length)
                    throw new Exception("Attempt to load car details for unknown carIndex.  carIdx: {0}, maxNumber: {1}".F(carIdx, cars.Length - 1));

                return cars[carIdx];
            }
        }

        public IEnumerator<Car> GetEnumerator()
        {
            return (cars as IEnumerable<Car>).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return cars.GetEnumerator();
        }
    }

    public partial class Telemetry : Dictionary<string, object>
    {
        public SessionData._SessionInfo._Sessions Session { get { return SessionData.SessionInfo.Sessions[SessionNum]; } }

        public Car CamCar { get { return Cars[CamCarIdx]; } }

        CarArray cars;
        public CarArray Cars
        {
            get
            {
                if (cars != null)
                    return cars;

                return cars = new CarArray(
                    Enumerable.Range(0, this.SessionData.DriverInfo.Drivers.Length).Select(i => new Car(this, i)).ToArray());

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

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach( var kv in this)
            {
                result.Append(kv.Key.ToString());
                result.Append(": ");
                result.Append(kv.Value.ToString());
                result.Append("\n");
            }

            return result.ToString();
        }
    }
}
