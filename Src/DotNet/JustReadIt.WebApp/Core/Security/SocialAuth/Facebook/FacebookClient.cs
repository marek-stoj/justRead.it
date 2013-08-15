using System.IO;
using ImmRafSoft.Net;
using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Core.Security.SocialAuth.Facebook {

  public class FacebookClient : IFacebookClient {

    private const string _ApiUrlTemplate_GetUserInfo = "https://graph.facebook.com/me?access_token=${accessToken}";

    private readonly IWebClientFactory _webClientFactory;

    public FacebookClient(IWebClientFactory webClientFactory) {
      Guard.ArgNotNull(webClientFactory, "webClientFactory");

      _webClientFactory = webClientFactory;
    }

    public UserInfo GetUserInfo(string accessToken) {
      Guard.ArgNotNull(accessToken, "accessToken");

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
          UserName = responseObj.username,
          Email = responseObj.email,
        };
    }

  }

}
