using System.Web.Mvc;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Areas.App {

  public static class Routes {

    private const string _RouteNamePrefix = "AppApi_";

    public const string Default = _RouteNamePrefix + "Default";

    public const string Subscriptions_GetItems = _RouteNamePrefix + "Subscriptions_GetItems";
    public const string Subscriptions_MarkAllItemsAsRead = _RouteNamePrefix + "Subscriptions_MarkAllItemsAsRead";
    public const string Subscriptions_Add = _RouteNamePrefix + "Subscriptions_Add";
    public const string Subscriptions_Import = _RouteNamePrefix + "Subscriptions_Import";

    public const string FeedItems_GetFeedItemContent = _RouteNamePrefix + "FeedItems_GetFeedItemContent";
    public const string FeedItems_ToggleIsRead = _RouteNamePrefix + "FeedItems_ToggleIsRead";

    public static string GetUrl(UrlHelper urlHelper, string routeName) {
      Guard.ArgNotNull(routeName, "routeName");
      Guard.ArgNotNullNorEmpty(routeName, "routeName");

      return urlHelper.RouteUrl(routeName, new { httproute = "", });
    }

  }

}
