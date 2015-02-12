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

using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace iRacingSDK.Net.Tests
{
    [TestFixture]
    public class VerifyReplayFrame
    {
        DataSample[] CreateSamplesFromFrameNumbers(SessionData s, params int[] frameNumbers)
        {
            DataSample lastSample = null;

            return frameNumbers.Select(n =>
                {
                    var sample = new DataSample
                    {
                        IsConnected = true,
                        Telemetry = new Telemetry
                        {
                            { "SessionNum", 0 },
                            { "ReplayFrameNum", n }
                        },
                        LastSample = lastSample
                    };

                    sample.Telemetry.SessionData = s;
                    lastSample = sample;
                    return sample;
                }
            ).ToArray();
        }

        int[] FrameNumbersFromSamples(IEnumerable<DataSample> samples)
        {
            return samples.Select(data => data.Telemetry.ReplayFrameNum).ToArray();
        }

        [Test]
        public void it_skips_a_single_zero_frame()
        {
            var s = new SessionData { SessionInfo = new SessionData._SessionInfo { Sessions = new[] { new SessionData._SessionInfo._Sessions() } } };

            var inputSamples = CreateSamplesFromFrameNumbers(s, 1, 2, 3, 0, 4, 5);

            var samples = iRacingSDK.DataSampleExtensions.VerifyReplayFrames(inputSamples).ToList();

            Assert.That(FrameNumbersFromSamples(samples), Is.EqualTo(new [] {1, 2, 3, 4, 5 }));
        }

        [Test]
        public void it_skips_a_two_zero_frame()
        {
            var s = new SessionData { SessionInfo = new SessionData._SessionInfo { Sessions = new[] { new SessionData._SessionInfo._Sessions() } } };

            var inputSamples = CreateSamplesFromFrameNumbers(s, 1, 2, 3, 0, 0, 4, 5);

            var samples = iRacingSDK.DataSampleExtensions.VerifyReplayFrames(inputSamples).ToList();

            Assert.That(FrameNumbersFromSamples(samples), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void it_skips_if_no_matching_session()
        {
            var s = new SessionData { SessionInfo = new SessionData._SessionInfo { Sessions = new[] { new SessionData._SessionInfo._Sessions() } } };

            var inputSamples = CreateSamplesFromFrameNumbers(s, 1, 2, 3, 4, 5);

            inputSamples.First().Telemetry["SessionNum"] = 1;
               
            var samples = iRacingSDK.DataSampleExtensions.VerifyReplayFrames(inputSamples).ToList();

            Assert.That(FrameNumbersFromSamples(samples), Is.EqualTo(new [] { 2, 3, 4, 5 }));
        }
    }
}
