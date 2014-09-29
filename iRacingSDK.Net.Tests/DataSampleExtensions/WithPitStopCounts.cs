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

namespace iRacingSDK.Net.Tests
{
    [TestFixture]
    public class WithPitStopCounts
    {
        static DataSample[] CreatesSamples(params TrackLocation[] values)
        {
            var s = new SessionData { DriverInfo = new SessionData._DriverInfo { Drivers = new[] { new SessionData._DriverInfo._Drivers { UserName = "Test" } } } };

            var sample1 = new DataSample
            {
                IsConnected = true,
                SessionData = s,
                Telemetry = new Telemetry {
                {
                    "CarIdxTrackSurface",	new[] { values[0] }	}
                }
            };

            var sample2 = new DataSample
            {
                IsConnected = true,
                SessionData = s,
                Telemetry = new Telemetry {	
                    { "CarIdxTrackSurface",	new[] { values[1] }	}
                }
            };

            sample1.Telemetry.SessionData = s;
            sample2.Telemetry.SessionData = s;
            sample2.LastSample = sample1;

            return new[] { sample1, sample2 };
        }

        [Test]
        public void Pitstop_count_should_be_zero_car_has_not_entered_pit_road()
        {
            var samples = CreatesSamples(TrackLocation.OnTrack, TrackLocation.OnTrack);

            var correctedSamples = samples.WithPitStopCounts().ToArray();

            Assert.That(correctedSamples.Last().Telemetry.CarIdxPitStopCount[0], Is.EqualTo(0));
        }

        [Test]
        public void Increment_pit_stop_count_when_driver_enters_pit_road()
        {
            var samples = CreatesSamples(TrackLocation.OnTrack, TrackLocation.InPitStall);

            var correctedSamples = samples.WithPitStopCounts().ToArray();

            Assert.That(correctedSamples.Last().Telemetry.CarIdxPitStopCount[0], Is.EqualTo(1));
        }
    }
}