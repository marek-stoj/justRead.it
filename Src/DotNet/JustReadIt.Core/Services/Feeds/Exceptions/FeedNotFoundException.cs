namespace JustReadIt.Core.Services.Feeds.Exceptions {

  public class FeedNotFoundException : FeedFetcherException {

    public FeedNotFoundException(string feedUrl)
      : base(string.Format("No feed was found at URL: '{0}'.", feedUrl), null) {
    }

  }

}
