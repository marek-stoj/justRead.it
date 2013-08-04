namespace JustReadIt.WebApp.Areas.Feedbin.Core.Services {

  public static class IoC {

    public static IDomainToJsonModelMapper GetDomainToJsonModelMapper() {
      return new DomainToJsonModelMapper();
    }

  }

}
