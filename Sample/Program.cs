using System;
using iRacingSDK;

namespace SpikeIRSDK
{
	class MainClass
	{
		public unsafe static void Main(string[] args)
		{
			var iRacing = new Feed();
			if(!iRacing.Connect())
				throw new Exception("Unable to connect to iRacing server");

			iRacing.OnSessionInfo( s => Console.WriteLine("New session data "));
			var sessionInfo = iRacing.SessionInfo;

			Console.WriteLine(sessionInfo);

			foreach(var data in iRacing.DataFeed)
			{
				Console.WriteLine("Tick, Session Time: " + data["TickCount"] + ", " + data["SessionTime"]);
			}
		}
	}
}
