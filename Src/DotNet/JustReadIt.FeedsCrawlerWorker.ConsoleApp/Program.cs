using JustReadIt.Core.Common;
using JustReadIt.Core.Services.Workers;
using log4net.Config;

namespace JustReadIt.FeedsCrawlerWorker.ConsoleApp {

  internal class Program {

    private readonly IFeedsCrawler _feedsCrawler;

    public Program(IFeedsCrawler feedsCrawler) {
      _feedsCrawler = feedsCrawler;
    }

    public Program()
      : this(CommonIoC.GetFeedsCrawler()) {
    }

    private static void Main(string[] args) {
      var program = new Program();

      program.Run(args);
    }

    private void Run(string[] args) {
      XmlConfigurator.Configure();

      _feedsCrawler.CrawlAllFeeds();
    }

  }

}
