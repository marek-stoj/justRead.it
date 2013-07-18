using System.Web.Mvc;

namespace JustReadIt.WebApp.Core.Controllers {

  public class HomeController : JustReadItController {

    [HttpGet]
    public ActionResult Index() {
      return View();
    }

    // TODO IMM HI: remove
    [HttpGet]
    public ActionResult Fancybox() {
      return View();
    }

  }

}
