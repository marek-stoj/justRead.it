namespace JustReadIt.Core.Services {

  public interface IMembershipService {

    CreateUserResult CreateUser(string emailAddress, string password);

    bool ValidateUser(string emailAddress, string password);

    int? FindUserAccountId(string emailAddress);

  }

}
