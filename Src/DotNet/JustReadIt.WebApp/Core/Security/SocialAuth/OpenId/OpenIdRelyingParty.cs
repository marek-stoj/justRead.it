using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace JustReadIt.WebApp.Core.Security.SocialAuth.OpenId {

  public class OpenIdRelyingParty : IOpenIdRelyingParty {

    private readonly DotNetOpenAuth.OpenId.RelyingParty.OpenIdRelyingParty _openIdRelyingParty;

    #region Constructor(s)

    public OpenIdRelyingParty() {
      _openIdRelyingParty = new DotNetOpenAuth.OpenId.RelyingParty.OpenIdRelyingParty();
    }

    #endregion

    #region IOpenIdRelyingParty Members

    public IAuthenticationResponse GetResponse() {
      return _openIdRelyingParty.GetResponse();
    }

    public IAuthenticationRequest CreateRequest(Identifier openIdIdentifier) {
      return _openIdRelyingParty.CreateRequest(openIdIdentifier);
    }

    #endregion
  }

}
