using System.Security.Principal;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Core.Security {

  public class JustReadItPrincipal : IJustReadItPrincipal {

    private readonly int _userAccountId;

    public JustReadItPrincipal(int userAccountId, IJustReadItIdentity identity) {
      Guard.ArgNotNull(identity, "identity");

      _userAccountId = userAccountId;
      Identity = identity;
    }

    public override string ToString() {
      return string.Format("[{0}] {1}", _userAccountId, Identity.Name);
    }

    public bool IsInRole(string role) {
      return false;
    }

    public IIdentity Identity { get; private set; }

    public int UserAccountId {
      get { return _userAccountId; }
    }

  }

}
