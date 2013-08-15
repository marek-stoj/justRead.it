using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using ImmRafSoft.Net;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.WebApp.Core.App;
using JustReadIt.WebApp.Core.MvcEx;
using JustReadIt.WebApp.Core.Resources;
using JustReadIt.WebApp.Core.Security.SocialAuth.Facebook;
using JustReadIt.WebApp.Core.Security.SocialAuth.OAuth;
using JustReadIt.WebApp.Core.Security.SocialAuth.OpenId;
using JustReadIt.WebApp.Core.Security.SocialAuth.WindowsLive;
using JustReadIt.WebApp.Core.Services;
using Newtonsoft.Json;
using FacebookUserInfo = JustReadIt.WebApp.Core.Security.SocialAuth.Facebook.UserInfo;
using WindowsLiveUserInfo = JustReadIt.WebApp.Core.Security.SocialAuth.WindowsLive.UserInfo;

namespace JustReadIt.WebApp.Core.Controllers {

  // TODO IMM HI: handle returnUrls
  public class AuthController : JustReadItController {

    private const string _OpenIdIdentifier_Facebook = "http://www.facebook.com/";
    private const string _OpenIdIdentifier_Twitter = "http://twitter.com/";
    private const string _OpenIdIdentifier_LiveId = "https://live.com/";
    private const string _OpenIdIdentifier_LiveJournal = "http://.livejournal.com/";
    private const string _OpenIdIdentifier_Wordpress = "http://.wordpress.com/";
    private const string _OpenIdIdentifier_Blogger = "http://.blogspot.com/";
    private const string _OpenIdIdentifier_VeriSign = "http://.pip.verisignlabs.com/";
    private const string _OpenIdIdentifier_ClaimId = "http://claimid.com/";
    private const string _OpenIdIdentifier_ClickPass = "http://clickpass.com/public/";
    private const string _OpenIdIdentifier_GoogleProfile = "http://www.google.com/profiles/";

    private const string _FacebookAppId = "218283511596312";
    private const string _FacebookAppSecret = "a0eb57c0dfc71cbd3a4849c929fee510";

    private const string _LiveId_ClientId = "0000000044086499";
    private const string _LiveId_ClientSecret = "cZ0bcnqCZw7m9-1CDd2F35j3WulE8sof";

    private readonly IAuthenticationService _authenticationService;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IWebClientFactory _webClientFactory;
    private readonly IOpenIdRelyingParty _openIdRelyingParty;
    private readonly ITwitterConsumer _twitterConsumer;
    private readonly IFacebookClient _facebookClient;
    private readonly IWindowsLiveClient _windowsLiveClient;

    public AuthController(IAuthenticationService authenticationService, IUserAccountRepository userAccountRepository, IWebClientFactory webClientFactory, IOpenIdRelyingParty openIdRelyingParty, ITwitterConsumer twitterConsumer, IFacebookClient facebookClient, IWindowsLiveClient windowsLiveClient) {
      Guard.ArgNotNull(authenticationService, "authenticationService");
      Guard.ArgNotNull(userAccountRepository, "userAccountRepository");
      Guard.ArgNotNull(webClientFactory, "webClientFactory");
      Guard.ArgNotNull(openIdRelyingParty, "openIdRelyingParty");
      Guard.ArgNotNull(twitterConsumer, "twitterConsumer");
      Guard.ArgNotNull(facebookClient, "facebookClient");
      Guard.ArgNotNull(windowsLiveClient, "windowsLiveClient");

      _authenticationService = authenticationService;
      _userAccountRepository = userAccountRepository;
      _webClientFactory = webClientFactory;
      _openIdRelyingParty = openIdRelyingParty;
      _twitterConsumer = twitterConsumer;
      _facebookClient = facebookClient;
      _windowsLiveClient = windowsLiveClient;
    }

