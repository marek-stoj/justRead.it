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
      RegisterSubscriptionsRoutes(context);
      RegisterFeedsRoutes(context);
      RegisterEntriesRoutes(context);
      RegisterTaggingsRoutes(context);
    }

    private static void RegisterSubscriptionsRoutes(AreaRegistrationContext context) {
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
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), id = @"\d+", });

      context.Routes.MapHttpRoute(
        name: Routes.Subscriptions_UpdateViaPatch,
        routeTemplate: _UrlPrefix + "subscriptions/{id}.json",
        defaults: new { controller = "Subscriptions", action = "UpdateViaPatch" },
        constraints: new { httpMethod = new HttpMethodConstraint(new HttpMethod("PATCH")), id = @"\d+", });

      context.Routes.MapHttpRoute(
        name: Routes.Subscriptions_UpdateViaPost,
        routeTemplate: _UrlPrefix + "subscriptions/{id}/update.json",
        defaults: new { controller = "Subscriptions", action = "UpdateViaPost" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), id = @"\d+", });
    }

    private static void RegisterFeedsRoutes(AreaRegistrationContext context) {
      context.Routes.MapHttpRoute(
        name: Routes.Feeds_Get,
        routeTemplate: _UrlPrefix + "feeds/{id}.json",
        defaults: new { controller = "Feeds", action = "Get" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });

      context.Routes.MapHttpRoute(
        name: Routes.Feeds_GetEntries,
        routeTemplate: _UrlPrefix + "feeds/{id}/entries.json",
        defaults: new { controller = "Feeds", action = "GetEntries" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });
    }

    private static void RegisterEntriesRoutes(AreaRegistrationContext context) {
      context.Routes.MapHttpRoute(
        name: Routes.Entries_GetAll,
        routeTemplate: _UrlPrefix + "entries.json",
        defaults: new { controller = "Entries", action = "GetAll", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.Routes.MapHttpRoute(
        name: Routes.Entries_Get,
        routeTemplate: _UrlPrefix + "entries/{id}.json",
        defaults: new { controller = "Entries", action = "Get" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });

      context.Routes.MapHttpRoute(
        name: Routes.Entries_GetAllUnread,
        routeTemplate: _UrlPrefix + "unread_entries.json",
        defaults: new { controller = "Entries", action = "GetAllUnread", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.Routes.MapHttpRoute(
        name: Routes.Entries_GetAllStarred,
        routeTemplate: _UrlPrefix + "starred_entries.json",
        defaults: new { controller = "Entries", action = "GetAllStarred", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.Routes.MapHttpRoute(
        name: Routes.Entries_CreateUnread,
        routeTemplate: _UrlPrefix + "unread_entries.json",
        defaults: new { controller = "Entries", action = "CreateUnread", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.Routes.MapHttpRoute(
        name: Routes.Entries_DeleteUnreadViaDelete,
        routeTemplate: _UrlPrefix + "unread_entries.json",
        defaults: new { controller = "Entries", action = "DeleteUnreadViaDelete", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), });

      context.Routes.MapHttpRoute(
        name: Routes.Entries_DeleteUnreadViaPost,
        routeTemplate: _UrlPrefix + "unread_entries/delete.json",
        defaults: new { controller = "Entries", action = "DeleteUnreadViaPost", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.Routes.MapHttpRoute(
        name: Routes.Entries_CreateStarred,
        routeTemplate: _UrlPrefix + "starred_entries.json",
        defaults: new { controller = "Entries", action = "CreateStarred", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.Routes.MapHttpRoute(
        name: Routes.Entries_DeleteStarredViaDelete,
        routeTemplate: _UrlPrefix + "starred_entries.json",
        defaults: new { controller = "Entries", action = "DeleteStarredViaDelete", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), });

      context.Routes.MapHttpRoute(
        name: Routes.Entries_DeleteStarredViaPost,
        routeTemplate: _UrlPrefix + "starred_entries/delete.json",
        defaults: new { controller = "Entries", action = "DeleteStarredViaPost", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });
    }

    private static void RegisterTaggingsRoutes(AreaRegistrationContext context) {
      context.Routes.MapHttpRoute(
        name: Routes.Taggings_GetAll,
        routeTemplate: _UrlPrefix + "taggings.json",
        defaults: new { controller = "Taggings", action = "GetAll", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.Routes.MapHttpRoute(
        name: Routes.Taggings_Get,
        routeTemplate: _UrlPrefix + "taggings/{id}.json",
        defaults: new { controller = "Taggings", action = "Get", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });

      context.Routes.MapHttpRoute(
        name: Routes.Taggings_Create,
        routeTemplate: _UrlPrefix + "taggings.json",
        defaults: new { controller = "Taggings", action = "Create", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.Routes.MapHttpRoute(
        name: Routes.Taggings_Delete,
        routeTemplate: _UrlPrefix + "taggings/{id}.json",
        defaults: new { controller = "Taggings", action = "Delete", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), id = @"\d+", });
    }

  }

}
