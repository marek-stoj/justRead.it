using JustReadIt.WebApp.Areas.FeedbinApi.Core.Services;
using JustReadIt.WebApp.Core.Services;

namespace JustReadIt.WebApp.Core.App {

  public static class IoC {

    public static IAuthenticationService GetAuthenticationService() {
      return new FormsAuthenticationService();
    }

    public static IDomainToJsonModelMapper GetDomainToJsonModelMapper() {
      return new DomainToJsonModelMapper();
    }

    public static ICacheService GetCacheService() {
      return new AspNetCacheService();
    }

  }

}
