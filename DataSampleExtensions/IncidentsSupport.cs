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
        /// <param name="sampleScanSettle">The number of samples to take, which must all have the same frame number - to identify that iRacing has paused.</param>
        /// <param name="maxTotalIncidents">an optional hard limit on the total number of incidents that can be returned.</param>
        /// <returns>DataSamples of each incident</returns>
        public static List<DataSample> FindIncidents(IEnumerable<DataSample> samples, Action<DataSample> toNext, int sampleScanSettle, int maxTotalIncidents = int.MaxValue)
        {
            if (maxTotalIncidents <= 0)
                return new List<DataSample>();

            iRacing.Replay.SetSpeed(0);
            iRacing.Replay.Wait();

            return samples.PositionsOf(toNext, sampleScanSettle).Take(maxTotalIncidents).ToList();
        }

        /// <summary>
        /// Assumes the game is in paused mode
        /// Filters the samples to just single frame numbers, that the game moves after invoking the moveReplay action (eg: which sends message to the game - MoveToNextIncident)
        /// If the game does not advance to another frame within about 1second, it will stop enumerating
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="moveReplay"></param>
        /// <param name="sampleScanSettle">The number of samples to take, which must all have the same frame number - to identify that iRacing has paused.</param>
        /// <returns></returns>
        static IEnumerable<DataSample> PositionsOf(this IEnumerable<DataSample> samples, Action<DataSample> moveReplay, int sampleScanSettle)
        {
            var lastSamples = new List<int>();
            var jumpMessageSent = false;
            var lastFrameNumber = -2;

            foreach(var data in samples)
            {
                if(!jumpMessageSent)
                {
                    iRacing.Replay.CameraOnDriver(0, 0); //Pace Car
                    moveReplay(data);
                    Thread.Sleep(100);
                    jumpMessageSent = true;
                    continue;
                }

                lastSamples.Add(data.Telemetry.ReplayFrameNum);

                if (lastSamples.Count == sampleScanSettle)
                    lastSamples.RemoveAt(0);

                if(lastSamples.Count == (sampleScanSettle-1) && lastSamples.All(f => f == data.Telemetry.ReplayFrameNum))
                {
                    if (data.Telemetry.ReplayFrameNum == lastFrameNumber)
                    {
                        TraceDebug.WriteLine("Incidents: Frame number did not change - asuming no more incidents.  Current Frame: {0}", data.Telemetry.ReplayFrameNum);
                        break;
                    }

                    if ( data.Telemetry.CamCarIdx == 0) //Pace Car
                    {
                        TraceWarning.WriteLine("Incident scan aborted - iRacing has not progressed to incident.  Frame Number: {0}", data.Telemetry.ReplayFrameNum);
                        break;
                    }

                    lastFrameNumber = data.Telemetry.ReplayFrameNum;
                    TraceDebug.WriteLine("Incidents: last {0} samples have settled on frame number {1}", sampleScanSettle, data.Telemetry.ReplayFrameNum);
                    yield return data;
                    jumpMessageSent = false;
                    lastSamples.Add(-1);
                    lastSamples.RemoveAt(0);
                }
            }
        }
    }
}
