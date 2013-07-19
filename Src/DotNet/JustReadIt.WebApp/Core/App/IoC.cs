using System.Configuration;
using JustReadIt.Core.DataAccess;
using JustReadIt.Core.Domain;

namespace JustReadIt.WebApp.Core.App {

  public static class IoC {

    private const string _ConnectionString = "JustReadIt";

    private static readonly string _JustReadItDbConnectionString;

    static IoC() {
      _JustReadItDbConnectionString =
        ConfigurationManager.ConnectionStrings[_ConnectionString]
          .ConnectionString;
    }

    public static IFeedRepository CreateFeedRepository() {
      return new DapperFeedRepository(_JustReadItDbConnectionString);
    }

  }

}
