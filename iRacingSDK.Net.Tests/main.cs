using System;
using System.IO;
using YamlDotNet.Serialization;

namespace iRacingSDK.Net.Tests
{
    public class main
    {
        public static void Main()
        {
            new WithCorrectedPercentages().should_correct_samples_until_we_get_back_to_low_percentages();

            var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
            
            var yaml = File.ReadAllText(@"C:\Users\dean\Downloads\data.yaml");

            yaml = yaml.Replace(": *", ": ");

            var input = new StringReader(yaml);

            var result = (iRacingSDK.SessionData)deserializer.Deserialize(input, typeof(iRacingSDK.SessionData));
        }
    }
}

