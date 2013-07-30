using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace JustReadIt.WebApp.Areas.FeedbinApi {

  public class FeedbinApiAreaRegistration : AreaRegistration {

    private const string _UrlPrefix = "feedbin-api/v2/";

    public override string AreaName {
      get { return "FeedbinApi"; }
    }

    public override void RegisterArea(AreaRegistrationContext context) {
      context.Routes.MapHttpRoute(
        name: Routes.Subscriptions_GetAll,
        routeTemplate: _UrlPrefix + "subscriptions.json",
        defaults: new { controller = "Subscriptions", action = "GetAll", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.Routes.MapHttpRoute(
        name: Routes.Subscriptions_Get,
        routeTemplate: _UrlPrefix + "subscriptions/{id}.json",
        defaults: new { controller = "Subscriptions", action = "Get" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });

      context.Routes.MapHttpRoute(
        name: Routes.Subscriptions_Create,
        routeTemplate: _UrlPrefix + "subscriptions/create",
        defaults: new { controller = "Subscriptions", action = "Create" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.Routes.MapHttpRoute(
        name: Routes.Subscriptions_Delete,
        routeTemplate: _UrlPrefix + "subscriptions/{id}.json",
        defaults: new { controller = "Subscriptions", action = "Delete" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), });
    }

  }

}
