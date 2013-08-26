using System.Collections.Generic;
using JustReadIt.Core.Domain.Query.Model;

namespace JustReadIt.Core.Services {

  // TODO IMM HI: maybe we should get rid of userAccountId here and use Thread.CurrentPrincipal (or HttpContext.Current.User)
  public interface ISubscriptionsService {

    /// <returns>Existing or newly created subscription id.</returns>
    int Subscribe(int userAccountId, string url, string groupTitle);

    IEnumerable<GroupedSubscription> GetGroupedSubscriptions(int userAccountId);

    IEnumerable<FeedItem> GetFeedItems(int userAccountId, int subscriptionId, bool returnRead);

    void MarkAllItemsAsRead(int userAccountId, int subscriptionId);

  }

}
