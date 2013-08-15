using System.Net;
using System.Web.Mvc;
using System.Web.UI;
using ImmRafSoft.Net;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Core.Controllers {

  public class AppController : JustReadItController {

    private readonly IWebClientFactory _webClientFactory;

    public AppController(IWebClientFactory webClientFactory) {
      Guard.ArgNotNull(webClientFactory, "webClientFactory");

      _webClientFactory = webClientFactory;
    }

    public AppController()
      : this(CommonIoC.GetWebClientFactory()) {
    }

    [HttpGet]
    public ActionResult Index() {
      return View();
    }

    // TODO IMM HI: getting favicons could be done better (eg. CSS sprite per user)
    [HttpGet]
    [OutputCache(VaryByParam = "url", Location = OutputCacheLocation.Any, Duration = 1209600)]
    public ActionResult GetFavicon(string url) {
      if (url.IsNullOrEmpty()) {
        return BadRequest();
      }

      byte[] faviconData;
      WebHeaderCollection responseHeaders;

      using (IWebClient webClient = _webClientFactory.CreateWebClient()) {
        faviconData =
          webClient.DownloadData(
            string.Format("http://immsoft.apphb.com/api/favicons/find-for-url?url={0}", url),
            out responseHeaders);
      }

      string contentType = responseHeaders["Content-Type"];

      if (contentType.IsNullOrEmpty()) {
        return HttpNotFound();
      }

      return new FileContentResult(faviconData, contentType);
    }

  }

}
