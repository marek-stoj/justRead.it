using System.Security.Principal;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Security {

  public interface IJustReadItPrincipal : IPrincipal {

    int UserAccountId { get; }
  }

}
