﻿using NerdDinner.UI.Controllers;
using System.Web.Mvc;
using NerdDinner.Tests.Fakes;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NerdDinner.Tests.Controllers
{
    [TestClass]
    public class RSVPControllerTest {

        RSVPController CreateRSVPController() {
            var testData = FakeDinnerData.CreateTestDinners();
            var repository = new FakeDinnerRepository(testData);

            return new RSVPController(repository);
        }

        RSVPController CreateRSVPControllerAs(string userName)
        {

            var mock = new Mock<ControllerContext>();
            var nerdIdentity = FakeIdentity.CreateIdentity("SomeUser");
            mock.SetupGet(p => p.HttpContext.User.Identity).Returns(nerdIdentity);

            var controller = CreateRSVPController();
            controller.ControllerContext = mock.Object;

            return controller;
        }

        [TestMethod]
        public void RegisterAction_Should_Return_Content()
        {
            // Arrange
            var controller = CreateRSVPControllerAs("scottha");

            // Act
            var result = controller.Register(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ContentResult));
        }

        [TestMethod]
        public void CancelAction_Should_Return_Content()
        {
            // Arrange
            var controller = CreateRSVPControllerAs("SomeUser");

            // Act
            var result = controller.Cancel(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ContentResult));
        }
    }
}
