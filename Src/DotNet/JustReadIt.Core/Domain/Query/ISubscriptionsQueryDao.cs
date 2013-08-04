using System.Collections.Generic;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

namespace JustReadIt.Core.Domain.Query {

  public interface ISubscriptionsQueryDao {

    IEnumerable<QueryModel.Subscription> GetAll(int userAccountId);

  }

}
