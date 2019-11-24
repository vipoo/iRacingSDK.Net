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
	public class WithCorrectedDistances
	{
		static DataSample[] CreatesSamples(params float[] values)
		{
			var s = new SessionData { DriverInfo = new SessionData._DriverInfo { Drivers = new [] { new SessionData._DriverInfo._Drivers { UserName = "Test", CarNumberRaw = 1 } } } };

			var sample1 = new DataSample 
			{
                IsConnected = true,
				SessionData = s,
				Telemetry = new Telemetry {	
					{ "CarIdxLap",	new[] { 1 }	}, 
					{ "CarIdxLapDistPct", new[] { values[0] } }
				}
			};


			var sample2 = new DataSample 
			{
                IsConnected = true,
				SessionData = s,
				Telemetry = new Telemetry {	
					{ "CarIdxLap",	new[] { 1 }	}, 
					{ "CarIdxLapDistPct", new[] { values[1] } }
				}
			};

			return new[] { sample1,	sample2	};
		}

		[Test]
		public void Corrects_samples_that_are_less_than_previous_samples()
		{
			var samples = CreatesSamples(0.3f, 0.2f);

			var correctedSamples = samples.WithCorrectedDistances().ToArray();

			Assert.That( correctedSamples.Last().Telemetry.CarIdxLapDistPct[0], Is.EqualTo(0.3f).Within(0.0001));
		}

		[Test]
		public void Only_corrects_to_within_4_decimal_places()
		{
			var samples = CreatesSamples(0.300544f, 0.300522f);

			var correctedSamples = samples.WithCorrectedDistances().ToArray();

			Assert.That( correctedSamples.Last().Telemetry.CarIdxLapDistPct[0], Is.EqualTo(0.300522f));
		}
	}
}
