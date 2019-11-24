using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingSDK
{
    [TestFixture]
    public class TelemetryCamCarTests
    {
        SessionData._DriverInfo._Drivers driver;
        Telemetry telemetry;
        
        [SetUp]
        public void setup()
        {
            telemetry = new Telemetry();

            driver = new SessionData._DriverInfo._Drivers { CarNumberRaw = 1 };

            telemetry.SessionData = new SessionData
            {
                DriverInfo = new SessionData._DriverInfo
                {
                    Drivers = new SessionData._DriverInfo._Drivers[] 
                    {
                        driver
                    }
                }
            };
        }

        [Test]
        public void ShouldReturnCarByIndex()
        {
            var c = telemetry.Cars[0];

            Assert.That(c.CarIdx, Is.EqualTo(0));
        }

        [Test]
        public void ShouldRaiseFriendErrorWhenAccessUnknownCarIdx()
        {
            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo("Attempt to load car details for unknown carIndex.  carIdx: 5, maxNumber: 0"), () => { var a = telemetry.Cars[5]; });
        }
    }
}
