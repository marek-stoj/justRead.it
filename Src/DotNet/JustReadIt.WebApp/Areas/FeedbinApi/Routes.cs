using System.Web.Http.Routing;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Areas.FeedbinApi {

  public static class Routes {

    private const string _RouteNamePrefix = "FeedbinApi_";

    public const string Subscriptions_GetAll = _RouteNamePrefix + "Subscriptions_GetAll";
    public const string Subscriptions_Get = _RouteNamePrefix + "Subscriptions_Get";
    public const string Subscriptions_Create = _RouteNamePrefix + "Subscriptions_Create";
    public const string Subscriptions_Delete = _RouteNamePrefix + "Subscriptions_Delete";
    public const string Subscriptions_UpdateViaPatch = _RouteNamePrefix + "Subscriptions_UpdateViaPatch";
    public const string Subscriptions_UpdateViaPost = _RouteNamePrefix + "Subscriptions_UpdateViaPost";

    public const string Feeds_Get = _RouteNamePrefix + "Feeds_Get";
    public const string Feeds_GetEntries = _RouteNamePrefix + "Feeds_GetEntries";

    public const string Entries_GetAll = _RouteNamePrefix + "Entries_GetAll";
    public const string Entries_Get = _RouteNamePrefix + "Entries_Get";
    public const string Entries_GetAllUnread = _RouteNamePrefix + "Entries_GetAllUnread";
    public const string Entries_GetAllStarred = _RouteNamePrefix + "Entries_GetAllStarred";
    public const string Entries_CreateUnread = _RouteNamePrefix + "Entries_CreateUnread";
    public const string Entries_DeleteUnreadViaDelete = _RouteNamePrefix + "Entries_DeleteUnreadViaDelete";
    public const string Entries_DeleteUnreadViaPost = _RouteNamePrefix + "Entries_DeleteUnreadViaPost";
    public const string Entries_CreateStarred = _RouteNamePrefix + "Entries_CreateStarred";
    public const string Entries_DeleteStarredViaDelete = _RouteNamePrefix + "Entries_DeleteStarredViaDelete";
    public const string Entries_DeleteStarredViaPost = _RouteNamePrefix + "Entries_DeleteStarredViaPost";

    public static string CreateApiUrlForGetSubscription(UrlHelper urlHelper, int subscriptionId) {
      Guard.ArgNotNull(urlHelper, "urlHelper");

      return urlHelper.Link(Subscriptions_Get, new { id = subscriptionId, });
    }

  }

}
