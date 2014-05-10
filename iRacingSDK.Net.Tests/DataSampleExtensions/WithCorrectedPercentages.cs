using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace iRacingSDK.Net.Tests
{
	[TestFixture]
	public class WithCorrectedPercentages
	{
		static DataSample[] CreatesSamples(int[] laps, float[] distPcts)
		{
			var s = new SessionData { DriverInfo = new SessionData._DriverInfo { Drivers = new [] { new SessionData._DriverInfo._Drivers { UserName = "Test" } } } };

			var result = new List<DataSample>();

			for(int i = 0; i < laps.Length; i++)
				result.Add(new DataSample {
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

		[Test]
		public void Corrects_samples_that_are_less_than_previous_samples()
		{
			var samples = CreatesSamples(new [] { 1, 2 }, new [] { 0.95f, 0.95f });

			var correctedSamples = samples.WithCorrectedPercentages().ToArray();

			Assert.That( correctedSamples.Last().Telemetry.CarIdxLapDistPct[0], Is.EqualTo(0f));
		}

		[Test]
		public void Correct_samples_until_we_get_back_to_low_percentages()
		{
			var samples = CreatesSamples(
				laps: new [] { 1, 2, 2, 2 },
				distPcts: new [] { 0.95f, 0.95f , 0.98f, 0.23f}
			);

			var correctedSamples = samples.WithCorrectedPercentages().ToArray();

			var actual = correctedSamples.Select(s => s.Telemetry.CarIdxLapDistPct[0]).ToArray();
			Assert.That( actual, Is.EqualTo(new [] {0.95f, 0f, 0f, 0.23f}));
		}
	}
}
