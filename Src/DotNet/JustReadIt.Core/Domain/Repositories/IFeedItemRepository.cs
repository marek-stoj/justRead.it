namespace JustReadIt.Core.Domain.Repositories {

  public interface IFeedItemRepository {

    void Add(FeedItem feedItem);

    bool Exists(string url);

  }

}
