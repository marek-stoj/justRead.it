using System;

namespace JustReadIt.Core.Services.Feeds {

  public class FeedParserException : Exception {

    public FeedParserException(string message, Exception innerException = null)
      : base(message, innerException) {
    }

  }

}
