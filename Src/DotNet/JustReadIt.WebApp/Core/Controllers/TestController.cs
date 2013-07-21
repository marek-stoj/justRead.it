using System.Web.Mvc;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Core.App;
using Io = System.IO;

namespace JustReadIt.WebApp.Core.Controllers {

  // TODO IMM HI: remove
  public class TestController : JustReadItController {

    [HttpGet]
    public ActionResult ImportOpml() {
      IOpmlImporter opmlImporter = IoC.CreateOpmlImporter();

      string opmlXml = Io.File.ReadAllText("C:\\feeds_old_reader.opml");
      //string opmlXml = Io.File.ReadAllText("C:\\feeds_feedbin.opml");
      //string opmlXml = Io.File.ReadAllText("C:\\feeds_feedly.opml");
      opmlImporter.Import(opmlXml, 1);

      return Content("OK");
    }

  }

}
