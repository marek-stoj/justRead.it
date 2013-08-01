using System.Collections.Generic;

namespace JustReadIt.Core.Domain.Repositories {

  public interface IFeedItemRepository {

    IEnumerable<FeedItem> Query(int userAccountId, int maxCount, FeedItemFilter feedItemFilter);

    void Add(FeedItem feedItem);

    bool Exists(string url);

    FeedItem FindById(int id);

  }

}
