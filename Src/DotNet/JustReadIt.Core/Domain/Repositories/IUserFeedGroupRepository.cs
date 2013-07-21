namespace JustReadIt.Core.Domain.Repositories {

  public interface IUserFeedGroupRepository {

    int? FindSpecialFeedGroupId(int userAccountId, SpecialUserFeedGroupType specialType);

    int? FindGroupIdByTitle(int userAccountId, string title);

    void Add(UserFeedGroup userFeedGroup);

  }

}
