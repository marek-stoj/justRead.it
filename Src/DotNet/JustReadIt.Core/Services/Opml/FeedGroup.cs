using System.Collections.Generic;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Services.Opml {

  public class FeedGroup {

    private List<Feed> _feeds;

    public FeedGroup(string title) {
      Guard.ArgNotNullNorEmpty(title, "title");

      Title = title;
    }

    public string Title { get; private set; }

    public List<Feed> Feeds {
      get { return _feeds ?? (_feeds = new List<Feed>()); }
      set { _feeds = value; }
    }

  }

}
