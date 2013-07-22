using System;

namespace JustReadIt.WebApp.Core.Common {

  public class InternalException : Exception {

    public InternalException(string message, Exception innerException = null)
      : base(message, innerException) {
    }

  }

}
