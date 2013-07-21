using System.Transactions;

namespace JustReadIt.Core.Services {

  public static class TransactionUtils {

    public static TransactionScope CreateTransactionScope() {
      return
        new TransactionScope(
          TransactionScopeOption.Required,
          new TransactionOptions {
            IsolationLevel = IsolationLevel.ReadCommitted
          });
    }

  }

}