    public AuthController()
      : this(IoC.GetAuthenticationService(), CommonIoC.GetUserAccountRepository(), CommonIoC.GetWebClientFactory(), IoC.GetOpenIdRelyingParty(), IoC.GetTwitterConsumer(), IoC.GetFacebookClient(), IoC.GetWindowsLiveClient()) {
    }

    // TODO IMM HI: xxx remove?
    [HttpGet]
    public ActionResult SignIn() {
      return View();
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    public ActionResult SignIn_Do(string openid_identifier, string openid_username) {
      IAuthenticationResponse authResponse = _openIdRelyingParty.GetResponse();

      if (authResponse == null) {
        if (string.IsNullOrEmpty(openid_identifier)) {
          // TODO IMM HI: xxx test
          return RedirectToSignIn();
        }

        if (openid_identifier == _OpenIdIdentifier_Twitter) {
          return SignInWithTwitter();
        }

        if (openid_identifier == _OpenIdIdentifier_Facebook) {
          return SignInWithFacebook();
        }

        if (openid_identifier == _OpenIdIdentifier_LiveId) {
          return SignInWithLiveId();
        }

        if (IsUserNameRquired(openid_identifier) && string.IsNullOrEmpty(openid_username)) {
          ModelState.AddModelError("openid_username", ValidationResources.OpenIdUserNameIsRequired);

          return View("SignIn");
        }

        if (openid_identifier == "http://" || openid_identifier == "https://") {
          ModelState.AddModelError("openid_identifier", ValidationResources.OpenIdIdentifierIsRequired);

          return View("SignIn");
        }

        return RedirectToOpenIdProvider(openid_identifier);
      }

      return HandleOpenIdProviderResponse(authResponse);
    }

/*
 * // TODO IMM HI: xxx handle users without e-mail addresses
 * 
    [HttpGet]
    public ActionResult SignInWithTwitter_Callback() {
      int userId;
      string screenName;

      if (_twitterConsumer.TryFinishSignInWithTwitter(out userId, out screenName)) {
        string authProviderId = string.Format("http://twitter.com/{0}/{1}", userId, screenName);
        bool isNewAccount;

        CreateUserAccountIfNeeded(
          authProviderId,
          out isNewAccount,
          emailAddress: null,
          displayName: screenName);

        return DoSignIn(screenName, isNewAccount);
      }

      return RedirectAfterSignInFailed();
    }
*/

    [HttpGet]
    public ActionResult SignInWithFacebook_Callback(string code, string error) {
      if (!string.IsNullOrEmpty(error)) {
        return RedirectAfterSignInFailed();
      }

      string redirectUrl = CreateFacebookCallbackUrl();

      string getAccessTokenUrl =
        string.Format(
          "https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&code={2}&redirect_uri={3}",
          _FacebookAppId,
          _FacebookAppSecret,
          code,
          HttpUtility.UrlEncode(redirectUrl));

      string response;

      using (var wc = _webClientFactory.CreateWebClient()) {
        response = wc.DownloadString(getAccessTokenUrl);
      }

      if (string.IsNullOrEmpty(response)) {
        return RedirectAfterSignInFailed();
      }

      Dictionary<string, string> responseValues =
        response.Split('&')
          .Select(s => s.Split('='))
          .ToDictionary(keyValue => keyValue[0], keyValue => keyValue[1]);

      string accessToken;

      if (!responseValues.TryGetValue("access_token", out accessToken)) {
        return RedirectAfterSignInFailed();
      }

      FacebookUserInfo userInfo = _facebookClient.GetUserInfo(accessToken);

      string authProviderId = string.Format("http://www.facebook.com/{0}", userInfo.Id);
      string email = userInfo.Email;
      string userName = userInfo.UserName;
      bool isNewAccount;

      CreateUserAccountIfNeeded(
        authProviderId, out isNewAccount,
        email,
        userName);

      return DoSignIn(email, isNewAccount);
    }

    [HttpGet]
    public ActionResult SignInWithLiveId_Callback(string code, string error) {
      if (!string.IsNullOrEmpty(error)) {
        return RedirectAfterSignInFailed();
      }

      string response;

      using (var wc = _webClientFactory.CreateWebClient()) {
        var postData = new NameValueCollection();
        string redirectUrl = CreateLiveIdCallbackUrl();

        postData.Add("client_id", _LiveId_ClientId);
        postData.Add("redirect_uri", redirectUrl);
        postData.Add("client_secret", _LiveId_ClientSecret);
        postData.Add("code", code);
        postData.Add("grant_type", "authorization_code");

        response = wc.UploadValues("https://oauth.live.com/token", postData);
      }

      string accessToken;

      using (var sr = new StringReader(response))
      using (var jsonReader = new JsonTextReader(sr)) {
        var jsonSerializer = new JsonSerializer();
        dynamic responseObj = jsonSerializer.Deserialize(jsonReader);

        accessToken = responseObj.access_token;
      }

      WindowsLiveUserInfo userInfo = _windowsLiveClient.GetUserInfo(accessToken);

      string authProviderId = userInfo.Link;
      string email = userInfo.AccountEmail;
      string userName = userInfo.Name;
      bool isNewAccount;

      CreateUserAccountIfNeeded(
        authProviderId,
        out isNewAccount,
        email,
        userName);

      return DoSignIn(email, isNewAccount);
    }

    [HttpGet]
    public ActionResult SignOut() {
      _authenticationService.SignOut();

      return RedirectToHome();
    }

    private static bool IsUserNameRquired(string openIdIdentifier) {
      if (openIdIdentifier == _OpenIdIdentifier_LiveJournal
          || openIdIdentifier == _OpenIdIdentifier_Wordpress
          || openIdIdentifier == _OpenIdIdentifier_Blogger
          || openIdIdentifier == _OpenIdIdentifier_VeriSign
          || openIdIdentifier == _OpenIdIdentifier_ClaimId
          || openIdIdentifier == _OpenIdIdentifier_ClickPass
          || openIdIdentifier == _OpenIdIdentifier_GoogleProfile) {
        return true;
      }

      return false;
    }

    private static void AddSimpleRegistrationExtension(IAuthenticationRequest authRequest) {
      ClaimsRequest claimsRequest =
        new ClaimsRequest {
          Nickname = DemandLevel.Request,
          Email = DemandLevel.Request,
        };

      authRequest.AddExtension(claimsRequest);
    }

    private static void AddAttributeExchangeExtension(IAuthenticationRequest authRequest) {
      var fetchRequest = new FetchRequest();

      // note: email attribute has to be set as required in order to get it returned from Google
      fetchRequest.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
      fetchRequest.Attributes.AddOptional(WellKnownAttributes.Name.Alias);

      authRequest.AddExtension(fetchRequest);
    }

    private static void ObtainUserInfo(ClaimsResponse claimsResponse, FetchResponse fetchResponse, out string email, out string userName) {
      email = null;
      userName = null;

      if (claimsResponse != null) {
        email = claimsResponse.Email;
        userName = claimsResponse.Nickname;
      }

      if (fetchResponse != null) {
        string tmpEmail = fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Email);

        if (!string.IsNullOrEmpty(tmpEmail)) {
          email = tmpEmail;
        }

        string tmpUserName = fetchResponse.GetAttributeValue(WellKnownAttributes.Name.Alias);

        if (!string.IsNullOrEmpty(tmpUserName)) {
          userName = tmpUserName;
        }
      }
    }

