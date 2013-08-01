namespace JustReadIt.Core.Domain.Repositories {

  // TODO IMM HI: maybe we could just leave ISubscriptionRepository?
  public interface IUserFeedGroupRepository {

    int? FindSpecialFeedGroupId(int userAccountId, SpecialUserFeedGroupType specialType);

    int? FindGroupIdByTitle(int userAccountId, string title);

    void Add(UserFeedGroup userFeedGroup);

  }

}
