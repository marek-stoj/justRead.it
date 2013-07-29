namespace JustReadIt.Core.Services.Feeds {

  public interface IFeedFetcher {

    FetchFeedResult FetchFeed(string feedUrl);

  }

}
