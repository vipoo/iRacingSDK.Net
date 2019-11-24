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
        private List<DataSample> samples;

        IncidentsSupport BuildSamples()
        {
            samples = new List<DataSample>();

            return this;
        }

        List<DataSample> Samples()
        {
            return samples;
        }

        IncidentsSupport OnACar(int countOfSamples = 1)
        {
            int frameNumber = samples.Count;
            for ( var i = 0; i < countOfSamples; i++)
                samples.Add(new DataSample
                {
                    IsConnected = true,
                    Telemetry = new Telemetry
                        {
                            { "CamCarIdx", 1 },
                            { "ReplayFrameNum", frameNumber }
                        }
                });

            return this;
        }
        IncidentsSupport OnAPaceCar(int countOfSamples = 1)
        {
            int frameNumber = samples.Count;
            for (var i = 0; i < countOfSamples; i++)
                samples.Add(new DataSample
                {
                    IsConnected = true,
                    Telemetry = new Telemetry
                        {
                            { "CamCarIdx", 0 },
                            { "ReplayFrameNum", frameNumber }
                        }
                });

            return this;
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
            var inputSamples = BuildSamples().OnACar().OnAPaceCar().OnACar(3).Samples();

            var samples = iRacingSDK.IncidentsSupport.FindIncidents(inputSamples, 3).ToList();

            Assert.That(FrameNumbersFromSamples(samples), Is.EqualTo(new[] { 2 }));

        }

        [Test]
        public void should_find_two_incidents()
        {
            var inputSamples = BuildSamples().OnACar().OnAPaceCar().OnACar(4).OnAPaceCar().OnACar(4).Samples();

            var samples = iRacingSDK.IncidentsSupport.FindIncidents(inputSamples, 3);

            Assert.That(FrameNumbersFromSamples(samples), Is.EqualTo(new[] { 2, 7 }));
        }
        
        [Test]
        public void should_stop_if_frame_number_does_not_advance()
        {
            var inputSamples = BuildSamples().OnACar().OnAPaceCar().OnACar(4).OnAPaceCar(10).Samples();

            var samples = iRacingSDK.IncidentsSupport.FindIncidents(inputSamples, 3);

            Assert.That(FrameNumbersFromSamples(samples), Is.EqualTo(new[] { 2 }));
        }
    }
}
