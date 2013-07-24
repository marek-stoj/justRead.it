using System.Web.Http;
using System.Web.Mvc;

namespace JustReadIt.WebApp.Areas.FeedbinApi {

  public class FeedbinApiAreaRegistration : AreaRegistration {

    private const string _UrlPrefix = "feedbin-api/v2/";

    public override string AreaName {
      get { return "FeedbinApi"; }
    }

    public override void RegisterArea(AreaRegistrationContext context) {
      context.Routes.MapHttpRoute(
        name: "FeedbinApi_Subscriptions_GetAll",
        routeTemplate: _UrlPrefix + "subscriptions.json",
        defaults: new { controller = "Subscriptions", action = "GetAll", });
    }

  }

}
