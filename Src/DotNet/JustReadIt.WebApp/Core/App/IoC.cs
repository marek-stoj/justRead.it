using System.Configuration;
using JustReadIt.Core.DataAccess.Dapper;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services;
using JustReadIt.Core.Services.Opml;

namespace JustReadIt.WebApp.Core.App {

  public static class IoC {

    private const string _ConnectionStringName_JustReadIt = "JustReadIt";

    private static readonly string _ConnectionString_JustReadIt;

    static IoC() {
      _ConnectionString_JustReadIt =
        ConfigurationManager.ConnectionStrings[_ConnectionStringName_JustReadIt]
          .ConnectionString;
    }

    private static IUserAccountRepository CreateUserAccountRepository() {
      return new UserAccountRepository(_ConnectionString_JustReadIt);
    }

    public static IFeedRepository CreateFeedRepository() {
      return new FeedRepository(_ConnectionString_JustReadIt);
    }

    private static IUserFeedGroupRepository CreateUserFeedGroupRepository() {
      return new UserFeedGroupRepository(_ConnectionString_JustReadIt);
    }

    private static IUserFeedGroupFeedRepository CreateUserFeedGroupFeedRepository() {
      return new UserFeedGroupFeedRepository(_ConnectionString_JustReadIt);
    }

    public static IOpmlParser CreateOpmlParser() {
      return new OpmlParser();
    }

    public static IOpmlImporter CreateOpmlImporter() {
      return
        new OpmlImporter(
          CreateOpmlParser(),
          CreateUserAccountRepository(),
          CreateFeedRepository(),
          CreateUserFeedGroupRepository(),
          CreateUserFeedGroupFeedRepository());
    }

  }

}
