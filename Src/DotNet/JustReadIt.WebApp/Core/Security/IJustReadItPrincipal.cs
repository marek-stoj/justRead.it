using System.Security.Principal;

namespace JustReadIt.WebApp.Core.Security {

  public interface IJustReadItPrincipal : IPrincipal {

    int UserAccountId { get; }
  }

}
