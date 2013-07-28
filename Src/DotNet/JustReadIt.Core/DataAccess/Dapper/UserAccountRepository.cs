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
            "   case when exists(select Id from UserAccount where Id = @Id)" +
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
            "   case when exists(select Id from UserAccount where EmailAddress = @EmailAddress)" +
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
            " (DateCreated, EmailAddress, PasswordHash, IsEmailAddressVerified)" +
            " values" +
            " (@DateCreated, @EmailAddress, @PasswordHash, @IsEmailAddressVerified);" +
            " " +
            " select cast(scope_identity() as int);",
            new {
              DateCreated = now,
              EmailAddress = userAccount.EmailAddress,
              PasswordHash = userAccount.PasswordHash,
              IsEmailAddressVerified = false,
            })
            .Single();

        userAccount.Id = userAccountId;
      }
    }

    public UserAccount FindUserAccountByEmailAddress(string emailAddress) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");

      using (var db = CreateOpenedConnection()) {
        UserAccount userAccount =
          db.Query<UserAccount>(
            " select * from UserAccount" +
            " where EmailAddress = @EmailAddress",
            new {
              EmailAddress = emailAddress,
            })
            .SingleOrDefault();

        return userAccount;
      }
    }

    public int? FindUserAccountIdByEmailAddress(string emailAddress) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");

      using (var db = CreateOpenedConnection()) {
        int? userAccountId =
          db.Query<int?>(
            " select Id from UserAccount" +
            " where EmailAddress = @EmailAddress",
            new {
              EmailAddress = emailAddress,
            })
            .SingleOrDefault();

        return userAccountId;
      }
    }

    public void VerifyEmailAddress(int userAccountId) {
      using (var db = CreateOpenedConnection()) {
        db.Execute(
          " update UserAccount" +
          " set IsEmailAddressVerified = @IsEmailAddressVerified" +
          " where Id = @UserAccountId",
          new {
            IsEmailAddressVerified = true,
            UserAccountId = userAccountId,
          });
      }
    }

  }

}
