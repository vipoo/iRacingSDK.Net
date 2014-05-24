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
using System.Diagnostics;
using System.Threading;

namespace iRacingSDK
{
    public static partial class DataSampleExtensions
    {
        public static IEnumerable<DataSample> RaceIncidents(this IEnumerable<DataSample> samples, int maxTotalIncidents = int.MaxValue)
        {
			var sessionNumber = GetSessionNumber(samples);

            var incidentsOnForward = GetIncidentsForward(samples, maxTotalIncidents);

			var incidentsOnReverse = GetIncidentsReverse(samples, sessionNumber, maxTotalIncidents - incidentsOnForward.Count);
		
			var incidents =  incidentsOnForward
				.Concat(incidentsOnReverse)
				.OrderBy( d => d.Telemetry.ReplayFrameNum)
				.ToList();

			foreach( var incident in incidents)
				Trace.WriteLine(string.Format("Found new incident at frame {0}", incident.Telemetry.SessionTimeSpan), "DEBUG");

			return incidents;
        }

		static int GetSessionNumber(IEnumerable<DataSample> samples)
		{
			var data = samples.First();
			return data.Telemetry.SessionNum;
		}

        static List<DataSample> GetIncidentsForward(IEnumerable<DataSample> samples, int maxTotalIncidents)
		{
            return FindIncidents(samples, iRacing.Replay.MoveToNextIncident, data => data.Telemetry.SessionState == SessionState.CoolDown, maxTotalIncidents);
		}

        static List<DataSample> GetIncidentsReverse(IEnumerable<DataSample> samples, int sessionNumber, int maxTotalIncidents)
		{
            return FindIncidents(samples, iRacing.Replay.MoveToPrevIncident, data => data.Telemetry.SessionNum != sessionNumber || data.Telemetry.RaceLaps <= 0, maxTotalIncidents);
		}

        static List<DataSample> FindIncidents(IEnumerable<DataSample> samples, Action toNext, Func<DataSample, bool> isFinished, int maxTotalIncidents)
		{
			var capturedIncidents = new List<DataSample>();

            if (maxTotalIncidents <= 0)
                return capturedIncidents;

			iRacing.Replay.SetSpeed(0);
			iRacing.Replay.Wait();

			var data = samples.First();

			while(!isFinished(data))
			{
				var frameNumber = data.Telemetry.ReplayFrameNum;
			
				toNext();

				if(!HasMovedToNewFrame(samples, ref data, frameNumber))
					break;

				capturedIncidents.Add(data);

                if (capturedIncidents.Count >= maxTotalIncidents)
                    break;
			}

			return capturedIncidents;
		}

		static bool HasMovedToNewFrame(IEnumerable<DataSample> samples, ref DataSample data, int frameNumber)
		{
			var retryCount = 4;
			while(data.Telemetry.ReplayFrameNum == frameNumber && retryCount >= 0)
			{
				retryCount++;
				Thread.Sleep(600);
				//Wait a bit more to ensure iRacing has moved to the incident
				data = samples.First();
			}
			return retryCount >= 0;
		}
	}
}

