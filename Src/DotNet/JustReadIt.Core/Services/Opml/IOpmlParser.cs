namespace JustReadIt.Core.Services.Opml {

  public interface IOpmlParser {

    ParseResult Parse(string opmlXml);

  }

}
