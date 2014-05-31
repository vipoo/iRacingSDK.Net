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
using iRacingSDK;
using NUnit.Framework;
using System.Collections.Generic;

namespace iRacingSDK.Net.Tests
{
	[TestFixture]
	public class TakeUntilEndOfReplay
	{
        DataSample[] CreateSamplesFromFrameNumbers(params int[] frameNumbers)
        {
            return frameNumbers.Select(n =>
                new DataSample
                {
                    IsConnected = true,
                    Telemetry = new Telemetry
                    {
                        { "ReplayFrameNum", n }
                    }
                }).ToArray();
        }

        int[] FrameNumbersFromSamples(IEnumerable<DataSample> samples)
        {
            return samples.Select(data => data.Telemetry.ReplayFrameNum).ToArray();
        }

		[Test]
		public void should_stop_enumerating_when_frame_number_does_not_advance()
		{
            var start = 28;
            var stopsAt = 30;
            var neverReached = 1000;

            var frameNumbers = new List<int>();
            for (var i = start; i < stopsAt; i++)
                frameNumbers.Add(i);
            
            for (int i = 0; i < 200; i++)
                frameNumbers.Add(stopsAt); //After 100 samples of the same framenumber we should have stop

            frameNumbers.Add(neverReached);

            var inputSamples = CreateSamplesFromFrameNumbers(frameNumbers.ToArray());

            var samples = inputSamples.TakeUntilEndOfReplay().ToArray();

            var expectedFrameNumbers = new List<int>();
            for (var i = start; i < stopsAt; i++)
                expectedFrameNumbers.Add(i);

            for (int i = 0; i < 101; i++)
                expectedFrameNumbers.Add(stopsAt);

            Assert.That(FrameNumbersFromSamples(samples), Is.EqualTo(expectedFrameNumbers.ToArray()));
        }
	}
}
