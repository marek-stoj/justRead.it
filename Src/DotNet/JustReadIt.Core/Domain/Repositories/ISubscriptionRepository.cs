using System;
using System.Collections.Generic;

namespace JustReadIt.Core.Domain.Repositories {

  public interface ISubscriptionRepository {

    IEnumerable<Subscription> Query(int userAccountId, DateTime? dateCreatedSince);

    Subscription FindById(int id);

    bool Exists(int userAccountId, int id);

    int? FindIdByFeedUrl(int userAccountId, string feedUrl);

    /// <param name="subscription"></param>
    /// <param name="groupId">Optional group id. If not given - the subscription will be added to the special Uncategorized group.</param>
    void Add(Subscription subscription, int? groupId = null);

    bool Delete(int userAccountId, int id);

    bool UpdateTitle(int userAccountId, int id, string title);

    bool IsSubscribedToFeed(int userAccountId, int feedId);

    void MarkAllItemsAsRead(int userAccountId, int id);

  }

}
