using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NerdDinner.Providers;

namespace NerdDinner.Tests.Providers
{
    [TestClass]
    public class AttendancePredictorTests
    {
        [TestMethod]
        public void GetProjectedAttendanceUsingvalidValues_ReturnsExpected()
        {
            string uri = "https://ussouthcentral.services.azureml.net/workspaces/8d32705e228247c7b2f14301c2158a99/services/ab3279d700b246afb16c5ba6ce4cbf04/execute?api-version=2.0&details=true";
            string apiKey = "IK+rd6ekNSJpJ+5yfdKG3BUJFdJHOt8+OE/pxA40mEewXq4gFceefoJI1E0f/XpqbWCCALD2NDr6JIX+DZC9VA==";
            AttendancePredictor predictor = new AttendancePredictor(apiKey, uri);
            var actual = predictor.GetProjectedAttendance("scottgu", "5");
            var notExpected = 0;
            Assert.AreNotEqual(notExpected, actual);
        }
    }
}
