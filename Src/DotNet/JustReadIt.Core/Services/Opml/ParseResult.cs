using System;
using System.Collections.Generic;

namespace JustReadIt.Core.Services.Opml {

  public class ParseResult {

    private readonly IEnumerable<FeedGroup> _feedGroups;
    private readonly IEnumerable<Feed> _uncategorizedFeeds;

    public ParseResult(IEnumerable<FeedGroup> feedGroups, IEnumerable<Feed> uncategorizedFeeds) {
      if (feedGroups == null) {
        throw new ArgumentNullException("feedGroups");
      }

      if (uncategorizedFeeds == null) {
        throw new ArgumentNullException("uncategorizedFeeds");
      }

      _feedGroups = feedGroups;
      _uncategorizedFeeds = uncategorizedFeeds;
    }

    public IEnumerable<FeedGroup> FeedGroups {
      get { return _feedGroups; }
    }

    public IEnumerable<Feed> UncategorizedFeeds {
      get { return _uncategorizedFeeds; }
    }

  }

}
