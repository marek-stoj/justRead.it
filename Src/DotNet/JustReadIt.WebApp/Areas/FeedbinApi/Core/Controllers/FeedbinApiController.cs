using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web;
using System.Web.Http;
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

    protected HttpResponseException HttpOk() {
      return new HttpResponseException(HttpStatusCode.OK);
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

    protected HttpResponseException HttpUnsupportedMediaType() {
      return new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
    }

    protected HttpResponseException HttpFound(IDictionary<string, string> responseHeaders = null) {
      HttpResponseMessage httpResponseMessage =
        new HttpResponseMessage(HttpStatusCode.Found);

      if (responseHeaders != null) {
        AddResponseHeaders(httpResponseMessage, responseHeaders);
      }

      return new HttpResponseException(httpResponseMessage);
    }

    protected HttpResponseException HttpCreated(IDictionary<string, string> responseHeaders = null) {
      HttpResponseMessage httpResponseMessage =
        new HttpResponseMessage(HttpStatusCode.Created);

      if (responseHeaders != null) {
        AddResponseHeaders(httpResponseMessage, responseHeaders);
      }

      return new HttpResponseException(httpResponseMessage);
    }

    protected HttpResponseException HttpNoContent() {
      return new HttpResponseException(HttpStatusCode.NoContent);
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

    private static void AddResponseHeaders(HttpResponseMessage httpResponseMessage, IDictionary<string, string> responseHeaders) {
      foreach (KeyValuePair<string, string> responseHeader in responseHeaders) {
        httpResponseMessage.Headers.Add(responseHeader.Key, new[] { responseHeader.Value });
      }
    }

  }

}
