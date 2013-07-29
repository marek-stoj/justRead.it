using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using JustReadIt.Core.Services.Feeds;
using NUnit.Framework;

namespace JustReadIt.Core.Tests.Services.Feeds {

  [TestFixture]
  public class FeedParserTests {

    private const string _TestFeedsBasePath = @"TestData\Feeds";

    private FeedParser _feedParser;

    [SetUp]
    public void SetUp() {
      _feedParser = new FeedParser();
    }

    [Test]
    [TestCaseSource("TestCaseSource_ParseFeed_correctly_extracts_feed_metadata")]
    public void ParseFeed_correctly_extracts_feed_metadata(string feedFileName, string expectedTitle, string expectedFeedUrl, string expectedSiteUrl) {
      // arrange
      string feedContent = ReadTestFeed(feedFileName);

      // act
      Feed feed = _feedParser.Parse(feedContent);

      // assert
      Assert.AreEqual(expectedTitle, feed.Title, "Feed titles differ.");
      Assert.AreEqual(expectedFeedUrl, feed.FeedUrl, "Feed URLs differ.");
      Assert.AreEqual(expectedSiteUrl, feed.SiteUrl, "Site URLs differ.");
    }

    [Test]
    [TestCaseSource("TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection")]
    public void ParseFeed_correctly_extracts_feed_items_collection(string feedFileName, int expectedItemsCount) {
      // arrange
      string feedContent = ReadTestFeed(feedFileName);

      // act
      Feed feed = _feedParser.Parse(feedContent);

      // assert
      Assert.AreEqual(expectedItemsCount, feed.Items.Count());
    }

    [Test]
    [TestCaseSource("TestCaseSource_ParseFeed_correctly_extracts_feed_items")]
    public void ParseFeed_correctly_extracts_feed_items(string feedFileName, string expectedFeedTitle, string expectedFeedUrl, DateTime expectedPublicationDate) {
      // arrange
      string feedContent = ReadTestFeed(feedFileName);

      // act
      Feed feed = _feedParser.Parse(feedContent);

      // assert
      FeedItem feedItem = feed.Items.FirstOrDefault();

      Assert.IsNotNull(feedItem);

      Assert.AreEqual(expectedFeedUrl, feedItem.Url);
      Assert.AreEqual(expectedFeedTitle, feedItem.Title);
      Assert.AreEqual((DateTime?)expectedPublicationDate, feedItem.DatePublished);
    }

    private static string ReadTestFeed(string feedFileName) {
      return File.ReadAllText(Path.Combine(_TestFeedsBasePath, feedFileName));
    }

    // ReSharper disable UnusedMethodReturnValue.Local

