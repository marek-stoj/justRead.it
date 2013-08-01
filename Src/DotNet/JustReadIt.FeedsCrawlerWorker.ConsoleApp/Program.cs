using System.Configuration;
using ImmRafSoft.Net;
using JustReadIt.Core.DataAccess.Dapper;
using JustReadIt.Core.Services.Feeds;
using JustReadIt.Core.Services.Workers;

namespace JustReadIt.FeedsCrawlerWorker.ConsoleApp {

  internal class Program {

    private const string _ConnectionStringName_JustReadIt = "JustReadIt";

    private static void Main(string[] args) {
      // TODO IMM HI: common ioc?
      string connectionString =
        ConfigurationManager.ConnectionStrings[_ConnectionStringName_JustReadIt]
          .ConnectionString;

      var feedRepository = new FeedRepository(connectionString);
      var feedItemRepository = new FeedItemRepository(connectionString);
      var webClientFactory = new SmartWebClientFactory();
      var feedFetcher = new FeedFetcher(webClientFactory);
      var feedParser = new FeedParser();

      var feedsCrawler =
        new FeedsCrawler(
          feedRepository,
          feedItemRepository,
          feedFetcher,
          feedParser);

      feedsCrawler.CrawlAllFeeds();
    }

  }

}
