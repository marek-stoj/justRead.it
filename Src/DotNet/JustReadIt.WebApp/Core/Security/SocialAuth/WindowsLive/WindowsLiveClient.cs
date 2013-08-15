using System.IO;
using ImmRafSoft.Net;
using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Core.Security.SocialAuth.WindowsLive {

  public class WindowsLiveClient : IWindowsLiveClient {

    private const string _ApiUrlTemplate_GetUserInfo = "https://apis.live.net/v5.0/me?access_token=${accessToken}";

    private readonly IWebClientFactory _webClientFactory;

    #region Constructor(s)

    public WindowsLiveClient(IWebClientFactory webClientFactory) {
      Guard.ArgNotNull(webClientFactory, "webClientFactory");

      _webClientFactory = webClientFactory;
    }

    #endregion

    #region IWindowsLiveClient Members

    public UserInfo GetUserInfo(string accessToken) {
      Guard.ArgNotNullNorEmpty(accessToken, "accessToken");

      string url = _ApiUrlTemplate_GetUserInfo.Replace("${accessToken}", accessToken);
      string response;

      using (IWebClient webClient = _webClientFactory.CreateWebClient()) {
        response = webClient.DownloadString(url);
      }

      var jsonSerializer = new JsonSerializer();
      dynamic responseObj;

      using (var sr = new StringReader(response))
      using (var jsonReader = new JsonTextReader(sr)) {
        responseObj = jsonSerializer.Deserialize(jsonReader);
      }

      return
        new UserInfo {
          Id = responseObj.id,
          Link = responseObj.link,
          Name = responseObj.name,
          AccountEmail = responseObj.emails["account"],
        };
    }

    #endregion
  }

}
