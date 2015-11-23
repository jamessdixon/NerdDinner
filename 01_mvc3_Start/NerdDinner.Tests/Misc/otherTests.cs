using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;

namespace NerdDinner.Tests.Misc
{
    [TestClass]
    public class otherTests
    {
        [TestMethod]
        public void IsCorrectVersion()
        {

            var expected = "3.2";
            var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().ToList();
            var nerdDinner = assemblies.Where(x => x.Name.ToLower() == "nerddinner").First();

            var actual = string.Format("{0}.{1}", nerdDinner.Version.Major, nerdDinner.Version.Minor);

            Assert.AreEqual(expected,actual);

        }
    }
}
