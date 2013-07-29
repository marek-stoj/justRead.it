using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Services.Opml {

  public class OpmlParser : IOpmlParser {

    public ParseResult Parse(string opmlXml) {
      Guard.ArgNotNullNorEmpty(opmlXml, "opmlXml");

      XDocument opmlDocument = XDocument.Parse(opmlXml);

      if (opmlDocument.Root == null) {
        throw new FormatException("OPML document has no root element.");
      }

      XElement bodyElement = opmlDocument.Root.Element("body");

      if (bodyElement == null) {
        throw new FormatException("OPML document has no 'body' element.");
      }

      var feedGroups = new List<FeedGroup>();
      var uncategorizedFeeds = new List<Feed>();

      foreach (XElement outlineElement in bodyElement.Elements("outline")) {
        if (IsFeedGroupElement(outlineElement)) {
          ProcessFeedGroupElement(outlineElement, feedGroups);
        }
        else if (IsFeedElement(outlineElement)) {
          ProcessFeedElement(outlineElement, uncategorizedFeeds);
        }
      }

      return new ParseResult(feedGroups, uncategorizedFeeds);
    }

    private static void ProcessFeedElement(XElement outlineElement, List<Feed> feeds) {
      string feedTitle = null;
      string textAttributeValue = GetAttributeValue(outlineElement, "text", null);

      if (!string.IsNullOrEmpty(textAttributeValue)) {
        feedTitle = textAttributeValue;
      }
      else {
        string titleAttributeValue = GetAttributeValue(outlineElement, "title", null);

        if (!string.IsNullOrEmpty(titleAttributeValue)) {
          feedTitle = titleAttributeValue;
        }
      }

      if (string.IsNullOrEmpty(feedTitle)) {
        return;
      }

      string feedUrl = GetAttributeValue(outlineElement, "xmlUrl", null);

      if (string.IsNullOrEmpty(feedUrl)) {
        return;
      }

      string siteUrl = GetAttributeValue(outlineElement, "htmlUrl", null);

      var feed = new Feed(feedTitle, feedUrl, siteUrl);

      feeds.Add(feed);
    }

    private static void ProcessFeedGroupElement(XElement outlineElement, List<FeedGroup> feedGroups) {
      string groupTitle = null;
      string textAttributeValue = GetAttributeValue(outlineElement, "text", null);

      if (!string.IsNullOrEmpty(textAttributeValue)) {
        groupTitle = textAttributeValue;
      }
      else {
        string titleAttributeValue = GetAttributeValue(outlineElement, "title", null);

        if (!string.IsNullOrEmpty(titleAttributeValue)) {
          groupTitle = titleAttributeValue;
        }
      }

      if (string.IsNullOrEmpty(groupTitle)) {
        return;
      }

      var feedGroup = new FeedGroup(groupTitle);

      feedGroups.Add(feedGroup);

      foreach (XElement childOutlineElement in outlineElement.Elements("outline").Where(IsFeedElement)) {
        ProcessFeedElement(childOutlineElement, feedGroup.Feeds);
      }
    }

    private static bool IsFeedElement(XElement outlineElement) {
      string typeAttributeValue = GetAttributeValue(outlineElement, "type", null);
      string textAttributeValue = GetAttributeValue(outlineElement, "text", null);
      string titleAttributeValue = GetAttributeValue(outlineElement, "title", null);
      string xmlUrlAttributeValue = GetAttributeValue(outlineElement, "xmlUrl", null);

      bool typeIsRss =
        string.Equals(typeAttributeValue, "rss", StringComparison.OrdinalIgnoreCase);

      bool hasTitle =
        !string.IsNullOrEmpty(textAttributeValue) || !string.IsNullOrEmpty(titleAttributeValue);

      bool hasFeedUrl =
        !string.IsNullOrEmpty(xmlUrlAttributeValue);

      return typeIsRss && hasTitle && hasFeedUrl;
    }

    private static bool IsFeedGroupElement(XElement outlineElement) {
      string textAttributeValue = GetAttributeValue(outlineElement, "text", null);
      string titleAttributeValue = GetAttributeValue(outlineElement, "title", null);
      string typeAttributeValue = GetAttributeValue(outlineElement, "type", null);
      string xmlUrlAttributeValue = GetAttributeValue(outlineElement, "xmlUrl", null);

      bool hasTitle =
        !string.IsNullOrEmpty(textAttributeValue) || !string.IsNullOrEmpty(titleAttributeValue);

      bool typeIsRss =
        string.Equals(typeAttributeValue, "rss", StringComparison.OrdinalIgnoreCase);

      bool hasFeedUrl =
        !string.IsNullOrEmpty(xmlUrlAttributeValue);

      return hasTitle && !typeIsRss && !hasFeedUrl;
    }

    private static string GetAttributeValue(XElement element, string attributeName, string defaultValue = null) {
      XAttribute attribute = element.Attribute(attributeName);

      return attribute != null ? attribute.Value : defaultValue;
    }

  }

}