    // TODO IMM HI: xxx test/remove?
    private ActionResult RedirectToSignIn() {
      return RedirectToHome();
    }

    private ActionResult RedirectAfterSignInSucceeded(bool isFirstSignIn) {
      return RedirectToApp();
    }

    private ActionResult RedirectAfterSignInFailed() {
      // TODO IMM HI: xxx sign in failed feedback
      //SetFeedbackMessage(FeedbackMessages.CouldntAuth, FeedbackMessageType.Warning);

      return RedirectToSignIn();
    }

    private ActionResult RedirectToOpenIdProvider(string openid_identifier) {
      Identifier identifier = Identifier.Parse(openid_identifier);
      IAuthenticationRequest authRequest;

      try {
        authRequest = _openIdRelyingParty.CreateRequest(identifier);
      }
      catch (ProtocolException) {
        return RedirectAfterSignInFailed();
      }

      AddSimpleRegistrationExtension(authRequest);
      AddAttributeExchangeExtension(authRequest);

      return authRequest.RedirectingResponse.AsActionResult();
    }

    private ActionResult HandleOpenIdProviderResponse(IAuthenticationResponse authResponse) {
      switch (authResponse.Status) {
        case AuthenticationStatus.Authenticated: {
          string authProviderId = authResponse.ClaimedIdentifier;
          ClaimsResponse claimsResponse = authResponse.GetExtension<ClaimsResponse>();
          FetchResponse fetchResponse = authResponse.GetExtension<FetchResponse>();
          string email;
          string userName;

          ObtainUserInfo(claimsResponse, fetchResponse, out email, out userName);

          bool isNewAccount;

          CreateUserAccountIfNeeded(
            authProviderId,
            out isNewAccount,
            email,
            userName);

          return DoSignIn(email, isNewAccount);
        }

        case AuthenticationStatus.Canceled: {
          return RedirectToSignIn();
        }

        default: {
          return RedirectAfterSignInFailed();
        }
      }
    }

