using System.Web.Http.Routing;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Areas.FeedbinApi {

  public static class Routes {

    private const string _RouteNamePrefix = "FeedbinApi_";

    public const string Subscriptions_GetAll = _RouteNamePrefix + "Subscriptions_GetAll";
    public const string Subscriptions_Get = _RouteNamePrefix + "Subscriptions_Get";
    public const string Subscriptions_Create = _RouteNamePrefix + "Subscriptions_Create";

    public static string CreateApiUrlForGetSubscription(UrlHelper urlHelper, int subscriptionId) {
      Guard.ArgNotNull(urlHelper, "urlHelper");

      return urlHelper.Link(Subscriptions_Get, new { id = subscriptionId, });
    }

  }

}
