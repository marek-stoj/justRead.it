using System.Collections.Generic;
using JustReadIt.Core.Domain.Query;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class SubscriptionQueryDao : DapperRepository, ISubscriptionQueryDao {

    public SubscriptionQueryDao(string connectionString)
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
            "     f.SiteUrl as SiteUrl," +
            "     (" +
            "       select" +
            "         count(*)" +
            "       from FeedItem fi" +
            "       where 1 = 1" +
            "         and fi.FeedId = ufgf.FeedId" +
            "         and not exists" +
            "             (" +
            "               select urfi.Id" +
            "               from UserReadFeedItem urfi" +
            "               where 1 = 1" +
            "                 and urfi.UserAccountId = @UserAccountId" +
            "                 and urfi.FeedItemId = fi.Id" +
            "             )" +
            "     ) as UnreadItemsCount" +
            "   from UserFeedGroupFeed ufgf" +
            "   join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            "   join Feed f on f.Id = ufgf.FeedId" +
            "   where 1 = 1" +
            "     and ufg.UserAccountId = @UserAccountId" +
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

    public IEnumerable<QueryModel.FeedItem> GetFeedItems(int userAccountId, int subscriptionId, bool returnRead) {
      using (var db = CreateOpenedConnection()) {
        var query =
          db.Query<QueryModel.FeedItem>(
            " with FeedItems as" +
            " (" +
            "   select" +
            "     fi.Id as Id," +
            "     fi.FeedId as FeedId," +
            "     fi.Title as Title," +
            "     fi.Url as Url," +
            "     case when fi.DatePublished is not null then fi.DatePublished else fi.DateCreated end as [Date]," +
            "     fi.Summary as Summary," +
            "     (" +
            "       select case when" +
            "         exists" +
            "         (" +
            "           select urfi.Id" +
            "           from UserReadFeedItem urfi" +
            "           where 1 = 1" +
            "             and urfi.UserAccountId = @UserAccountId" +
            "             and urfi.FeedItemId = fi.Id)" +
            "       then 1" +
            "       else 0" +
            "       end" +
            "     ) as IsRead" +
            "   from FeedItem fi" +
            "   join UserFeedGroupFeed ufgf on ufgf.FeedId = fi.FeedId" +
            "   join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            "   where 1 = 1" +
            "     and ufg.UserAccountId = @UserAccountId" +
            "     and ufgf.Id = @SubscriptionId" +
            " )" +
            " select" +
            "   *" +
            " from FeedItems fi" +
            " where 1 = 1" +
            (!returnRead ? "and fi.IsRead = 0" : "") +
            " order by fi.[Date] desc, fi.Id desc",
            new {
              UserAccountId = userAccountId,
              SubscriptionId = subscriptionId,
            });

        return query;
      }
    }

  }

}
