using System.Collections.Generic;
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
      Assert.AreEqual(expectedTitle, feed.Title);
      Assert.AreEqual(expectedFeedUrl, feed.FeedUrl);
      Assert.AreEqual(expectedSiteUrl, feed.SiteUrl);
    }

    [Test]
    [TestCaseSource("TestCaseSource_ParseFeed_correctly_extracts_feed_items")]
    public void ParseFeed_correctly_extracts_feed_items(string feedFileName, int expectedItemsCount) {
      // arrange
      string feedContent = ReadTestFeed(feedFileName);

      // act
      Feed feed = _feedParser.Parse(feedContent);

      // assert
      Assert.AreEqual(expectedItemsCount, feed.Items.Count());
    }

    private static string ReadTestFeed(string feedFileName) {
      return File.ReadAllText(Path.Combine(_TestFeedsBasePath, feedFileName));
    }

    private static IEnumerable<TestCaseData> TestCaseSource_ParseFeed_correctly_extracts_feed_metadata() {
      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_atom_01.xml",
          "Google Chrome Blog",
          "http://feeds.feedburner.com/blogspot/Egta",
          "http://chrome.blogspot.com/");

      yield return
        CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(
          "feed_atom_02.xml",
          "Coding Horror",
          "http://feeds.feedburner.com/codinghorror",
          "http://www.codinghorror.com/blog/");
    }

    private static IEnumerable<TestCaseData> TestCaseSource_ParseFeed_correctly_extracts_feed_items() {
      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_atom_01.xml",
          25);

      yield return
        CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(
          "feed_atom_02.xml",
          10);
    }

    private static TestCaseData CreateTestCaseDataFor_ParseFeed_correctly_extracts_feed_metadata(string feedFileName, string expectedTitle, string expectedFeedUrl, string expectedSiteUrl) {
      return
        new TestCaseData(
          feedFileName,
          expectedTitle,
          expectedFeedUrl,
          expectedSiteUrl).SetName(feedFileName);
    }

    private static TestCaseData CreateTestCaseDataFor_TestCaseSource_ParseFeed_correctly_extracts_feed_items(string feedFileName, int expectedItemsCount) {
      return
        new TestCaseData(
          feedFileName,
          expectedItemsCount).SetName(feedFileName);
    }

  }

}
