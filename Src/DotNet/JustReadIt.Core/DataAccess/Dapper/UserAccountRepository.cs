using System;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class UserAccountRepository : DapperRepository, IUserAccountRepository {

    public UserAccountRepository(string connectionString)
      : base(connectionString) {
    }

    public bool UserAccountExists(int id) {
      using (var db = CreateOpenedConnection()) {
        int existsInt =
          db.Query<int>(
            " select" +
            "   case when exists(select ua.Id from UserAccount ua where ua.Id = @Id)" +
            "     then 1" +
            "     else 0" +
            "   end",
            new { Id = id })
            .Single();

        return existsInt == 1;
      }

    }

    public bool UserWithEmailAddressExists(string emailAddress) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");

      using (var db = CreateOpenedConnection()) {
        int existsInt =
          db.Query<int>(
            " select" +
            "   case when exists(select ua.Id from UserAccount ua where ua.EmailAddress = @EmailAddress)" +
            "     then 1" +
            "     else 0" +
            "   end ",
            new {
              EmailAddress = emailAddress,
            })
            .Single();

        return existsInt == 1;
      }
    }

    public void Add(UserAccount userAccount) {
      Guard.ArgNotNull(userAccount, "userAccount");

      if (userAccount.Id != 0) {
        throw new ArgumentException("Non-transient entity can't be added. Id must be 0.", "userAccount");
      }

      using (var db = CreateOpenedConnection()) {
        DateTime now = DateTime.UtcNow;

        int userAccountId =
          db.Query<int>(
            " insert into UserAccount" +
            " (DateCreated, EmailAddress, IsEmailAddressVerified, AuthProviderId, PasswordHash)" +
            " values" +
            " (@DateCreated, @EmailAddress, @IsEmailAddressVerified, @AuthProviderId, @PasswordHash);" +
            " " +
            " select cast(scope_identity() as int);",
            new {
              DateCreated = now,
              EmailAddress = userAccount.EmailAddress,
              IsEmailAddressVerified = false,
              AuthProviderId = userAccount.AuthProviderId,
              PasswordHash = userAccount.PasswordHash,
            })
            .Single();

        userAccount.Id = userAccountId;
      }
    }

    public UserAccount FindByEmailAddress(string emailAddress) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");

      using (var db = CreateOpenedConnection()) {
        UserAccount userAccount =
          db.Query<UserAccount>(
            " select * from UserAccount ua" +
            " where 1 = 1" +
            "   and ua.EmailAddress = @EmailAddress",
            new {
              EmailAddress = emailAddress,
            })
            .SingleOrDefault();

        return userAccount;
      }
    }

    public int? FindIdByEmailAddress(string emailAddress) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");

      using (var db = CreateOpenedConnection()) {
        int? userAccountId =
          db.Query<int?>(
            " select Id from UserAccount ua" +
            " where 1 = 1" +
            "   and ua.EmailAddress = @EmailAddress",
            new {
              EmailAddress = emailAddress,
            })
            .SingleOrDefault();

        return userAccountId;
      }
    }

    public UserAccount FindByAuthProviderId(string authProviderId) {
      Guard.ArgNotNullNorEmpty(authProviderId, "authProviderId");

      using (var db = CreateOpenedConnection()) {
        UserAccount userAccount =
          db.Query<UserAccount>(
            " select" +
            "  *" +
            " from UserAccount ua" +
            " where 1 = 1" +
            "   and ua.AuthProviderId = @AuthProviderId",
            new {
              AuthProviderId = authProviderId
            }).SingleOrDefault();

        return userAccount;
      }
    }

    public void VerifyEmailAddress(int id) {
      using (var db = CreateOpenedConnection()) {
        db.Execute(
          " update UserAccount" +
          " set IsEmailAddressVerified = @IsEmailAddressVerified" +
          " where 1 = 1" +
          "   and Id = @UserAccountId",
          new {
            UserAccountId = id,
            IsEmailAddressVerified = true,
          });
      }
    }

    public void SetAuthProviderId(int id, string authProviderId) {
      Guard.ArgNotNullNorEmpty(authProviderId, "authProviderId");

      using (var db = CreateOpenedConnection()) {
        db.Execute(
          " update UserAccount" +
          " set AuthProviderId = @AuthProviderId" +
          " where 1 = 1" +
          "   and Id = @UserAccountId",
          new {
            UserAccountId = id,
            AuthProviderId = authProviderId,
          });
      }
    }

  }

}
