using System;
using System.IO;

namespace iRacingSDK.Net.Tests
{
    public class main
    {
        public static void Main()
        {
            //new WithPitStopCounts().Increment_pit_stop_count_when_driver_enters_pit_road();

            var deserializer = new YamlDotNet.Serialization.Deserializer(ignoreUnmatched: true);

            var yaml = File.ReadAllText(@"C:\Users\dean\Downloads\data.yaml");

            yaml = yaml.Replace(": *", ": ");

            var input = new StringReader(yaml);

            var result = (iRacingSDK.SessionData)deserializer.Deserialize(input, typeof(iRacingSDK.SessionData));
        }
    }
}

