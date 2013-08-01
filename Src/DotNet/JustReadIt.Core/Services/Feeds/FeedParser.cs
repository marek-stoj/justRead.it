using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using ImmRafSoft.Xml;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Services.Feeds {

  public class FeedParser : IFeedParser {

    public Feed Parse(string feedContent) {
      Guard.ArgNotNullNorEmpty(feedContent, "feedContent");

      SyndicationFeed syndicationFeed;

      using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(feedContent)))
      using (var xss = new XmlSanitizingStream(ms))
      using (XmlReader xr = XmlReader.Create(xss)) {
        syndicationFeed = SyndicationFeed.Load(xr);
      }

      if (syndicationFeed == null) {
        throw new FeedParserException("Couldn't parse the feed.");
      }

      string feedTitle =
        syndicationFeed.Title != null
          ? syndicationFeed.Title.Text
          : null;

      SyndicationLink feedLink = FindFeedLink(syndicationFeed);
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

    private static SyndicationLink FindFeedLink(SyndicationFeed syndicationFeed) {
      Collection<SyndicationLink> links = syndicationFeed.Links;
      SyndicationLink feedLink;

      feedLink =
        links.FirstOrDefault(
          link => !link.RelationshipType.IsNullOrEmpty() && "self".EqualsOrdinalIgnoreCase(link.RelationshipType));

      if (feedLink != null) {
        return feedLink;
      }

      feedLink =
        links.FirstOrDefault(
          link => !link.MediaType.IsNullOrEmpty() && link.MediaType.ContainsOrdinalIgnoreCase("rss"));

      if (feedLink != null) {
        return feedLink;
      }

      feedLink =
        links.FirstOrDefault(
          link => !link.MediaType.IsNullOrEmpty() && link.MediaType.ContainsOrdinalIgnoreCase("xml"));

      return feedLink;
    }

    private static SyndicationLink FindHtmlLink(Collection<SyndicationLink> links) {
      SyndicationLink siteLink;

      siteLink =
        links.FirstOrDefault(
          link => !link.MediaType.IsNullOrEmpty() && link.MediaType.ContainsOrdinalIgnoreCase("html"));

      if (siteLink != null) {
        return siteLink;
      }

      siteLink =
        links.FirstOrDefault(
          link => link.MediaType.IsNullOrEmpty() && "alternate".EqualsOrdinalIgnoreCase(link.RelationshipType));

      if (siteLink != null) {
        return siteLink;
      }

      siteLink =
        links.FirstOrDefault(
          link => "alternate".EqualsOrdinalIgnoreCase(link.MediaType) && "alternate".EqualsOrdinalIgnoreCase(link.RelationshipType));

      return siteLink;
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
          Title =
            syndicationItem.Title != null
              ? syndicationItem.Title.Text
              : null,
          Url = itemUrl,
          DatePublished =
            syndicationItem.PublishDate != null
              ? syndicationItem.PublishDate.UtcDateTime
              : (DateTime?)null,
          Author =
            syndicationItem.Authors != null
              ? CreateAuthorString(syndicationItem)
              : null,
          Summary =
            syndicationItem.Summary != null
              ? syndicationItem.Summary.Text
              : null,
          Content =
            syndicationItem.Content != null
              ? CreateContentString(syndicationItem)
              : null,
        };
    }

    private static string CreateAuthorString(SyndicationItem syndicationItem) {
      if (syndicationItem.Authors.Count == 0) {
        return null;
      }

      IEnumerable<string> authorsNames =
        syndicationItem.Authors
          .Select(sp => sp.Name);

      return
        string.Join(
          ", ",
          authorsNames.Where(n => !n.IsNullOrEmpty()));
    }

    private static string CreateContentString(SyndicationItem syndicationItem) {
      var textSyndicationContent = syndicationItem.Content as TextSyndicationContent;

      return
        textSyndicationContent != null
          ? textSyndicationContent.Text
          : null;
    }

  }

}
