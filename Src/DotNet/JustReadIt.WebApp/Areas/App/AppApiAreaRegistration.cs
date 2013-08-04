using System.Web.Http;
using System.Web.Mvc;
using JustReadIt.WebApp.Core.MvcEx;

namespace JustReadIt.WebApp.Areas.App {

  public class AppApiAreaRegistration : AreaRegistration {

    public override string AreaName {
      get { return "App"; }
    }

    public override void RegisterArea(AreaRegistrationContext context) {
      context.MapHttpRoute(
        name: Routes.Default,
        routeTemplate: RouteTemplatePrefix + "{controller}/{id}",
        defaults: new { id = RouteParameter.Optional });
    }

    private string RouteTemplatePrefix {
      get { return string.Format("{0}/api/", AreaName); }
    }

  }

}
