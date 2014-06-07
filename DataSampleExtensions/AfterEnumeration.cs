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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace iRacingSDK
{
    public class AfterEnumeration
    {
        readonly IEnumerable<DataSample> samples;
        readonly TimeSpan period;

        public AfterEnumeration(IEnumerable<DataSample> samples, TimeSpan period)
        {
            this.samples = samples;
            this.period = period;
        }

        public IEnumerable<DataSample> After(Func<DataSample, bool> condition)
        {
            bool conditionMet = false;
            TimeSpan conditionMetAt = new TimeSpan();

            foreach( var data in samples)
            {
                if(!conditionMet && condition(data))
                {
                    conditionMet = true;
                    conditionMetAt = data.Telemetry.SessionTimeSpan;
                }

                if (conditionMet && conditionMetAt + period < data.Telemetry.SessionTimeSpan)
                    break;

                yield return data;
            }
        }

        public IEnumerable<DataSample> AfterReplayPaused()
        {
            var timeoutAt = DateTime.Now + period;
            var lastFrameNumber = -1;

            foreach (var data in samples)
            {
                if (lastFrameNumber == data.Telemetry.ReplayFrameNum)
                {
                    if (timeoutAt < DateTime.Now)
                    {
                        Trace.WriteLine(string.Format("{0} Replay paused for {1}.  Assuming end of replay", data.Telemetry.SessionTimeSpan, period), "INFO");
                        break;
                    }
                }
                else
                {
                    timeoutAt = DateTime.Now + period;
                    lastFrameNumber = data.Telemetry.ReplayFrameNum;
                }

                yield return data;
            }
        }
    }
}
