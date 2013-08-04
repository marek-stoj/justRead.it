using System.Web.Mvc;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.WebApp.Core.Controllers {

  public class HomeController : JustReadItController {

    private readonly IFeedRepository _feedRepository;

    public HomeController(IFeedRepository feedRepository) {
      Guard.ArgNotNull(feedRepository, "feedRepository");

      _feedRepository = feedRepository;
    }

    public HomeController()
      : this(CommonIoC.GetFeedRepository()) {
    }

    [HttpGet]
    public ActionResult Index() {
      if (Request.IsAuthenticated) {
        return RedirectToApp();
      }

      return View();
    }

    // TODO IMM HI: remove
    [HttpGet]
    public ActionResult Fancybox() {
      return View();
    }

  }

}
