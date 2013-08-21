using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;
using JustReadIt.WebApp.Core.MvcEx;

// TODO IMM HI: 404 on some js.map file (see errors in logs)

namespace JustReadIt.WebApp.Areas.App {

  public class AppApiAreaRegistration : AreaRegistration {

    public override string AreaName {
      get { return "App"; }
    }

    public override void RegisterArea(AreaRegistrationContext context) {
      context.MapHttpRoute(
        name: Routes.Subscriptions_GetList,
        routeTemplate: RouteTemplatePrefix + "subscriptions",
        defaults: new { controller = "Subscriptions", action = "GetSubscriptionsList" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.MapHttpRoute(
        name: Routes.Subscriptions_GetItems,
        routeTemplate: RouteTemplatePrefix + "subscriptions/{id}/items",
        defaults: new { controller = "Subscriptions", action = "GetItems" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });

      context.MapHttpRoute(
        name: Routes.Subscriptions_Add,
        routeTemplate: RouteTemplatePrefix + "subscriptions/add",
        defaults: new { controller = "Subscriptions", action = "Add" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.MapHttpRoute(
        name: Routes.Subscriptions_Import,
        routeTemplate: RouteTemplatePrefix + "subscriptions/import",
        defaults: new { controller = "Subscriptions", action = "Import" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.MapHttpRoute(
        name: Routes.Subscriptions_MarkAllItemsAsRead,
        routeTemplate: RouteTemplatePrefix + "subscriptions/{id}/mark-all-items-as-read",
        defaults: new { controller = "Subscriptions", action = "MarkAllItemsAsRead" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), id = @"\d+", });

      context.MapHttpRoute(
        name: Routes.FeedItems_GetFeedItemContent,
        routeTemplate: RouteTemplatePrefix + "feeditems/{id}/content",
        defaults: new { controller = "FeedItems", action = "GetFeedItemContent" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });

      context.MapHttpRoute(
        name: Routes.FeedItems_ToggleIsRead,
        routeTemplate: RouteTemplatePrefix + "feeditems/{id}/toggle-is-read",
        defaults: new { controller = "FeedItems", action = "ToggleIsRead" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), id = @"\d+", });
    }

    private string RouteTemplatePrefix {
      get { return string.Format("{0}/api/", AreaName); }
    }

  }

}
