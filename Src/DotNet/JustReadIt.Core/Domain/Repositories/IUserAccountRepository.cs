namespace JustReadIt.Core.Domain.Repositories {

  public interface IUserAccountRepository {

    bool UserAccountExists(int id);

    bool UserWithEmailAddressExists(string emailAddress);

    void Add(UserAccount userAccount);

    UserAccount FindUserAccountByEmailAddress(string emailAddress);

    int? FindUserAccountIdByEmailAddress(string emailAddress);

    void VerifyEmailAddress(int userAccountId);

  }

}
