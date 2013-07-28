using System;

namespace JustReadIt.Core.Services {

  public interface IMembershipService {

    CreateUserResult CreateUser(string emailAddress, string password);

    bool ValidateUser(string emailAddress, string password, out int userAccountId);

    int? FindUserAccountId(string emailAddress);

    void VerifyEmailAddress(int userAccountId, Guid emailVerificationToken);

  }

}
