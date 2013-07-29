using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Services.Feeds {

  public class FeedParser : IFeedParser {

    public Feed Parse(string feedContent) {
      Guard.ArgNotNullNorEmpty(feedContent, "feedContent");

      SyndicationFeed syndicationFeed;

      using (StringReader stringReader = new StringReader(feedContent))
      using (XmlReader xmlReader = XmlReader.Create(stringReader)) {
        syndicationFeed = SyndicationFeed.Load(xmlReader);
      }

      if (syndicationFeed == null) {
        throw new FeedParserException("Couldn't parse the feed.");
      }

      string feedTitle =
        syndicationFeed.Title != null
          ? syndicationFeed.Title.Text
          : null;

      SyndicationLink feedLink =
        syndicationFeed.Links.FirstOrDefault(
          link => !link.RelationshipType.IsNullOrEmpty() && "self".EqualsOrdinalIgnoreCase(link.RelationshipType));

      string feedUrl = FindAbsoluteUrl(feedLink);

      SyndicationLink siteLink = FindHtmlLink(syndicationFeed.Links);
      string siteUrl = FindAbsoluteUrl(siteLink);

      List<FeedItem> items =
        syndicationFeed.Items.Select(CreateFeedItem)
          .ToList();

      return
        new Feed {
          Title = feedTitle,
          FeedUrl = feedUrl,
          SiteUrl = siteUrl,
          Items = items,
        };
    }

    private static SyndicationLink FindHtmlLink(IEnumerable<SyndicationLink> links) {
      return
        links.FirstOrDefault(
          link => (!link.MediaType.IsNullOrEmpty() && link.MediaType.ContainsOrdinalIgnoreCase("html")
                   || (link.MediaType.IsNullOrEmpty() && "alternate".EqualsOrdinalIgnoreCase(link.RelationshipType))));
    }

    private static string FindAbsoluteUrl(SyndicationLink syndicationLink = null) {
      if (syndicationLink == null) {
        return null;
      }

      Uri uri = syndicationLink.GetAbsoluteUri();

      if (uri == null) {
        return null;
      }

      return uri.ToString();
    }

    private static FeedItem CreateFeedItem(SyndicationItem syndicationItem) {
      SyndicationLink itemLink = FindHtmlLink(syndicationItem.Links);
      string itemUrl = FindAbsoluteUrl(itemLink);

      return
        new FeedItem {
          DatePublished =
            syndicationItem.PublishDate != null
              ? syndicationItem.PublishDate.UtcDateTime
              : (DateTime?)null,
          Title =
            syndicationItem.Title != null
              ? syndicationItem.Title.Text
              : null,
          Url = itemUrl,
        };
    }

  }

}
