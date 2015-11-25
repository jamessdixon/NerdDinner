using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NerdDinner.Providers;

namespace NerdDinner.Tests.Providers
{
    [TestClass]
    public class AdProviderTests
    {
        [TestMethod]
        public void GetCatagoryUsingValidValue_ReturnsExpected()
        {
            String fileUri = "Data/nameLookup.csv";
            var userName = "James";
            AdProvider adProvider = new AdProvider(fileUri);
            var actual = adProvider.GetCatagory(userName);
            var expected = "MiddleAged Male";
            Assert.AreEqual(expected, actual);
        }
    }
}
