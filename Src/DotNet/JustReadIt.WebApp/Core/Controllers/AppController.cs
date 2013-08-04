using System.Web.Mvc;

namespace JustReadIt.WebApp.Core.Controllers {

  public class AppController : JustReadItController {

    [HttpGet]
    public ActionResult Index() {
      return View();
    }

  }

}
