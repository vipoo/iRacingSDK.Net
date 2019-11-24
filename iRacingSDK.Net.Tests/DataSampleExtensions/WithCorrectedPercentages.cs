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
    public class WithCorrectedPercentages
    {
        static DataSample[] CreatesSamplesForDistancesOf(params float[] distances)
        {
            var laps = distances.Select(d => (int)d).ToArray();
            var distPcts = distances.Select(d => d - (float)(int)d).ToArray();

            var s = new SessionData { DriverInfo = new SessionData._DriverInfo { Drivers = new[] { new SessionData._DriverInfo._Drivers { UserName = "Test", CarNumberRaw = 1 } } } };

            var result = new List<DataSample>();

            for (int i = 0; i < laps.Length; i++)
                result.Add(new DataSample
                {
                    IsConnected = true,
                    SessionData = s,
                    Telemetry = new Telemetry {	
                        { "CarIdxLap",	new[] { laps[i] }	}, 
                        { "CarIdxLapDistPct", new[] { distPcts[i] } },
                        { "CarIdxTrackSurface", new[] { TrackLocation.OnTrack } },
                        { "ReplayFrameNum", 10 + i }
                    }
                });

            return result.ToArray();
        }

        private float[] GetDistancesFromSamples(DataSample[] correctedSamples)
        {
            return correctedSamples.Select(s => s.Telemetry.CarIdxLap[0] + s.Telemetry.CarIdxLapDistPct[0]).ToArray();
        }

        [Test]
        public void should_correct_samples_until_we_get_back_to_low_percentages()
        {
            var expected = new[] { 4.5f, 4.95f, 5.0f, 5.0f, 5.23f };
            var samples = CreatesSamplesForDistancesOf(4.5f, 4.95f, 5.95f, 5.98f, 5.23f);

            var correctedSamples = samples.WithCorrectedPercentages().ToArray();

            var actual = GetDistancesFromSamples(correctedSamples);
            Assert.That(actual, Is.EqualTo(expected).Within(0.001f));
        }

        [Test]
        public void should_correct_for_pre_race_starting_percentages()
        {
            var expected = new[] { 0.95f, 1.0f, 1.02f, 1.05f };
            var samples = CreatesSamplesForDistancesOf(0.95f, 1.95f, 1.02f, 1.05f);

            var correctedSamples = samples.WithCorrectedPercentages().ToArray();

            var actual = GetDistancesFromSamples(correctedSamples);
            Assert.That(actual, Is.EqualTo(expected).Within(0.01f));
        }
    }
}
