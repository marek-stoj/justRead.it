using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace JustReadIt.WebApp.Core.Security.SocialAuth.OpenId {

  public interface IOpenIdRelyingParty {

    IAuthenticationResponse GetResponse();

    IAuthenticationRequest CreateRequest(Identifier openIdIdentifier);

  }

}
