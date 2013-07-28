using System;

namespace JustReadIt.Core.DataAccess.Dapper.Exceptions {

  public class InternalException : Exception {

    public InternalException(string message, Exception innerException = null)
      : base(message, innerException) {
    }

  }

}
