using NerdDinner.UI.Controllers;

namespace NerdDinner.Tests.Mocks
{
    public class MockFormsAuthenticationService : IFormsAuthentication
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
        }

        public void SignOut()
        {
        }
    }
}
