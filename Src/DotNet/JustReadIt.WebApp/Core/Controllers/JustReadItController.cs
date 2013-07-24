using System.Web.Mvc;
using JustReadIt.WebApp.Core.MvcEx;
using JustReadIt.WebApp.Core.Resources;

namespace JustReadIt.WebApp.Core.Controllers {

  public abstract class JustReadItController : Controller {

    private MvcModelValidator _modelValidator;

    protected ActionResult BadRequest() {
      Response.StatusCode = 400;

      return Content("400 - Bad Request");
    }

    protected ActionResult Forbidden() {
      Response.StatusCode = 403;

      return Content("403 - Forbidden");
    }

    protected ActionResult RedirectToHome() {
      return RedirectToAction("Index", "Home");
    }

    protected MvcModelValidator ModelValidator {
      get {
        if (_modelValidator == null) {
          _modelValidator =
            new MvcModelValidator(
              new[] { ValidationResources.ResourceManager },
              ValidationResources.InvalidValue);
        }

        return _modelValidator;
      }
    }

  }

}
