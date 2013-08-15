namespace JustReadIt.WebApp.Core.Security.SocialAuth.WindowsLive {

  public interface IWindowsLiveClient {

    UserInfo GetUserInfo(string accessToken);

  }

}