    private void CreateUserAccountIfNeeded(string authProviderId, out bool isNewAccount, string emailAddress = null, string displayName = null) {
      UserAccount userAccount =
        _userAccountRepository.FindByAuthProviderId(
          authProviderId);

      if (userAccount == null) {
        isNewAccount = true;

        userAccount =
          new UserAccount {
            AuthProviderId = authProviderId,
            EmailAddress = emailAddress,
          };

        _userAccountRepository.Add(userAccount);
      }
      else {
        isNewAccount = false;
      }
    }

    private ActionResult SignInWithTwitter() {
      Uri requestUrl = Request.Url;

      if (requestUrl == null) {
        throw new InvalidOperationException("Request.Url is null.");
      }

      string callBackUrl = CreateTwitterCallbackUrl();

      _twitterConsumer.StartSignInWithTwitter(callBackUrl);

      return RedirectToSignIn();
    }

    private ActionResult SignInWithFacebook() {
      string redirectUrl = CreateFacebookCallbackUrl();

      string facebookSignInUrl =
        string.Format(
          "https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&scope=email",
          _FacebookAppId,
          HttpUtility.UrlEncode(redirectUrl));

      return Redirect(facebookSignInUrl);
    }

    private ActionResult SignInWithLiveId() {
      string redirectUrl = CreateLiveIdCallbackUrl();

      string liveIdSignInUrl =
        string.Format(
          "https://oauth.live.com/authorize?client_id={0}&scope=wl.signin%20wl.emails&response_type=code&redirect_uri={1}",
          _LiveId_ClientId,
          HttpUtility.UrlEncode(redirectUrl));

      return Redirect(liveIdSignInUrl);
    }

    private ActionResult DoSignIn(string username, bool isFirstSignIn) {
      _authenticationService.SignIn(username, true);

      return RedirectAfterSignInSucceeded(isFirstSignIn);
    }

    private string CreateTwitterCallbackUrl() {
      return Url.AbsoluteAction("SignInWithTwitter_Callback", "Auth");
    }

    private string CreateFacebookCallbackUrl() {
      return Url.AbsoluteAction("SignInWithFacebook_Callback", "Auth");
    }

    private string CreateLiveIdCallbackUrl() {
      return Url.AbsoluteAction("SignInWithLiveId_Callback", "Auth");
    }

  }

}
