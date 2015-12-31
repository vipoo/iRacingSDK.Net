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
    public class IncidentsSupport
    {
        DataSample[] CreateSamplesFromFrameNumbers(params int[] frameNumbers)
        {
            return frameNumbers.Select(n =>
                new DataSample
                {
                    IsConnected = true,
                    Telemetry = new Telemetry
                    {
                        { "CamCarIdx", 1 },
                        { "ReplayFrameNum", n }
                    }
                }).ToArray();
        }

        int[] FrameNumbersFromSamples(IEnumerable<DataSample> samples)
        {
            return samples.Select(data => data.Telemetry.ReplayFrameNum).ToArray();
        }

        //Simulate a series of samples, representing a paused replay
        //that jumps to another incident's frame number 

        [Test]
        public void should_find_single_incidents()
        {
            var start = 10;
            var incidentFrame = 30;

            var frameNumbers = new List<int>();
            for (var i = 0; i < 2; i++)
                frameNumbers.Add(start);
            for (var i = 0; i < 100; i++)
                frameNumbers.Add(incidentFrame);

            var inputSamples = CreateSamplesFromFrameNumbers(frameNumbers.ToArray());

            var receivedSamples = new List<DataSample>();

            var samples = iRacingSDK.IncidentsSupport.FindIncidents(inputSamples, d => { receivedSamples.Add(d); }, 100).ToList();

            Assert.That(FrameNumbersFromSamples(samples), Is.EqualTo(new[] { incidentFrame }));

            Assert.That(FrameNumbersFromSamples(receivedSamples), Is.EqualTo(new[] { start, incidentFrame }));
        }

        [Test]
        public void should_find_two_incidents()
        {
            var start = 10;
            var incidentFrame1 = 30;
            var incidentFrame2 = 60;

            var frameNumbers = new List<int>();
            for (var i = 0; i < 2; i++)
                frameNumbers.Add(start);
            for (var i = 0; i < 101; i++)
                frameNumbers.Add(incidentFrame1);
            for (var i = 0; i < 201; i++)
                frameNumbers.Add(incidentFrame2);


            var inputSamples = CreateSamplesFromFrameNumbers(frameNumbers.ToArray());

            var samples = iRacingSDK.IncidentsSupport.FindIncidents(inputSamples, d => { }, 100);

            Assert.That(FrameNumbersFromSamples(samples), Is.EqualTo(new[] { incidentFrame1, incidentFrame2 }));
        }

        [Test]
        public void should_stop_if_frame_number_does_not_advance()
        {
            var start = 10;
            var incidentFrame = 30;
            var neverReached = 1000;

            var frameNumbers = new List<int>();
            frameNumbers.Add(start);
            for (int i = 0; i < 110; i++)
                frameNumbers.Add(incidentFrame);
            frameNumbers.Add(neverReached);

            var inputSamples = CreateSamplesFromFrameNumbers(frameNumbers.ToArray());

            var receivedSamples = new List<DataSample>();

            var samples = iRacingSDK.IncidentsSupport.FindIncidents(inputSamples, (d) => { receivedSamples.Add(d); }, 100);

            Assert.That(FrameNumbersFromSamples(samples), Is.EqualTo(new[] { incidentFrame }));

            Assert.That(FrameNumbersFromSamples(receivedSamples), Is.EqualTo(new[] { start, incidentFrame }));
        }
    }
}
