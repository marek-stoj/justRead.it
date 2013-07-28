namespace JustReadIt.Core.Services.Feeds {

  public interface IFeedParser {

    void Parse(string feedContent);

    FeedMetadata GetMetadata();

  }

}
