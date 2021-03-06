﻿using System;
using System.Diagnostics;
using System.Transactions;
using ImmRafSoft.Security;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Resources;

namespace JustReadIt.Core.Services {

  public class MembershipService : IMembershipService {

    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IUserFeedGroupRepository _userFeedGroupRepository;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
    private readonly ICryptoUtils _cryptoUtils;
    private readonly IMailingService _mailingService;

    public MembershipService(IUserAccountRepository userAccountRepository, IUserFeedGroupRepository userFeedGroupRepository, IEmailVerificationTokenRepository emailVerificationTokenRepository, ICryptoUtils cryptoUtils, IMailingService mailingService) {
      Guard.ArgNotNull(userAccountRepository, "userAccountRepository");
      Guard.ArgNotNull(userFeedGroupRepository, "userFeedGroupRepository");
      Guard.ArgNotNull(emailVerificationTokenRepository, "_emailVerificationTokenRepository");
      Guard.ArgNotNull(cryptoUtils, "cryptoUtils");
      Guard.ArgNotNull(mailingService, "mailingService");

      _userAccountRepository = userAccountRepository;
      _userFeedGroupRepository = userFeedGroupRepository;
      _emailVerificationTokenRepository = emailVerificationTokenRepository;
      _cryptoUtils = cryptoUtils;
      _mailingService = mailingService;
    }

    public CreateUserResult CreateUser(string emailAddress, string password) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");
      Guard.ArgNotNullNorEmpty(password, "password");

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
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

        var uncategorizedFeedGroup =
          new UserFeedGroup {
            UserAccountId = userAccount.Id,
            SpecialType = SpecialUserFeedGroupType.Uncategorized,
            Title = CommonResources.UncategorizedFeedGroupTitle,
          };

        _userFeedGroupRepository.Add(uncategorizedFeedGroup);

        Debug.Assert(uncategorizedFeedGroup.Id > 0);

        _mailingService.SendVerificationEmail(
          userAccount.Id,
          emailAddress);

        ts.Complete();

        return CreateUserResult.Success;
      }
    }

    public bool ValidateUser(string emailAddress, string password, out int userAccountId) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");
      Guard.ArgNotNullNorEmpty(password, "password");

      UserAccount userAccount =
        _userAccountRepository.FindByEmailAddress(emailAddress);

      if (userAccount == null || !userAccount.IsEmailAddressVerified) {
        userAccountId = -1;

        return false;
      }

      userAccountId = userAccount.Id;

      string passwordHash = _cryptoUtils.ComputePasswordHash(password);

      return passwordHash == userAccount.PasswordHash;
    }

    public int? FindUserAccountId(string emailAddress) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");

      int? userAccountId =
        _userAccountRepository.FindIdByEmailAddress(emailAddress);

      return userAccountId;
    }

    public void VerifyEmailAddress(int userAccountId, Guid emailVerificationToken) {
      Guard.ArgNotEmpty(emailVerificationToken, "emailVerificationToken");

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        _emailVerificationTokenRepository.MarkTokenAsUsed(emailVerificationToken);
        _userAccountRepository.VerifyEmailAddress(userAccountId);

        ts.Complete();
      }
    }

  }

}
