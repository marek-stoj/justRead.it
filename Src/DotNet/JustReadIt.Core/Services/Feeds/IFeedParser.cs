namespace JustReadIt.Core.Services.Feeds {

  public interface IFeedParser {

    Feed Parse(string feedContent);

  }

}
