using System;
using System.Globalization;
using System.Net;
using System.Security;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Security;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Controllers {

  [FeedbinAuthorize]
  public abstract class FeedbinApiController : ApiController {

    protected static DateTime? ParseFeedbinDateTime(string dateTimeString) {
      return
        DateTime.Parse(
          dateTimeString,
          CultureInfo.InvariantCulture,
          DateTimeStyles.RoundtripKind);
    }

    protected HttpResponseException HttpBadRequest() {
      return new HttpResponseException(HttpStatusCode.BadRequest);
    }

    protected HttpResponseException HttpForbidden() {
      return new HttpResponseException(HttpStatusCode.Forbidden);
    }

    protected HttpResponseException HttpNotFound() {
      return new HttpResponseException(HttpStatusCode.NotFound);
    }

    protected string CurrentUsername {
      get {
        IJustReadItPrincipal justReadItPrincipal = GetJustReadItPrincipal();

        return justReadItPrincipal.Identity.Name;
      }
    }

    protected int CurrentUserAccountId {
      get {
        IJustReadItPrincipal justReadItPrincipal = GetJustReadItPrincipal();

        return justReadItPrincipal.UserAccountId;
      }
    }

    private static IJustReadItPrincipal GetJustReadItPrincipal() {
      if (HttpContext.Current == null) {
        throw new InvalidOperationException("No http context present.");
      }

      IJustReadItPrincipal justReadItPrincipal = HttpContext.Current.User as IJustReadItPrincipal;

      if (justReadItPrincipal == null) {
        throw new SecurityException("User is not signed in.");
      }

      return justReadItPrincipal;
    }

  }

}
