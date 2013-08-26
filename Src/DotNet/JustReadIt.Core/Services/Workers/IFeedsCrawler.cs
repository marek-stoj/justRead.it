namespace JustReadIt.Core.Services.Workers {

  public interface IFeedsCrawler {

    void CrawlAllFeeds();

    /// <param name="feedUrl">Url of the feed. Must already be present in the db.</param>
    void CrawlFeedIfNeeded(string feedUrl);

  }

}
