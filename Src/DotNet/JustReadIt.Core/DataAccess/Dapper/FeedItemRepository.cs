using System;
using System.Collections.Generic;
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

    public IEnumerable<FeedItem> Query(int userAccountId, int maxCount, FeedItemFilter feedItemFilter) {
      Guard.ArgNotNull(feedItemFilter, "feedItemFilter");

      // TODO IMM HI: handle FeedItemFilter.IsRead
      // TODO IMM HI: handle FeedItemFilter.IsStarred
      const string orderBy = " order by fi.DateCreated desc, fi.Id desc ";

      using (var db = CreateOpenedConnection()) {
        bool pageNumberHasValue = feedItemFilter.PageNumber.HasValue;

        string query =
          " with FeedItems as" +
          " (" +
          "   select" +
          (pageNumberHasValue
             ? " row_number() over(" + orderBy + ") as RowNumber,"
             : "") +
          "     fi.*" +
          "   from FeedItem fi" +
          "   join UserFeedGroupFeed ufgf on ufgf.FeedId = fi.FeedId" +
          "   join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
          "   where 1 = 1" +
          "     and ufg.UserAccountId = @UserAccountId" +
          (feedItemFilter.FeedId.HasValue
             ? " and fi.FeedId = @FeedId"
             : "") +
          (feedItemFilter.DateCreatedSince.HasValue
             ? " and fi.DateCreated > @DateCreatedSince"
             : "") +
          (feedItemFilter.Ids != null
             ? " and fi.Id in @Ids"
             : "") +
          " )" +
          " select top (@MaxCount)" +
          "   *" +
          " from FeedItems fi" +
          (pageNumberHasValue
             ? " where fi.RowNumber between ((@PageNumber - 1) * @MaxCount + 1) and (@PageNumber * @MaxCount)"
             : "") +
          orderBy;

        IEnumerable<FeedItem> feedItems =
          db.Query<FeedItem>(
            query,
            new {
              UserAccountId = userAccountId,
              MaxCount = maxCount,
              FeedId = feedItemFilter.FeedId,
              DateCreatedSince = feedItemFilter.DateCreatedSince,
              PageNumber = feedItemFilter.PageNumber,
              Ids = feedItemFilter.Ids,
            });

        return feedItems;
      }
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

    public FeedItem FindById(int id) {
      using (var db = CreateOpenedConnection()) {
        FeedItem feedItem =
          db.Query<FeedItem>(
            " select *" +
            " from FeedItem fi" +
            " where 1 = 1" +
            "   and fi.Id = @Id",
            new {
              Id = id,
            }).SingleOrDefault();

        return feedItem;
      }
    }

  }

}
