using System.Collections.Generic;
using JustReadIt.Core.Domain.Query;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class SubscriptionsQueryDao : DapperRepository, ISubscriptionsQueryDao {

    public SubscriptionsQueryDao(string connectionString)
      : base(connectionString) {
    }

    public IEnumerable<QueryModel.Subscription> GetAll(int userAccountId) {
      using (var db = CreateOpenedConnection()) {
        var query =
          db.Query<QueryModel.Subscription>(
            " with Subscriptions as" +
            " (" +
            "   select" +
            "     ufg.Id as GroupId," +
            "     ufg.Title as GroupTitle," +
            "     ufgf.Id as Id," +
            "     f.Id as FeedId," +
            "     case when ufgf.CustomTitle is not null then ufgf.CustomTitle else f.Title end as Title," +
            "     f.SiteUrl as SiteUrl" +
            "   from UserFeedGroupFeed ufgf" +
            "   join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            "   join Feed f on f.Id = ufgf.FeedId" +
            " )" +
            " select" +
            "   *" +
            " from Subscriptions s" +
            " order by s.GroupTitle asc, s.GroupId asc, s.Title asc, s.Id asc",
            new {
              UserAccountId = userAccountId,
            });

        return query;
      }
    }

  }

}
