using System;
using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using JustReadIt.Core.Common;
using JustReadIt.Core.Common.Logging;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;
using log4net;

namespace JustReadIt.Core.Services.Workers {

  public class FeedsCrawler : IFeedsCrawler {

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
      // TODO IMM HI: batching?
      IEnumerable<Feed> feedsBatch;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        DateTime maxDateLastCrawlStarted =
          DateTime.UtcNow.Add(-_FeedsCrawlInterval);

        feedsBatch =
          _feedRepository.GetFeedsToCrawl(maxDateLastCrawlStarted);

        ts.Complete();
      }

      foreach (Feed feed in feedsBatch) {
        // ReSharper disable AccessToForEachVariableInClosure
        _log.DebugIfEnabled(() => string.Format("Crawling feed: '{0}'.", feed.FeedUrl));
        // ReSharper restore AccessToForEachVariableInClosure

        try {
          CrawlFeed(feed);
        }
        catch (Exception exc) {
          _log.ErrorIfEnabled(() => "Error.", exc);
        }
      }
    }

    private void CrawlFeed(Feed feed) {
      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        _feedRepository.SetDateLastCrawlStarted(feed.Id, DateTime.UtcNow);

        ts.Complete();
      }

      Feeds.FetchFeedResult fetchFeedResult =
        _feedFetcher.FetchFeed(feed.FeedUrl);

      Feeds.Feed parsedFeed =
        _feedParser.Parse(fetchFeedResult.FeedContent);

      foreach (Feeds.FeedItem parsedFeedItem in parsedFeed.Items) {
        try {
          AddFeedItemIfNeeded(parsedFeedItem, feed.Id);
        }
        catch (Exception exc) {
          _log.ErrorIfEnabled(() => "Error.", exc);
        }
      }
    }

    private void AddFeedItemIfNeeded(Feeds.FeedItem parsedFeedItem, int feedId) {
      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        if (_feedItemRepository.Exists(parsedFeedItem.Url)) {
          _log.DebugIfEnabled(() => string.Format("Feed item already exists. URL: '{0}'", parsedFeedItem.Url));

          ts.Complete();

          return;
        }

        _log.DebugIfEnabled(() => string.Format("Adding new feed item. URL: '{0}'", parsedFeedItem.Url));

        var feedItem =
          new FeedItem {
            FeedId = feedId,
            Title = parsedFeedItem.Title,
            Url = parsedFeedItem.Url,
            DatePublished = parsedFeedItem.DatePublished,
            Author = parsedFeedItem.Author,
            Summary = parsedFeedItem.Summary,
            Content = parsedFeedItem.Content,
          };

        _feedItemRepository.Add(feedItem);

        ts.Complete();
      }
    }

  }

}
