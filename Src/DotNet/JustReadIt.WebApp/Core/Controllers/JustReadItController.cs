using System.Web.Mvc;

namespace JustReadIt.WebApp.Core.Controllers {

  public abstract class JustReadItController : Controller {

    protected ActionResult BadRequest() {
      Response.StatusCode = 400;

      return Content("400 - Bad Request");
    }

    protected ActionResult Forbidden() {
      Response.StatusCode = 403;

      return Content("403 - Forbidden");
    }

  }

}
