using System;
using System.Collections.Generic;

namespace JustReadIt.Core.Domain.Repositories {

  public interface ISubscriptionRepository {

    IEnumerable<Subscription> Query(int userAccountId, DateTime? dateCreatedSince);

    Subscription FindById(int id);

    bool Exists(int userAccountId, int id);

    int? FindIdByFeedUrl(int userAccountId, string feedUrl);

    void Add(Subscription subscription);

    bool Delete(int userAccountId, int id);

    bool UpdateTitle(int userAccountId, int id, string title);

    bool IsSubscribedToFeed(int userAccountId, int feedId);

    void MarkAllItemsAsRead(int userAccountId, int id);

  }

}
