namespace JustReadIt.Core.Domain.Repositories {

  // TODO IMM HI: maybe we could just leave ISubscriptionRepository?
  public interface IUserFeedGroupFeedRepository {

    int? FindFeedGroupFeedId(int userFeedGroupId, int feedId);

    void Add(UserFeedGroupFeed userFeedGroupFeed);

  }

}
