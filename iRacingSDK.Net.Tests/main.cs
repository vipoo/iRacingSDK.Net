using System;
using System.IO;

namespace iRacingSDK.Net.Tests
{
    public class main
    {
        public static void Main()
        {
            new WithPitStopCounts().Does_not_double_count_when_car_leaves_world();

           /* var deserializer = new YamlDotNet.Serialization.Deserializer(ignoreUnmatched: true);

            var yaml = File.ReadAllText(@"C:\Users\dean\Downloads\data.yaml");

            yaml = yaml.Replace(": *", ": ");

            var input = new StringReader(yaml);

            var result = (iRacingSDK.SessionData)deserializer.Deserialize(input, typeof(iRacingSDK.SessionData));
        */}
    }
}

