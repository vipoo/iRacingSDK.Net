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
using System.Threading.Tasks;

namespace iRacingSDK
{
    public struct LapSector
    {
        public readonly int LapNumber;
        public readonly int Sector;

        public LapSector(int lapNumber, int sector)
        {
            Sector = sector;
            LapNumber = lapNumber;
        }

        public static LapSector ForLap(int lapNumber)
        {
            return new LapSector(lapNumber, 0);
        }

        public override bool Equals(Object obj)
        {
            return obj is LapSector && this == (LapSector)obj;
        }

        public override int GetHashCode()
        {
            return LapNumber << 4 + Sector;
        }
        
        public static bool operator ==(LapSector x, LapSector y)
        {
            return x.LapNumber == y.LapNumber && x.Sector == y.Sector;
        }

        public static bool operator !=(LapSector x, LapSector y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return string.Format("Lap: {0}, Sector: {1}", LapNumber, Sector);
        }
    }

	public partial class Telemetry : Dictionary<string, object>
    {
        public LapSector RaceLapSector
        {
            get
            {
                var firstSector = this.CarIdxLap
                    .Select((lap, idx) => new { Lap = lap, Idx = idx, Pct = this.CarIdxLapDistPct[idx] })
                    .Where(l => l.Lap == this.RaceLaps)
                    .OrderByDescending(l => l.Pct)
                    .First();

                return new LapSector(this.RaceLaps, ToSectorFromPercentage(firstSector.Pct));
            }
        }

        static int ToSectorFromPercentage(float percentage)
        {
            if (percentage > 0.66)
                return 2;
            
            else if (percentage > 0.33)
                return 1;

            return 0;
        }

        LapSector[] carSectorIdx;
        public LapSector[] CarSectorIdx //0 -> Start/Finish, 1 -> 33%, 2-> 66%
        {
            get
            {
                if (carSectorIdx != null)
                    return carSectorIdx;

                carSectorIdx = new LapSector[64];
                for(int i = 0; i < 64; i++)
                    carSectorIdx[i] = new LapSector(this.CarIdxLap[i], ToSectorFromPercentage(CarIdxLapDistPct[i]));

                return carSectorIdx;
            }
        }
    }
}
