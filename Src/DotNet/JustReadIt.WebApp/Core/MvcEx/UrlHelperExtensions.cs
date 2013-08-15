using System;
using System.Web.Mvc;

namespace JustReadIt.WebApp.Core.MvcEx {

  public static class UrlHelperExtensions {

    public static string AbsoluteAction(this UrlHelper urlHelper, string actionName, string controllerName, object routeValues) {
      Uri url = urlHelper.RequestContext.HttpContext.Request.Url;

      if (url == null) {
        throw new InvalidOperationException("HttpContext.Request.Url is null.");
      }

      return
        string.Format(
          "{0}://{1}{2}",
          url.Scheme,
          url.Authority,
          urlHelper.Action(actionName, controllerName, routeValues));
    }

    public static string AbsoluteAction(this UrlHelper urlHelper, string actionName, string controllerName) {
      return AbsoluteAction(urlHelper, actionName, controllerName, null);
    }

    public static string AbsoluteAction(this UrlHelper urlHelper, string actionName) {
      return AbsoluteAction(urlHelper, actionName, null);
    }

  }

}
