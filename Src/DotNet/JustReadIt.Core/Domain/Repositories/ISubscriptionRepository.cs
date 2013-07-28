using System;
using System.Collections.Generic;

namespace JustReadIt.Core.Domain.Repositories {

  public interface ISubscriptionRepository {

    IEnumerable<Subscription> GetAll(int userAccountId, DateTime? dateCreatedSince);

  }

}
