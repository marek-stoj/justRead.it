using System;
using System.Transactions;
using ImmRafSoft.Net;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Resources;

namespace JustReadIt.Core.Services {

  public class MailingService : IMailingService {

    private readonly IMailer _mailer;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
    private readonly string _from;

    public MailingService(IMailer mailer, IEmailVerificationTokenRepository emailVerificationTokenRepository, string from) {
      Guard.ArgNotNull(mailer, "mailer");
      Guard.ArgNotNull(emailVerificationTokenRepository, "emailVerificationTokenRepository");
      Guard.ArgNotNullNorEmpty(from, "from");

      _mailer = mailer;
      _emailVerificationTokenRepository = emailVerificationTokenRepository;
      _from = from;
    }

    public void SendVerificationEmail(int userAccountId, string emailAddress) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        Guid emailVerificationToken = Guid.NewGuid();

        _emailVerificationTokenRepository
          .Add(userAccountId, emailVerificationToken);

        // TODO IMM HI: parametrize MailingResources

        _mailer.SendEmail(
          _from,
          emailAddress,
          MailingResources.MailSubject_VerificationEmail,
          MailingResources.MailBodyHtml_VerificationEmailBodyHtml
            .Replace("${EmailVerificationToken}", emailVerificationToken.ToString()));

        ts.Complete();
      }
    }

  }

}
