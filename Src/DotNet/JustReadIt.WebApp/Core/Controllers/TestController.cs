using System.Web.Mvc;
using JustReadIt.Core.Common;
using JustReadIt.Core.Services;
using JustReadIt.Core.Services.Feeds;
using Io = System.IO;

namespace JustReadIt.WebApp.Core.Controllers {

  // TODO IMM HI: remove
  public class TestController : JustReadItController {

    [HttpGet]
    public ActionResult ImportOpml() {
      IOpmlImporter opmlImporter = CommonIoC.GetOpmlImporter();

      //string opmlXml = Io.File.ReadAllText("C:\\feeds_old_reader_full.opml");
      //string opmlXml = Io.File.ReadAllText("C:\\feeds_old_reader_trimmed.opml");
      //string opmlXml = Io.File.ReadAllText("C:\\feeds_feedbin.opml");
      //string opmlXml = Io.File.ReadAllText("C:\\feeds_feedly.opml");
      string opmlXml = Io.File.ReadAllText("C:\\feeds.opml");
      opmlImporter.Import(opmlXml, 14);

      return Content("OK");
    }

    [HttpGet]
    public ActionResult ParseFeed() {
      Feed feed = new FeedParser().Parse(Io.File.ReadAllText("C:\\feed_atom.xml"));

      return Content(
        string.Format("{0}\r\n{1}\r\n{2}", feed.Title, feed.FeedUrl, feed.SiteUrl),
        "text/plain");
    }

  }

}
