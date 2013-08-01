using System;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.DataAccess.Dapper.Exceptions;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class FeedItemRepository : DapperRepository, IFeedItemRepository {

    public FeedItemRepository(string connectionString)
      : base(connectionString) {
    }

    public void Add(FeedItem feedItem) {
      Guard.ArgNotNull(feedItem, "feedItem");

      if (feedItem.Id != 0) {
        throw new ArgumentException("Non-transient entity can't be added. Id must be 0.", "feedItem");
      }

      feedItem.DateCreated = DateTime.UtcNow;

      using (var db = CreateOpenedConnection()) {
        int feedItemId =
          db.Query<int>(
            " insert into FeedItem" +
            " (FeedId, DateCreated, Title, Url, DatePublished, Author, Summary, Content)" +
            " values" +
            " (@FeedId, @DateCreated, @Title, @Url, @DatePublished, @Author, @Summary, @Content)" +
            " " +
            " select cast(scope_identity() as int);",
            new {
              FeedId = feedItem.FeedId,
              DateCreated = feedItem.DateCreated,
              Title = feedItem.Title,
              Url = feedItem.Url,
              DatePublished = feedItem.DatePublished,
              Author = feedItem.Author,
              Summary = feedItem.Summary,
              Content = feedItem.Content,
            })
            .Single();

        if (feedItemId <= 0) {
          throw new IdentityInsertFailedException();
        }

        feedItem.Id = feedItemId;
      }

    }

    public bool Exists(string url) {
      Guard.ArgNotNullNorEmpty(url, "url");

      using (var db = CreateOpenedConnection()) {
        int existsInt =
          db.Query<int>(
            " select" +
            "   case when" +
            "     exists(" +
            "       select" +
            "         fi.Id" +
            "       from FeedItem fi" +
            "       where 1 = 1" +
            "         and fi.UrlChecksum = checksum(@Url)" +
            "         and fi.Url = @Url" +
            "     )" +
            "     then 1" +
            "     else 0" +
            "   end",
            new {
              Url = url,
            }).Single();

        return existsInt == 1;
      }
    }

  }

}
