using System;
using System.Collections.Generic;
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

    public IEnumerable<Feed> GetAll() {
      using (var db = CreateOpenedConnection()) {
        IEnumerable<Feed> feeds =
          db.Query<Feed>(
            "select * from Feed");

        return feeds;
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

  }

}
