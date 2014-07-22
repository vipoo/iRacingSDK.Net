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

using iRacingSDK.Support;
using System;
using System.Diagnostics;
using System.Linq;

namespace iRacingSDK
{
    public class Car
    {
        readonly int carIdx;
        readonly Telemetry telemetry;
        readonly SessionData._DriverInfo._Drivers driver;
      
        public Car(Telemetry telemetry, int carIdx)
        {
            this.telemetry = telemetry;
            this.carIdx = carIdx;
            this.driver = telemetry.SessionData.DriverInfo.Drivers[carIdx];
        }

        public int Index { get { return carIdx; } }
        public int CarIdx { get { return carIdx; } }

        public int Lap { get { return telemetry.CarIdxLap[carIdx]; } }
        public float DistancePercentage { get { return telemetry.CarIdxLapDistPct[carIdx]; } }
        public float TotalDistance { get { return this.Lap + this.DistancePercentage; } }
        public SessionData._DriverInfo._Drivers Driver { get { return driver; } }
        public LapSector LapSector { get { return telemetry.CarSectorIdx[carIdx]; } }
        public int Position { get { return telemetry.Positions[carIdx]; } }
        public short CarNumber { get { return (short)driver.CarNumber; } }
        public string UserName { get { return driver.UserName; } }
        public bool HasSeenCheckeredFlag { get { return telemetry.HasSeenCheckeredFlag[carIdx]; } }
        public bool IsPaceCar { get { return carIdx == 0; } }
        public bool HasData { get { return telemetry.HasData(carIdx); } }
        public bool HasRetired { get { return telemetry.HasRetired[carIdx]; } }
        public TrackLocation TrackSurface { get { return telemetry.CarIdxTrackSurface[carIdx]; } }

        public SessionData._SessionInfo._Sessions._ResultsPositions ResultPosition
        {
            get
            {
                if (telemetry.Session.ResultsPositions == null)
                    return null;

                return telemetry.Session.ResultsPositions.FirstOrDefault(rp => rp.CarIdx == carIdx);
            }
        }

        public TimeSpan LastTimeSpan
        {
            get { return LastTime.Seconds(); }
        }

        public double LastTime
        {
            get
            {
                var rp = ResultPosition;
                if (rp == null)
                    return 0f;

                if( rp.LapsComplete != (Lap-1))
                {
                    Trace.WriteLine("Attempt to get LastTime from session data, with mismatch Lap counters.  Telemerty Lap: {0}.  Session LapComplete: {1}".F(Lap-1, rp.LapsComplete), "INFO");
                    return 0f;
                }

                return rp.LastTime;
            }
        }
    }
}
