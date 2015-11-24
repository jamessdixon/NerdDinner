using System.Collections.Generic;
using NerdDinner.UI.Controllers;
using System.Web.Mvc;
using NerdDinner.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NerdDinner.Tests.Controllers
{
    [TestClass]
    public class SearchControllerTest {

        SearchController CreateSearchController() {
            var testData = FakeDinnerData.CreateTestDinners();
            var repository = new FakeDinnerRepository(testData);

            return new SearchController(repository);
        }

        [TestMethod]
        public void SearchByLocationAction_Should_Return_Json()
        {

            // Arrange
            var controller = CreateSearchController();

            // Act
            var result = controller.SearchByLocation(99, -99);

            // Assert
            Assert.IsInstanceOfType(result,typeof(JsonResult));
        }

        [TestMethod]
        public void SearchByLocationAction_Should_Return_JsonDinners()
        {

            // Arrange
            var controller = CreateSearchController();

            // Act
            var result = (JsonResult)controller.SearchByLocation(99, -99);

            // Assert
            Assert.IsInstanceOfType(result.Data, typeof(JsonDinner));
            var dinners = (List<JsonDinner>)result.Data;
            Assert.AreEqual(101, dinners.Count);
        }

        [TestMethod]
        public void GetMostPopularDinnersAction_WithLimit_Returns_Expected_Dinners()
        {

            // Arrange
            var controller = CreateSearchController();

            // Act
            var result = (JsonResult)controller.GetMostPopularDinners(5);

            // Assert
            Assert.IsInstanceOfType(result.Data, typeof(JsonDinner));
            var dinners = (List<JsonDinner>)result.Data;
            Assert.AreEqual(5, dinners.Count);
        }

        [TestMethod]
        public void GetMostPopularDinnersAction_WithNoLimit_Returns_Expected_Dinners()
        {

            // Arrange
            var controller = CreateSearchController();

            // Act
            var result = (JsonResult)controller.GetMostPopularDinners(null);

            // Assert
            Assert.IsInstanceOfType(result.Data, typeof(JsonDinner));
            var dinners = (List<JsonDinner>)result.Data;
            Assert.AreEqual(40, dinners.Count);
        }


    }
}
