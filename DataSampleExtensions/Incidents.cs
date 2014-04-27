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
using System.Threading;
using System.Threading.Tasks;

namespace iRacingSDK
{
    public static partial class DataSampleExtensions
    {
        public static IEnumerable<DataSample> RaceIncidents(this IEnumerable<DataSample> samples)
        {
            iRacing.Replay.MoveToStartOfRace();

            samples.AtSpeed(8).First(d => d.Telemetry.SessionState == SessionState.Racing);

            iRacing.Replay.SetSpeed(0);
            iRacing.Replay.Wait();

            DataSample data = null;
            while( data == null || data.Telemetry.SessionState != SessionState.CoolDown)
            {
                iRacing.Replay.MoveToNextIncident();
                data = samples.First();

                yield return data;
            }
        }
    }
}

