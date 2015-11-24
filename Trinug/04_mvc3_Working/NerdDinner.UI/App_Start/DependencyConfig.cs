using System.Web.Mvc;
using Microsoft.Practices.Unity;
using NerdDinner.UI.Helpers;
using NerdDinner.UI.Models;
using NerdDinner.Models;

namespace NerdDinner.UI
{
    public class DependencyConfig
    {
        public static void RegisterDependencyInjection()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IDinnerRepository, DinnerRepository>();
            container.RegisterType<INerdDinners, NerdDinners>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}