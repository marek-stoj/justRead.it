using DotNetOpenAuth.OAuth.ChannelElements;
using JustReadIt.Core.Common;
using JustReadIt.WebApp.Core.Security.SocialAuth.Facebook;
using JustReadIt.WebApp.Core.Security.SocialAuth.OAuth;
using JustReadIt.WebApp.Core.Security.SocialAuth.OpenId;
using JustReadIt.WebApp.Core.Security.SocialAuth.WindowsLive;
using JustReadIt.WebApp.Core.Services;

namespace JustReadIt.WebApp.Core.App {

  public static class IoC {

    public static IAuthenticationService GetAuthenticationService() {
      return new FormsAuthenticationService();
    }

    public static ICacheService GetCacheService() {
      return new AspNetCacheService();
    }

    public static IOpenIdRelyingParty GetOpenIdRelyingParty() {
      return new OpenIdRelyingParty();
    }

    public static ITwitterConsumer GetTwitterConsumer() {
      return new TwitterConsumer(GetConsumerTokenManager());
    }

    public static IFacebookClient GetFacebookClient() {
      return new FacebookClient(CommonIoC.GetWebClientFactory());
    }

    public static IWindowsLiveClient GetWindowsLiveClient() {
      return new WindowsLiveClient(CommonIoC.GetWebClientFactory());
    }

    public static IConsumerTokenManager GetConsumerTokenManager() {
      // TODO IMM HI: support Twitter
      return new TransientOAuthTokenManager("abc", "def");
    }

  }

}
