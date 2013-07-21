using System;

namespace JustReadIt.Core.DataAccess.Dapper.Exceptions {

  public class IdentityInsertFailedException : Exception {

    public IdentityInsertFailedException()
      : base("Identity of inserted entity couldn't be obtained.") {
    }

  }

}
