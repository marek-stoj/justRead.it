using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Services.Feeds {

  public class FeedParser : IFeedParser {

    private SyndicationFeed _feed;

    public void Parse(string feedContent) {
      Guard.ArgNotNullNorEmpty(feedContent, "feedContent");

      using (StringReader stringReader = new StringReader(feedContent))
      using (XmlReader xmlReader = XmlReader.Create(stringReader)) {
        _feed = SyndicationFeed.Load(xmlReader);
      }

      if (_feed == null) {
        throw new FeedParserException("Couldn't parse the feed.");
      }
    }

    public FeedMetadata GetMetadata() {
      EnsureFeedIsParsed();

      return
        new FeedMetadata {
          FeedTitle = _feed.Title.Text,
          FeedUrl = "Feed Url",
          SiteUrl = "Site Url",
        };
    }

    private void EnsureFeedIsParsed() {
      if (_feed != null) {
        return;
      }

      throw new FeedParserException("The feed has to be parsed first.");
    }

  }

}
