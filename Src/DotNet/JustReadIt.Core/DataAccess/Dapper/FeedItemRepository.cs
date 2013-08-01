using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class FeedItemRepository : DapperRepository, IFeedItemRepository {

    public FeedItemRepository(string connectionString)
      : base(connectionString) {
    }

  }

}
