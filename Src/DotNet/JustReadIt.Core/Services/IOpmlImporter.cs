namespace JustReadIt.Core.Services {

  public interface IOpmlImporter {

    void Import(string opmlXml, int userAccountId);

  }

}