    private static IEnumerable<TestCaseData> TestCaseSource_ParseFeed_correctly_extracts_feed_metadata() {
      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_01.xml",
          "Google Chrome Blog",
          "http://feeds.feedburner.com/blogspot/Egta",
          "http://chrome.blogspot.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_02.xml",
          "Coding Horror",
          "http://feeds.feedburner.com/codinghorror",
          "http://www.codinghorror.com/blog/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_03.xml",
          "Townhall's Recent Columns",
          null,
          "http://townhall.com/columnists");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_04.xml",
          "Ars Technica",
          "http://feeds.arstechnica.com/arstechnica/index",
          "http://arstechnica.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_05.xml",
          "CNN.com - Top Stories",
          "http://rss.cnn.com/rss/edition",
          "http://edition.cnn.com/index.html?eref=edition");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_06.xml",
          "Laughing Squid",
          "http://laughingsquid.com/feed/",
          "http://laughingsquid.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_07.xml",
          "Hot Air » Top Picks",
          "http://feeds.feedburner.com/hotair/main",
          "http://hotair.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_08.xml",
          "The Next Web",
          "http://feeds.feedburner.com/TheNextWeb",
          "http://thenextweb.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_09.xml",
          "SBNation.com -  All Posts",
          "http://feeds.feedburner.com/sportsblogs/sbnation",
          "http://www.sbnation.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_10.xml",
          "Zero Hedge",
          "http://feeds.feedburner.com/zerohedge/feed",
          "http://www.zerohedge.com/fullrss2.xml");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_11.xml",
          "GigaOM",
          "http://gigaom.com/feed/",
          "http://gigaom.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_12.xml",
          "Mediaite",
          "http://www.mediaite.com/feed/",
          "http://www.mediaite.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_13.xml",
          "TechCrunch",
          "http://feeds.feedburner.com/Techcrunch",
          "http://techcrunch.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_14.xml",
          "Engadget RSS Feed",
          null,
          "http://www.engadget.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_15.xml",
          "Gizmodo",
          "http://www.gizmodo.com/index.xml",
          "http://gizmodo.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_16.xml",
          "TMZ.com",
          "http://www.tmz.com/rss.xml",
          "http://www.tmz.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_17.xml",
          "Gawker",
          "http://www.gawker.com/index.xml",
          "http://gawker.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_18.xml",
          "The Verge -  All Posts",
          null,
          "http://www.theverge.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_19.xml",
          "Mashable",
          "http://feeds.mashable.com/Mashable",
          "http://mashable.com/stories/?utm_campaign=Mash-Prod-RSS-Feedburner-All-Partial&utm_cid=Mash-Prod-RSS-Feedburner-All-Partial&utm_medium=feed&utm_source=rss");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_20.xml",
          "BuzzFeed - Raw Feed",
          "http://buzzfeed.com/raw.xml",
          "http://buzzfeed.com/raw");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_21.xml",
          "The Full Feed from HuffingtonPost.com",
          "http://feeds.huffingtonpost.com/huffingtonpost/raw_feed",
          "http://www.huffingtonpost.com/raw_feed_index.rdf");
    }

    private static IEnumerable<TestCaseData> TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection() {
      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(
          "feed_01.xml", 25);

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(
          "feed_02.xml", 10);

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(
          "feed_03.xml", 20);

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(
          "feed_04.xml", 25);

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(
          "feed_05.xml", 115);

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(
          "feed_06.xml", 25);

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(
          "feed_07.xml", 25);

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(
          "feed_08.xml", 10);

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(
          "feed_09.xml", 8);

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(
          "feed_10.xml", 25);
    }

    private static IEnumerable<TestCaseData> TestCaseSource_ParseFeed_correctly_extracts_feed_items() {
      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_01.xml",
          "From TVs to tablets: Everything you love, across all your screens",
          "http://feedproxy.google.com/~r/blogspot/Egta/~3/INMF1bGtBJ4/from-tvs-to-tablets-everything-you-love.html",
          "2013-07-24T13:14:00.000-04:00");

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_02.xml",
          "The Rule of Three",
          "http://www.codinghorror.com/blog/2013/07/rule-of-three.html",
          "Thu, 18 Jul 2013 23:19:22 -0700");

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_03.xml",
          "Textbook Praises Islam, Denigrates Christianity",
          "http://townhall.com/columnists/toddstarnes/2013/07/29/textbook-praises-islam-denigrates-christianity-n1651503",
          "2013-07-29T09:24:00-04:00");

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_04.xml",
          "Disabling a car’s brakes and speed by hacking its computers: A new how-to",
          "http://feeds.arstechnica.com/~r/arstechnica/index/~3/RJXQXJbvmjE/story01.htm",
          "Mon, 29 Jul 2013 14:43:11 GMT");

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_05.xml",
          "$53m in jewels taken in Cannes heist",
          "http://edition.cnn.com/2013/07/28/world/europe/cannes-jewel-theft/index.html?eref=edition",
          "Sun, 28 Jul 2013 19:01:51 -0400");

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_06.xml",
          "LCD Soundsystem’s ‘All My Friends’ Music Video Remade Using LEGO",
          "http://laughingsquid.com/lcd-soundsystems-all-my-friends-recreated-using-lego/",
          "Mon, 29 Jul 2013 14:43:51 +0000");

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_07.xml",
          "Pew poll: Major swing against government surveillance among tea partiers",
          "http://hotair.com/archives/2013/07/29/pew-poll-major-swing-against-government-surveillance-among-tea-partiers/",
          "Mon, 29 Jul 2013 15:21:39 +0000");

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_08.xml",
          "Why it’s not easy to break into the Brazilian tech market – a chat with Boo-box’s Marco Gomes",
          "http://feedproxy.google.com/~r/TheNextWeb/~3/JDqDUrRCAVE/",
          "Mon, 29 Jul 2013 15:03:35 +0000");

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_09.xml",
          "Khalil Mack 2014 NFL Draft preseason scouting report",
          "http://www.sbnation.com/nfl-mock-draft/2013/7/29/4567834/khalil-mack-2014-nfl-draft-preseason-scouting-report",
          "2013-07-29T15:26:07Z");

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_10.xml",
          "Meet The \"Mental Asylum\" - The Department Of Homeland Security's New 'Pentagon'",
          "http://feedproxy.google.com/~r/zerohedge/feed/~3/jxg-49PFOD0/story01.htm",
          "Mon, 29 Jul 2013 14:56:08 GMT");
    }

    // ReSharper restore UnusedMethodReturnValue.Local

    private static TestCaseData CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(string feedFileName, string expectedTitle, string expectedFeedUrl, string expectedSiteUrl) {
      return
        new TestCaseData(
          feedFileName,
          expectedTitle,
          expectedFeedUrl,
          expectedSiteUrl).SetName(feedFileName);
    }

    private static TestCaseData CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items_collection(string feedFileName, int expectedItemsCount) {
      return
        new TestCaseData(
          feedFileName,
          expectedItemsCount).SetName(feedFileName);
    }

    private static TestCaseData CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(string feedFileName, string expectedFeedTitle, string expectedFeedUrl, string expectedPublicationDateString) {
      return
        new TestCaseData(
          feedFileName,
          expectedFeedTitle,
          expectedFeedUrl,
          DateTime.Parse(expectedPublicationDateString, CultureInfo.InvariantCulture.DateTimeFormat).ToUniversalTime())
          .SetName(feedFileName);
    }

  }

}
