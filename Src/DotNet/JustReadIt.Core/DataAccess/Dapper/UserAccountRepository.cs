using System.Linq;
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
            "   case when exists(select id from UserAccount where Id = 1)" +
            "     then 1" +
            "     else 0" +
            "   end",
            new { Id = id })
            .Single();

        return existsInt == 1;
      }

    }

  }

}
