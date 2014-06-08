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

namespace iRacingSDK
{
    public partial class SessionData
    {
        public string Raw { get; /*internal*/ set; }

        public partial class _SessionInfo
        {
            public partial class _Sessions
            {
                public bool IsRace
                {
                    get
                    {
                        return this.SessionType.ToLower().Contains("race");
                    }
                }
            }
        }

        public partial class _DriverInfo
        {
            public partial class _Drivers
            {
                public bool IsPaceCar { get { return this.CarIdx == 0; } }
            }
        }
    }

    public static class _SessionExtensions
    {
        public static SessionData._SessionInfo._Sessions Qualifying(this SessionData._SessionInfo._Sessions[] sessions)
        {
            return sessions.FirstOrDefault(s => s.SessionType.ToLower().Contains("qualif"));
        }
    }
}
