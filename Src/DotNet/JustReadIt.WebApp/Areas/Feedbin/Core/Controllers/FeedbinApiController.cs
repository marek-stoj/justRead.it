using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Controllers {

  // TODO IMM HI: think about returning json in erroneous responses in addition to http status code: status, message, errors
  [FeedbinAuthorize]
  public abstract class FeedbinApiController : ApiController {

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

    private static void AddResponseHeaders(HttpResponseMessage httpResponseMessage, IDictionary<string, string> responseHeaders) {
      foreach (KeyValuePair<string, string> responseHeader in responseHeaders) {
        httpResponseMessage.Headers.Add(responseHeader.Key, new[] { responseHeader.Value });
      }
    }

  }

}
