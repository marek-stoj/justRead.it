using JustReadIt.WebApp.Core.Security;

namespace JustReadIt.WebApp.Core.Services {

  public interface ICacheService {

    IJustReadItPrincipal GetPrincipal(string username);

    void CachePrincipal(IJustReadItPrincipal principal);

  }

}
