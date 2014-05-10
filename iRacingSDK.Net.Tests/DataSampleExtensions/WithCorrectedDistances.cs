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
			var s = new SessionData { DriverInfo = new SessionData._DriverInfo { Drivers = new [] { new SessionData._DriverInfo._Drivers { UserName = "Test" } } } };

			var sample1 = new DataSample 
			{
				SessionData = s,
				Telemetry = new Telemetry {	
					{ "CarIdxLap",	new[] { 1 }	}, 
					{ "CarIdxLapDistPct", new[] { values[0] } }
				}
			};


			var sample2 = new DataSample 
			{
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
