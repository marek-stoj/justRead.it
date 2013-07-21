using System;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.DataAccess.Dapper.Exceptions;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class UserFeedGroupRepository : DapperRepository, IUserFeedGroupRepository {

    public UserFeedGroupRepository(string connectionString)
      : base(connectionString) {
    }

    public int? FindSpecialFeedGroupId(int userAccountId, SpecialUserFeedGroupType specialType) {
      using (var db = CreateOpenedConnection()) {
        int? specialFeedGroupId =
          db.Query<int?>(
            " select" +
            "   Id" +
            " from UserFeedGroup" +
            " where UserAccountId = @UserAccountId" +
            "   and SpecialType = @SpecialType",
            new {
              UserAccountId = userAccountId,
              SpecialType = specialType.ToString(),
            })
            .SingleOrDefault();

        return specialFeedGroupId;
      }
    }

    public int? FindGroupIdByTitle(int userAccountId, string title) {
      using (var db = CreateOpenedConnection()) {
        int? feedGroupId =
          db.Query<int?>(
            " select" +
            "   Id" +
            " from UserFeedGroup" +
            " where UserAccountId = @UserAccountId" +
            "   and Title = @Title",
            new {
              UserAccountId = userAccountId,
              Title = title,
            })
            .SingleOrDefault();

        return feedGroupId;
      }
    }

    public void Add(UserFeedGroup userFeedGroup) {
      Guard.ArgNotNull(userFeedGroup, "userFeedGroup");

      if (userFeedGroup.Id != 0) {
        throw new ArgumentException("Non-transient entity can't be added. Id must be 0.", "userFeedGroup");
      }

      using (var db = CreateOpenedConnection()) {
        int userFeedGroupId =
          db.Query<int>(
            " insert into UserFeedGroup" +
            " (UserAccountId, SpecialType, Title)" +
            " values" +
            " (@UserAccountId, @SpecialType, @Title)" +
            " " +
            " select cast(scope_identity() as int);",
            new {
              UserAccountId = userFeedGroup.UserAccountId,
              SpecialType = userFeedGroup.SpecialType.HasValue ? userFeedGroup.SpecialType.Value.ToString() : null,
              Title = userFeedGroup.Title,
            })
            .Single();

        if (userFeedGroupId <= 0) {
          throw new IdentityInsertFailedException();
        }

        userFeedGroup.Id = userFeedGroupId;
      }
    }

  }

}
