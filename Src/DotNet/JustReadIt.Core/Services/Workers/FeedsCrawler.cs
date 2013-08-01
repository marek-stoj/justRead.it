using System;
using System.Collections.Generic;
using System.Transactions;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.Services.Workers {

  public class FeedsCrawler : IFeedsCrawler {

    private const int _FeedsToCrawlBatchSize = 10;
    private static readonly TimeSpan _FeedsCrawlInterval = TimeSpan.FromMinutes(15);

    private readonly IFeedRepository _feedRepository;
    private readonly IFeedItemRepository _feedItemRepository;
    private readonly Feeds.IFeedFetcher _feedFetcher;
    private readonly Feeds.IFeedParser _feedParser;

    public FeedsCrawler(IFeedRepository feedRepository, IFeedItemRepository feedItemRepository, Feeds.IFeedFetcher feedFetcher, Feeds.IFeedParser feedParser) {
      Guard.ArgNotNull(feedRepository, "feedRepository");
      Guard.ArgNotNull(feedItemRepository, "feedItemRepository");
      Guard.ArgNotNull(feedFetcher, "feedFetcher");
      Guard.ArgNotNull(feedParser, "feedParser");

      _feedRepository = feedRepository;
      _feedItemRepository = feedItemRepository;
      _feedFetcher = feedFetcher;
      _feedParser = feedParser;
    }

    public void CrawlAllFeeds() {
      IEnumerable<Domain.Feed> allFeeds;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        DateTime maxDateLastCrawlStarted =
          DateTime.UtcNow.Add(-_FeedsCrawlInterval);

        allFeeds =
          _feedRepository.GetFeedsToCrawl(
            _FeedsToCrawlBatchSize,
            maxDateLastCrawlStarted);

        ts.Complete();
      }

      foreach (Domain.Feed feed in allFeeds) {
        Console.WriteLine("Crawling feed: " + feed.FeedUrl);

        try {
          CrawlFeed(feed);
        }
        catch (Exception exc) {
          Console.WriteLine("Error: {0}", exc);
        }
      }
    }

    private void CrawlFeed(Domain.Feed feed) {
      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        _feedRepository.SetDateLastCrawlStarted(feed.Id, DateTime.UtcNow);

        ts.Complete();
      }

      Feeds.FetchFeedResult fetchFeedResult =
        _feedFetcher.FetchFeed(feed.FeedUrl);

      Feeds.Feed parsedFeed =
        _feedParser.Parse(fetchFeedResult.FeedContent);

      foreach (Feeds.FeedItem parsedFeedItem in parsedFeed.Items) {
        // TODO IMM HI: do we want to keep this try/catch?
        try {
          AddFeedItemIfNeeded(parsedFeedItem, feed.Id);
        }
        catch (Exception exc) {
          Console.WriteLine("Error: {0}", exc);
        }
      }
    }

    private void AddFeedItemIfNeeded(Feeds.FeedItem parsedFeedItem, int feedId) {
      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        // TODO IMM HI: check and insert if new

        ts.Complete();
      }
    }

  }

}
