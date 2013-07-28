using JustReadIt.Core.Common;

namespace JustReadIt.Core.Services.Feeds {

  public class FeedMetadata {

    private string _feedUrl;
    private string _feedTitle;
    private string _siteUrl;

    public string FeedUrl {
      get { return _feedUrl; }
      set { _feedUrl = value.TrimmedOrNull(); }
    }

    public string FeedTitle {
      get { return _feedTitle; }
      set { _feedTitle = value.TrimmedOrNull(); }
    }

    public string SiteUrl {
      get { return _siteUrl; }
      set { _siteUrl = value.TrimmedOrNull(); }
    }

  }

}
