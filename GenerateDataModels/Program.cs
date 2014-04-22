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
using System.IO;
using YamlDotNet.RepresentationModel;

namespace GenerateDataModels
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new TelemetryTemplate();
            File.WriteAllText("GeneratedTelemetry.cs", x.TransformText());

            var y = new SessionInfoTemplate();
            File.WriteAllText(@"GeneratedSessionData.cs", y.TransformText());
        }

        public static string GetTypeFor(string fieldName, object sampleValue)
        {
            return sampleValue.GetType().ToString();
        }
    }
}
