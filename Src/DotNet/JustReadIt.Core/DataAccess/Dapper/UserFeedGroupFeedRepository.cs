using System;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.DataAccess.Dapper.Exceptions;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class UserFeedGroupFeedRepository : DapperRepository, IUserFeedGroupFeedRepository {

    public UserFeedGroupFeedRepository(string connectionString)
      : base(connectionString) {
    }

    public int? FindFeedGroupFeedId(int userFeedGroupId, int feedId) {
      using (var db = CreateOpenedConnection()) {
        int? feedGroupId =
          db.Query<int?>(
            " select" +
            "   Id" +
            " from UserFeedGroupFeed" +
            " where UserFeedGroupId = @UserFeedGroupId" +
            "   and FeedId = @FeedId",
            new {
              UserFeedGroupId = userFeedGroupId,
              FeedId = feedId,
            })
            .SingleOrDefault();

        return feedGroupId;
      }
    }

    public void Add(UserFeedGroupFeed userFeedGroupFeed) {
      Guard.ArgNotNull(userFeedGroupFeed, "userFeedGroupFeed");

      if (userFeedGroupFeed.Id != 0) {
        throw new ArgumentException("Non-transient entity can't be added. Id must be 0.", "userFeedGroupFeed");
      }

      userFeedGroupFeed.DateCreated = DateTime.UtcNow;

      using (var db = CreateOpenedConnection()) {
        int userFeedGroupFeedId =
          db.Query<int>(
            " insert into UserFeedGroupFeed" +
            " (UserFeedGroupId, FeedId, DateCreated)" +
            " values" +
            " (@UserFeedGroupId, @FeedId, @DateCreated);" +
            " " +
            " select cast(scope_identity() as int);",
            new {
              UserFeedGroupId = userFeedGroupFeed.UserFeedGroupId,
              FeedId = userFeedGroupFeed.FeedId,
              DateCreated = userFeedGroupFeed.DateCreated,
            })
            .Single();

        if (userFeedGroupFeedId <= 0) {
          throw new IdentityInsertFailedException();
        }

        userFeedGroupFeed.Id = userFeedGroupFeedId;
      }
    }

  }

}
