using System.Collections.Generic;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

namespace JustReadIt.Core.Domain.Query {

  public interface ISubscriptionsQueryDao {

    IEnumerable<QueryModel.GroupedSubscription> GetGroupedSubscriptions(int userAccountId);

    IEnumerable<QueryModel.FeedItem> GetFeedItems(int userAccountId, int subscriptionId, bool returnRead);

  }

}
