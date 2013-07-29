using System;
using System.IO;
using ImmRafSoft.Net;
using JustReadIt.Core.Services.Feeds;
using JustReadIt.Core.Services.Opml;
using Feed = JustReadIt.Core.Services.Opml.Feed;

namespace JustReadIt.FeedsFetcher.ConsoleApp {

  internal class Program {

    private static void Main(string[] args) {
      var opmlParser = new OpmlParser();
      string opmlXml = File.ReadAllText("feeds.opml");
      ParseResult parseResult = opmlParser.Parse(opmlXml);
      int index = 3;

      var feedFetcher = new FeedFetcher(new SmartWebClientFactory());
      foreach (Feed feed in parseResult.UncategorizedFeeds) {
        Console.WriteLine("Feed: " + feed.FeedUrl);

        FetchFeedResult fetchFeedResult = feedFetcher.FetchFeed(feed.FeedUrl);

        string feedXml = fetchFeedResult.FeedContent;
        string feedXmlFileName = string.Format("feed_{0}.xml", index.ToString().PadLeft(2, '0'));

        File.WriteAllText(feedXmlFileName, feedXml);

        Console.WriteLine("{0}\t{1}\t{2}\t{3}", feedXmlFileName, feed.Title, feed.FeedUrl, feed.SiteUrl);

        index++;
      }
    }

  }

}
