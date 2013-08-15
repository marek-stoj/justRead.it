namespace JustReadIt.Core.Domain.Repositories {

  public interface IUserAccountRepository {

    bool UserAccountExists(int id);

    bool UserWithEmailAddressExists(string emailAddress);

    void Add(UserAccount userAccount);

    UserAccount FindByEmailAddress(string emailAddress);

    int? FindIdByEmailAddress(string emailAddress);

    UserAccount FindByAuthProviderId(string authProviderId);

    void VerifyEmailAddress(int id);

    void SetAuthProviderId(int id, string authProviderId);

  }

}
