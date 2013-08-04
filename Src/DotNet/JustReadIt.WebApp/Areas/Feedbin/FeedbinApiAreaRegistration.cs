using System.Net.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;
using JustReadIt.WebApp.Core.MvcEx;

namespace JustReadIt.WebApp.Areas.Feedbin {

  public class FeedbinApiAreaRegistration : AreaRegistration {

    public override string AreaName {
      get { return "Feedbin"; }
    }

    public override void RegisterArea(AreaRegistrationContext context) {
      RegisterSubscriptionsRoutes(context);
      RegisterFeedsRoutes(context);
      RegisterEntriesRoutes(context);
      RegisterTaggingsRoutes(context);
    }

    private void RegisterSubscriptionsRoutes(AreaRegistrationContext context) {
      context.MapHttpRoute(
        name: Routes.Subscriptions_GetAll,
        routeTemplate: RouteTemplatePrefix + "subscriptions.json",
        defaults: new { controller = "Subscriptions", action = "GetAll", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.MapHttpRoute(
        name: Routes.Subscriptions_Get,
        routeTemplate: RouteTemplatePrefix + "subscriptions/{id}.json",
        defaults: new { controller = "Subscriptions", action = "Get" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });

      context.MapHttpRoute(
        name: Routes.Subscriptions_Create,
        routeTemplate: RouteTemplatePrefix + "subscriptions/create",
        defaults: new { controller = "Subscriptions", action = "Create" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.MapHttpRoute(
        name: Routes.Subscriptions_Delete,
        routeTemplate: RouteTemplatePrefix + "subscriptions/{id}.json",
        defaults: new { controller = "Subscriptions", action = "Delete" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), id = @"\d+", });

      context.MapHttpRoute(
        name: Routes.Subscriptions_UpdateViaPatch,
        routeTemplate: RouteTemplatePrefix + "subscriptions/{id}.json",
        defaults: new { controller = "Subscriptions", action = "UpdateViaPatch" },
        constraints: new { httpMethod = new HttpMethodConstraint(new HttpMethod("PATCH")), id = @"\d+", });

      context.MapHttpRoute(
        name: Routes.Subscriptions_UpdateViaPost,
        routeTemplate: RouteTemplatePrefix + "subscriptions/{id}/update.json",
        defaults: new { controller = "Subscriptions", action = "UpdateViaPost" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), id = @"\d+", });
    }

    private void RegisterFeedsRoutes(AreaRegistrationContext context) {
      context.MapHttpRoute(
        name: Routes.Feeds_Get,
        routeTemplate: RouteTemplatePrefix + "feeds/{id}.json",
        defaults: new { controller = "Feeds", action = "Get" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });

      context.MapHttpRoute(
        name: Routes.Feeds_GetEntries,
        routeTemplate: RouteTemplatePrefix + "feeds/{id}/entries.json",
        defaults: new { controller = "Feeds", action = "GetEntries" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });
    }

    private void RegisterEntriesRoutes(AreaRegistrationContext context) {
      context.MapHttpRoute(
        name: Routes.Entries_GetAll,
        routeTemplate: RouteTemplatePrefix + "entries.json",
        defaults: new { controller = "Entries", action = "GetAll", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.MapHttpRoute(
        name: Routes.Entries_Get,
        routeTemplate: RouteTemplatePrefix + "entries/{id}.json",
        defaults: new { controller = "Entries", action = "Get" },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });

      context.MapHttpRoute(
        name: Routes.Entries_GetAllUnread,
        routeTemplate: RouteTemplatePrefix + "unread_entries.json",
        defaults: new { controller = "Entries", action = "GetAllUnread", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.MapHttpRoute(
        name: Routes.Entries_GetAllStarred,
        routeTemplate: RouteTemplatePrefix + "starred_entries.json",
        defaults: new { controller = "Entries", action = "GetAllStarred", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.MapHttpRoute(
        name: Routes.Entries_CreateUnread,
        routeTemplate: RouteTemplatePrefix + "unread_entries.json",
        defaults: new { controller = "Entries", action = "CreateUnread", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.MapHttpRoute(
        name: Routes.Entries_DeleteUnreadViaDelete,
        routeTemplate: RouteTemplatePrefix + "unread_entries.json",
        defaults: new { controller = "Entries", action = "DeleteUnreadViaDelete", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), });

      context.MapHttpRoute(
        name: Routes.Entries_DeleteUnreadViaPost,
        routeTemplate: RouteTemplatePrefix + "unread_entries/delete.json",
        defaults: new { controller = "Entries", action = "DeleteUnreadViaPost", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.MapHttpRoute(
        name: Routes.Entries_CreateStarred,
        routeTemplate: RouteTemplatePrefix + "starred_entries.json",
        defaults: new { controller = "Entries", action = "CreateStarred", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.MapHttpRoute(
        name: Routes.Entries_DeleteStarredViaDelete,
        routeTemplate: RouteTemplatePrefix + "starred_entries.json",
        defaults: new { controller = "Entries", action = "DeleteStarredViaDelete", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), });

      context.MapHttpRoute(
        name: Routes.Entries_DeleteStarredViaPost,
        routeTemplate: RouteTemplatePrefix + "starred_entries/delete.json",
        defaults: new { controller = "Entries", action = "DeleteStarredViaPost", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });
    }

    private void RegisterTaggingsRoutes(AreaRegistrationContext context) {
      context.MapHttpRoute(
        name: Routes.Taggings_GetAll,
        routeTemplate: RouteTemplatePrefix + "taggings.json",
        defaults: new { controller = "Taggings", action = "GetAll", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), });

      context.MapHttpRoute(
        name: Routes.Taggings_Get,
        routeTemplate: RouteTemplatePrefix + "taggings/{id}.json",
        defaults: new { controller = "Taggings", action = "Get", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"\d+", });

      context.MapHttpRoute(
        name: Routes.Taggings_Create,
        routeTemplate: RouteTemplatePrefix + "taggings.json",
        defaults: new { controller = "Taggings", action = "Create", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), });

      context.MapHttpRoute(
        name: Routes.Taggings_Delete,
        routeTemplate: RouteTemplatePrefix + "taggings/{id}.json",
        defaults: new { controller = "Taggings", action = "Delete", },
        constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), id = @"\d+", });
    }

    private string RouteTemplatePrefix {
      get { return string.Format("{0}/api/v2/", AreaName); }
    }

  }

}
