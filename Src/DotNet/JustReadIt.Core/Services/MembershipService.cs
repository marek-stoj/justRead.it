using System.Diagnostics;
using ImmRafSoft.Security;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.Services {

  public class MembershipService : IMembershipService {

    private readonly IUserAccountRepository _userAccountRepository;
    private readonly ICryptoUtils _cryptoUtils;
    private readonly IMailingService _mailingService;

    public MembershipService(IUserAccountRepository userAccountRepository, ICryptoUtils cryptoUtils, IMailingService mailingService) {
      Guard.ArgNotNull(userAccountRepository, "userAccountRepository");
      Guard.ArgNotNull(cryptoUtils, "cryptoUtils");
      Guard.ArgNotNull(mailingService, "mailingService");

      _userAccountRepository = userAccountRepository;
      _cryptoUtils = cryptoUtils;
      _mailingService = mailingService;
    }

    public CreateUserResult CreateUser(string emailAddress, string password) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");
      Guard.ArgNotNullNorEmpty(password, "password");

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        if (_userAccountRepository.UserWithEmailAddressExists(emailAddress)) {
          return CreateUserResult.Failed_EmailAddressExists;
        }

        var userAccount =
          new UserAccount {
            EmailAddress = emailAddress,
            PasswordHash = _cryptoUtils.ComputePasswordHash(password),
          };

        _userAccountRepository.Add(userAccount);

        Debug.Assert(userAccount.Id > 0);

        _mailingService.SendVerificationEmail(
          userAccount.Id,
          emailAddress);

        ts.Complete();

        return CreateUserResult.Success;
      }
    }

    public bool ValidateUser(string emailAddress, string password) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");
      Guard.ArgNotNullNorEmpty(password, "password");

      UserAccount userAccount =
        _userAccountRepository.FindUserAccountByEmailAddress(emailAddress);

      if (userAccount == null || !userAccount.IsEmailVerified) {
        return false;
      }

      string passwordHash = _cryptoUtils.ComputePasswordHash(password);

      return passwordHash == userAccount.PasswordHash;
    }

    public int? FindUserAccountId(string emailAddress) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");

      int? userAccountId =
        _userAccountRepository.FindUserAccountIdByEmailAddress(emailAddress);

      return userAccountId;
    }

  }

}
