namespace JustReadIt.WebApp.Areas.App.Core.Services {

  public static class IoC {

    public static IQueryModelToJsonModelMapper GetQueryModelToJsonModelMapper() {
      return new QueryModelToJsonModelMapper();
    }

  }

}
