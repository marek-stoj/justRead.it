using System.Collections.Generic;

namespace JustReadIt.Core.Domain.Repositories {

  public interface IFeedItemRepository {

    IEnumerable<FeedItem> Query(int userAccountId, int maxCount, FeedItemFilter feedItemFilter);

    void Add(FeedItem feedItem);

    bool Exists(string url);

    FeedItem FindById(int id);

    string FindUrlById(int id);

    IEnumerable<int> GetAllUnreadIds(int userAccountId);

    IEnumerable<int> GetAllStarredIds(int userAccountId);

    IEnumerable<int> GetExistingFeedItemIds(int userAccountId, IEnumerable<int> feedItemIds);

    void MarkRead(int userAccountId, IEnumerable<int> feedItemIds);

    void MarkUnread(int userAccountId, IEnumerable<int> feedItemIds);

    void MarkStarred(int userAccountId, IEnumerable<int> feedItemIds);

    void MarkUnstarred(int userAccountId, IEnumerable<int> feedItemIds);

  }

}
