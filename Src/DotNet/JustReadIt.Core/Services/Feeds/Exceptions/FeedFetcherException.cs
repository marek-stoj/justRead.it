using System;

namespace JustReadIt.Core.Services.Feeds.Exceptions {

  public abstract class FeedFetcherException : Exception {

    protected FeedFetcherException(string message, Exception innerException = null)
      : base(message, innerException) {
    }

  }

}
