using System.Collections.Generic;
using JustReadIt.Core.DataAccess.Dapper;
using JustReadIt.Core.Domain;

namespace JustReadIt.Core.DataAccess {

  public class DapperFeedRepository : DapperRepository, IFeedRepository {

    public DapperFeedRepository(string connectionString)
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

  }

}
