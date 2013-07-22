using System;

namespace JustReadIt.Core.Domain.Repositories {

  public interface IEmailVerificationTokenRepository {

    void Add(int userAccountId, Guid token);

    bool IsTokenValid(Guid token, out int? userAccountId);

    void MarkTokenAsUsed(Guid token);

  }

}
