﻿using System;
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

    public string FindUrlById(int id) {
      using (var db = CreateOpenedConnection()) {
        string url =
          db.Query<string>(
            " select fi.Url" +
            " from FeedItem fi" +
            " where 1 = 1" +
            "   and fi.Id = @Id",
            new {
              Id = id,
            }).SingleOrDefault();

        return url;
      }
    }

    public IEnumerable<int> GetAllUnreadIds(int userAccountId) {
      using (var db = CreateOpenedConnection()) {
        IEnumerable<int> ids =
          db.Query<int>(
            " select fi.Id" +
            " from FeedItem fi" +
            " join UserFeedGroupFeed ufgf on ufgf.FeedId = fi.FeedId" +
            " join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            " where 1 = 1" +
            "   and ufg.UserAccountId = @UserAccountId" +
            "   and not exists(" +
            "     select urfi.Id" +
            "     from UserReadFeedItem urfi" +
            "     where 1 = 1" +
            "       and urfi.UserAccountId = @UserAccountId" +
            "       and urfi.FeedItemId = fi.Id)" +
            " order by fi.DateCreated desc, fi.Id desc",
            new {
              UserAccountId = userAccountId,
            });

        return ids;
      }
    }

    public IEnumerable<int> GetAllStarredIds(int userAccountId) {
      using (var db = CreateOpenedConnection()) {
        IEnumerable<int> ids =
          db.Query<int>(
            " select fi.Id" +
            " from FeedItem fi" +
            " join UserFeedGroupFeed ufgf on ufgf.FeedId = fi.FeedId" +
            " join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            " where 1 = 1" +
            "   and ufg.UserAccountId = @UserAccountId" +
            "   and exists(" +
            "     select usfi.Id" +
            "     from UserStarredFeedItem usfi" +
            "     where 1 = 1" +
            "       and usfi.UserAccountId = @UserAccountId" +
            "       and usfi.FeedItemId = fi.Id)" +
            " order by fi.DateCreated desc, fi.Id desc",
            new {
              UserAccountId = userAccountId,
            });

        return ids;
      }
    }

    public IEnumerable<int> GetExistingFeedItemIds(int userAccountId, IEnumerable<int> feedItemIds) {
      if (feedItemIds == null) {
        throw new ArgumentNullException("feedItemIds");
      }

      var feedItemIdsList = feedItemIds.ToList();

      if (feedItemIdsList.Count == 0) {
        return Enumerable.Empty<int>();
      }

      using (var db = CreateOpenedConnection()) {
        IEnumerable<int> existingFeedItemIds =
          db.Query<int>(
            " select" +
            "   fi.Id" +
            " from FeedItem fi" +
            " join UserFeedGroupFeed ufgf on ufgf.FeedId = fi.FeedId" +
            " join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            " where 1 = 1" +
            "   and ufg.UserAccountId = @UserAccountId" +
            "   and fi.Id in @FeedItemIds" +
            " order by fi.DateCreated desc, fi.Id desc",
            new {
              UserAccountId = userAccountId,
              FeedItemIds = feedItemIdsList,
            });

        return existingFeedItemIds;
      }
    }

    public void MarkRead(int userAccountId, IEnumerable<int> feedItemIds) {
      if (feedItemIds == null) {
        throw new ArgumentNullException("feedItemIds");
      }

      var feedItemIdsList = feedItemIds.ToList();

      if (feedItemIdsList.Count == 0) {
        return;
      }

      DateTime now = DateTime.UtcNow;

      using (var db = CreateOpenedConnection()) {
        db.Execute(
          " insert into UserReadFeedItem" +
          " (" +
          "   UserAccountId," +
          "   FeedItemId," +
          "   DateCreated" +
          " )" +
          " select" +
          "   @UserAccountId," +
          "   @FeedItemId," +
          "   @DateCreated" +
          " where not exists (select Id from UserReadFeedItem urfi where urfi.UserAccountId = @UserAccountId and urfi.FeedItemId = @FeedItemId)",
          feedItemIdsList.Select(
            feedItemId =>
            new {
              UserAccountId = userAccountId,
              FeedItemId = feedItemId,
              DateCreated = now,
            }));
      }
    }

    public void MarkUnread(int userAccountId, IEnumerable<int> feedItemIds) {
      if (feedItemIds == null) {
        throw new ArgumentNullException("feedItemIds");
      }

      var feedItemIdsList = feedItemIds.ToList();

      if (feedItemIdsList.Count == 0) {
        return;
      }

      using (var db = CreateOpenedConnection()) {
        db.Execute(
          " delete from UserReadFeedItem" +
          " where 1 = 1" +
          "   and UserAccountId = @UserAccountId" +
          "   and FeedItemId in @FeedItemIds",
          new {
            UserAccountId = userAccountId,
            FeedItemIds = feedItemIdsList,
          });
      }
    }

    public void MarkStarred(int userAccountId, IEnumerable<int> feedItemIds) {
      if (feedItemIds == null) {
        throw new ArgumentNullException("feedItemIds");
      }

      var feedItemIdsList = feedItemIds.ToList();

      if (feedItemIdsList.Count == 0) {
        return;
      }

      DateTime now = DateTime.UtcNow;

      using (var db = CreateOpenedConnection()) {
        db.Execute(
          " insert into UserStarredFeedItem" +
          " (" +
          "   UserAccountId," +
          "   FeedItemId," +
          "   DateCreated" +
          " )" +
          " select" +
          "   @UserAccountId," +
          "   @FeedItemId," +
          "   @DateCreated" +
          " where not exists (select Id from UserStarredFeedItem urfi where urfi.UserAccountId = @UserAccountId and urfi.FeedItemId = @FeedItemId)",
          feedItemIdsList.Select(
            feedItemId =>
            new {
              UserAccountId = userAccountId,
              FeedItemId = feedItemId,
              DateCreated = now,
            }));
      }
    }

    public void MarkUnstarred(int userAccountId, IEnumerable<int> feedItemIds) {
      if (feedItemIds == null) {
        throw new ArgumentNullException("feedItemIds");
      }

      var feedItemIdsList = feedItemIds.ToList();

      if (feedItemIdsList.Count == 0) {
        return;
      }

      using (var db = CreateOpenedConnection()) {
        db.Execute(
          " delete from UserStarredFeedItem" +
          " where 1 = 1" +
          "   and UserAccountId = @UserAccountId" +
          "   and FeedItemId in @FeedItemIds",
          new {
            UserAccountId = userAccountId,
            FeedItemIds = feedItemIdsList,
          });
      }
    }

  }

}
