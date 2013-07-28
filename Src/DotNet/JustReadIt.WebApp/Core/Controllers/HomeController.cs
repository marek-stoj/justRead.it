using System.Collections.Generic;
using System.Web.Mvc;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.WebApp.Core.App;
using System.Linq;

namespace JustReadIt.WebApp.Core.Controllers {

  public class HomeController : JustReadItController {

    private readonly IFeedRepository _feedRepository;

    public HomeController(IFeedRepository feedRepository) {
      Guard.ArgNotNull(feedRepository, "feedRepository");

      _feedRepository = feedRepository;
    }

    public HomeController()
      : this(IoC.CreateFeedRepository()) {
    }

    [HttpGet]
    public ActionResult Index() {
      int userAccountId = CurrentUserAccountId;
      string username = CurrentUsername;

      // TODO IMM HI: remove
      List<Feed> feedsList =
        _feedRepository.GetAll().ToList();

      return View();
    }

    // TODO IMM HI: remove
    [HttpGet]
    public ActionResult Fancybox() {
      return View();
    }

  }

}
