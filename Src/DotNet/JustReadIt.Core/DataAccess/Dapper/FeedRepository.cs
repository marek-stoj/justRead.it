using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.DataAccess.Dapper.Exceptions;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class FeedRepository : DapperRepository, IFeedRepository {

    public FeedRepository(string connectionString)
      : base(connectionString) {
    }

    public IEnumerable<Feed> GetFeedsToCrawl(int maxCount, DateTime maxDateLastCrawlStarted) {
      using (var db = CreateOpenedConnection()) {
        IEnumerable<Feed> feeds =
          db.Query<Feed>(
            " select top (@MaxCount)" +
            "   f.*" +
            " from Feed f" +
            " where f.DateLastCrawlStarted is null or f.DateLastCrawlStarted <= @MaxDateLastCrawlStarted" +
            " order by f.DateCreated desc, f.Id desc",
            new {
              MaxCount = maxCount,
              MaxDateLastCrawlStarted = maxDateLastCrawlStarted,
            });

        return feeds;
      }
    }

    public bool Exists(int id) {
      using (var db = CreateOpenedConnection()) {
        int existsInt =
          db.Query<int>(
            " select" +
            "   case when" +
            "     exists(" +
            "       select" +
            "         f.Id" +
            "       from Feed f" +
            "       where 1 = 1" +
            "         and f.Id = @Id" +
            "     )" +
            "     then 1" +
            "     else 0" +
            "   end",
            new {
              Id = id,
            }).Single();

        return existsInt == 1;
      }
    }

    public Feed FindById(int id) {
      using (var db = CreateOpenedConnection()) {
        Feed feed =
          db.Query<Feed>(
            " select * from Feed f where f.Id = @Id",
            new {
              Id = id,
            }).SingleOrDefault();

        return feed;
      }
    }

    public void Add(Feed feed) {
      Guard.ArgNotNull(feed, "feed");

      if (feed.Id != 0) {
        throw new ArgumentException("Non-transient entity can't be added. Id must be 0.", "feed");
      }

      using (var db = CreateOpenedConnection()) {
        feed.DateCreated = DateTime.UtcNow;

        int feedId =
          db.Query<int>(
            " insert into Feed" +
            " (DateCreated, Title, FeedUrl, SiteUrl)" +
            " values" +
            " (@DateCreated, @Title, @FeedUrl, @SiteUrl);" +
            " " +
            " select cast(scope_identity() as int);",
            new {
              DateCreated = feed.DateCreated,
              Title = feed.Title,
              FeedUrl = feed.FeedUrl,
              SiteUrl = feed.SiteUrl,
            })
            .Single();

        if (feedId <= 0) {
          throw new IdentityInsertFailedException();
        }

        feed.Id = feedId;
      }
    }

    public int? FindFeedId(string feedUrl) {
      Guard.ArgNotNullNorEmpty(feedUrl, "feedUrl");

      using (var db = CreateOpenedConnection()) {
        int? feedId =
          db.Query<int?>(
            " select" +
            "   Id" +
            " from Feed" +
            " where FeedUrlChecksum = checksum(@FeedUrl)" +
            "   and FeedUrl = @FeedUrl",
            new { FeedUrl = feedUrl })
            .SingleOrDefault();

        return feedId;
      }
    }

    public bool SetDateLastCrawlStarted(int id, DateTime dateTime) {
      using (var db = CreateOpenedConnection()) {
        int affectedRowsCount =
          db.Query<int>(
            " update Feed" +
            " set DateLastCrawlStarted = @DateLastCrawlStarted" +
            " where 1 = 1" +
            "   and Id = @Id" +
            " " +
            " select @@ROWCOUNT;",
            new {
              Id = id,
              DateLastCrawlStarted = dateTime,
            })
            .Single();

        Debug.Assert(affectedRowsCount == 0 || affectedRowsCount == 1, string.Format("Unexpected number of rows affected while updating feed's last date crawl started. Feed id: '{0}'. Affected rows count: '{1}'.", id, affectedRowsCount));

        return affectedRowsCount > 0;
      }
    }

  }

}
