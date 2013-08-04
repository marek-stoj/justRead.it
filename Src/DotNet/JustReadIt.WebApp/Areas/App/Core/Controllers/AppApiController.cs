using System.Net;
using System.Web.Http;

namespace JustReadIt.WebApp.Areas.App.Core.Controllers {

  [Authorize]
  public abstract class AppApiController : ApiController {

    protected HttpResponseException HttpOk() {
      return new HttpResponseException(HttpStatusCode.OK);
    }

    protected HttpResponseException HttpBadRequest() {
      return new HttpResponseException(HttpStatusCode.BadRequest);
    }

  }

}
