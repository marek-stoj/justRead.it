using System;
using System.Data;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class EmailVerificationTokenRepository : DapperRepository, IEmailVerificationTokenRepository {

    public EmailVerificationTokenRepository(string connectionString)
      : base(connectionString) {
    }

    public void Add(int userAccountId, Guid token) {
      Guard.ArgNotEmpty(token, "token");

      using (var db = CreateOpenedConnection()) {
        var emailVerificationToken =
          new EmailVerificationToken {
            UserAccountId = userAccountId,
            DateCreated = DateTime.UtcNow,
            Token = token,
            IsUsed = false,
          };

        db.Execute(
          " insert into EmailVerificationToken" +
          " (UserAccountId, DateCreated, Token, IsUsed)" +
          " values" +
          " (@UserAccountId, @DateCreated, @Token, @IsUsed)",
          new {
            UserAccountId = emailVerificationToken.UserAccountId,
            DateCreated = emailVerificationToken.DateCreated,
            Token = emailVerificationToken.Token,
            IsUsed = emailVerificationToken.IsUsed,
          });
      }
    }

    public bool IsTokenValid(Guid token, out int? userAccountId) {
      Guard.ArgNotEmpty(token, "token");

      userAccountId = null;

      using (var db = CreateOpenedConnection()) {
        EmailVerificationToken emailVerificationToken =
          DoGetEmailVerificationToken(db, token);

        if (emailVerificationToken == null
            || emailVerificationToken.HasExpired
            || emailVerificationToken.IsUsed) {
          return false;
        }

        userAccountId = emailVerificationToken.UserAccountId;

        return true;
      }
    }

    public void MarkTokenAsUsed(Guid token) {
      Guard.ArgNotEmpty(token, "token");

      using (var db = CreateOpenedConnection()) {
        db.Execute(
          " update EmailVerificationToken" +
          " set IsUsed = @IsUsed" +
          " where Token = @Token",
          new {
            IsUsed = true,
            Token = token,
          });
      }
    }

    private static EmailVerificationToken DoGetEmailVerificationToken(IDbConnection db, Guid token) {
      return
        db.Query<EmailVerificationToken>(
          " select * from EmailVerificationToken" +
          " where Token = @Token",
          new {
            Token = token,
          })
          .SingleOrDefault();
    }

  }

}
