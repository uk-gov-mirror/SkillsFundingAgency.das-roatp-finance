using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.RoatpFinance.Web.Controllers;

namespace SFA.DAS.RoatpFinance.Web.Tests.Controllers.Ping
{
    [TestFixture]
    public class PingControllerTests
    {

        private PingController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new PingController()
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        [Test]
        public void Ping_returns_Pong()
        {
            var expectedResponse = "Pong";

            var result = _controller.Ping() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(expectedResponse));
        }
    }
}
