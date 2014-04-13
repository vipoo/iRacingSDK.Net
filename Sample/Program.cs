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
using iRacingSDK;
using YamlDotNet.RepresentationModel;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace SpikeIRSDK
{
	struct DriverTable
	{
		public int Index;
		public int Lap;
		public float Percentage;
	}

	class MainClass
	{
		public unsafe static void Main(string[] args)
		{
			var iRacing = new DataFeed();
			if(!iRacing.Connect())
				throw new Exception("Unable to connect to iRacing server");

			iRacing.OnSessionInfo( s => Console.WriteLine("New session data "));


			foreach(var data in iRacing.Feed)
			{
				var numberOfDrivers = iRacing.SessionInfo.DriverInfo.Drivers.Length;
				var positions = new DriverTable[numberOfDrivers];
				for(int i = 0; i < numberOfDrivers; i++)
				{
					positions[i].Index = i;
					positions[i].Lap = data.Telementary.CarIdxLap[i];
					positions[i].Percentage = data.Telementary.CarIdxLapDistPct[i];

				}

				Console.WriteLine(); //
				Console.WriteLine("Tick, Session Time: " + data.Telementary["TickCount"] + ", " + data.Telementary["SessionTime"]);
			}
		}

		public class IRacingSessionInfo
		{
		}

		static void NewMethod(DataFeed iRacing)
		{
			var sessionInfo = iRacing.SessionInfo;

			Console.WriteLine(sessionInfo);

			/*

			var mapping = (YamlMappingNode)sessionInfo.Documents[0].RootNode;
			// List all the items

			dynamic spando = new ExpandoObject();
			WriteElements(mapping, (IDictionary<string, Object>)spando);
			var d = (IDictionary<string, Object>)spando.DriverInfo;


			var drivers = (object[])spando.DriverInfo.Drivers;
			foreach(var dr in (dynamic)drivers)
			{
				foreach( var kv in dr)
					Console.WriteLine(kv.Key + "," + kv.Value);

				//Console.WriteLine(dr);
			}*/
		}

		static void WriteElements(YamlMappingNode node, IDictionary<string, Object> spando)
		{
			foreach(var s in node.Children)
			{
				if(s.Value.GetType() == typeof(YamlMappingNode))
				{
					var spando2 = new ExpandoObject();
					WriteElements((YamlMappingNode)s.Value, spando2);
					spando.Add(s.Key.ToString(), spando2);
				}
				else
				{
					if(s.Value.GetType() == typeof(YamlScalarNode))
					{
						spando.Add(s.Key.ToString(), s.Value.ToString());
					}
					else
					{
						if(s.Value.GetType() == typeof(YamlSequenceNode))
						{
							var arry = new List<object>();
							foreach(var x in (YamlSequenceNode)s.Value)
							{
								var spando3 = new ExpandoObject();
								WriteElements((YamlMappingNode)x, spando3);
								arry.Add(spando3);
							}
							spando.Add(s.Key.ToString(), arry.ToArray());
						} else
							Console.WriteLine(s.Value.GetType());
					}
				}
			}
		}

	}

	public class DynamicYamlReader
	{

	}
}
