using System.Web.Mvc;
 
[assembly: WebActivator.PreApplicationStartMethod(typeof(NerdDinner.UI.MobileViewEngines), "Start")]
namespace NerdDinner.UI 
{
    public static class MobileViewEngines
    {
        public static void Start()
        {
            ViewEngines.Engines.Insert(0, new MobileCapableRazorViewEngine());
        }
    }
}