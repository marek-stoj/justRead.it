namespace JustReadIt.WebApp.Core.Security.SocialAuth.Facebook {

  public interface IFacebookClient {

    UserInfo GetUserInfo(string accessToken);

  }

}
