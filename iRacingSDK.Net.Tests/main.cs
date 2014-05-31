using System;

namespace iRacingSDK.Net.Tests
{
	public class main
	{
		public static void Main()
		{
            new TakeUntilEndOfReplay().should_stop_enumerating_when_frame_number_does_not_advance();
		}
	}
}

