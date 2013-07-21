namespace JustReadIt.Core.Domain.Repositories {

  public interface IUserFeedGroupFeedRepository {

    int? FindFeedGroupFeedId(int userFeedGroupId, int feedId);

    void Add(UserFeedGroupFeed userFeedGroupFeed);

  }

}
