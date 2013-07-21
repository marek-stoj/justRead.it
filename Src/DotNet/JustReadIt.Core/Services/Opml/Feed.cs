using JustReadIt.Core.Common;

namespace JustReadIt.Core.Services.Opml {

  public class Feed {

    public Feed(string title, string feedUrl, string siteUrl = null) {
      Guard.ArgNotNullNorEmpty(title, "title");
      Guard.ArgNotNullNorEmpty(feedUrl, "feedUrl");

      Title = title;
      FeedUrl = feedUrl;
      SiteUrl = siteUrl;
    }

    public string Title { get; private set; }

    public string FeedUrl { get; private set; }

    public string SiteUrl { get; private set; }

  }

}
