using System.Collections.Generic;
using JustReadIt.Core.Domain.Query;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class SubscriptionsQueryDao : DapperRepository, ISubscriptionsQueryDao {

    public SubscriptionsQueryDao(string connectionString)
      : base(connectionString) {
    }

    public IEnumerable<QueryModel.GroupedSubscription> GetGroupedSubscriptions(int userAccountId) {
      using (var db = CreateOpenedConnection()) {
        var query =
          db.Query<QueryModel.GroupedSubscription>(
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

    public IEnumerable<QueryModel.FeedItem> GetFeedItems(int userAccountId, int subscriptionId) {
      using (var db = CreateOpenedConnection()) {
        var query =
          db.Query<QueryModel.FeedItem>(
            " select" +
            "   fi.Id as Id," +
            "   fi.Title as Title," +
            "   case when fi.DatePublished is not null then fi.DatePublished else fi.DateCreated end as [Date]," +
            "   fi.Summary as Summary" +
            " from FeedItem fi" +
            " join UserFeedGroupFeed ufgf on ufgf.FeedId = fi.FeedId" +
            " join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            " where ufgf.Id = @SubscriptionId" +
            " order by fi.DatePublished desc, fi.DateCreated desc, fi.Id desc",
            new {
              UserAccountId = userAccountId,
              SubscriptionId = subscriptionId,
            });

        return query;
      }
    }

  }

}
