using System.Security;
using System.Web.Mvc;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Security;
using JustReadIt.WebApp.Core.MvcEx;
using JustReadIt.WebApp.Core.Resources;
using System.Web;

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

    protected string CurrentUsername {
      get {
        IJustReadItPrincipal justReadItPrincipal = HttpContext.User as IJustReadItPrincipal;

        if (justReadItPrincipal == null) {
          throw new SecurityException("User is not signed in.");
        }

        return justReadItPrincipal.Identity.Name;
      }
    }

    protected int CurrentUserAccountId {
      get {
        IJustReadItPrincipal justReadItPrincipal = HttpContext.User as IJustReadItPrincipal;

        if (justReadItPrincipal == null) {
          throw new SecurityException("User is not signed in.");
        }

        return justReadItPrincipal.UserAccountId;
      }
    }

  }

}
