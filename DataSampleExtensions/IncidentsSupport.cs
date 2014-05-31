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
using System.Threading;

namespace iRacingSDK
{
    public static class IncidentsSupport
    {
        /// <summary>
        /// Pauses the game and then uses the 'toNext' Action to send a message to the game to advance through all incidents
        /// Returns a list of DataSamples for each frame of each incident
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="toNext">A function to that sends ither move to next incident or move to previous incident</param>
        /// <param name="maxTotalIncidents">an optional hard limit on the total number of incidents that can be returned.</param>
        /// <returns>DataSamples of each incident</returns>
        public static List<DataSample> FindIncidents(IEnumerable<DataSample> samples, Action<DataSample> toNext, int maxTotalIncidents = int.MaxValue)
        {
            if (maxTotalIncidents <= 0)
                return new List<DataSample>();

            iRacing.Replay.SetSpeed(0);
            iRacing.Replay.Wait();

            return samples.PositionsOf(toNext).Take(maxTotalIncidents).ToList();
        }

        /// <summary>
        /// Assumes the game is in paused mode
        /// Filters the samples to just single frame numbers, that the game moves after invoking the moveReplay action (eg: which sends message to the game - MoveToNextIncident)
        /// If the game does not advance to another frame within about 1second, it will stop enumerating
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="moveReplay"></param>
        /// <returns></returns>
        static IEnumerable<DataSample> PositionsOf(this IEnumerable<DataSample> samples, Action<DataSample> moveReplay)
        {
            const int MaxRetryAttempt = 100;
            int frameNumber = -1;
            var retryCount = MaxRetryAttempt;

            foreach( var data in samples)
            {
                if (frameNumber != data.Telemetry.ReplayFrameNum && retryCount != MaxRetryAttempt)
                {
                    yield return data;
                    retryCount = MaxRetryAttempt;
                }

                if (retryCount-- == MaxRetryAttempt)
                {
                    frameNumber = data.Telemetry.ReplayFrameNum;
                    moveReplay(data);
                    continue;
                }
             
                if (retryCount < 0)
                    break;
            }
        }
    }
}
