using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Core.Security.SocialAuth.OAuth {

  public class TwitterConsumer : ITwitterConsumer {

    private readonly IConsumerTokenManager _consumerTokenManager;

    private static readonly ServiceProviderDescription ServiceDescription =
      new ServiceProviderDescription {
        RequestTokenEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/request_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
        UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/authorize", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
        AccessTokenEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/access_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
        TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
      };

    private static readonly ServiceProviderDescription SignInWithTwitterServiceDescription =
      new ServiceProviderDescription {
        RequestTokenEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/request_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
        UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/authenticate", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
        AccessTokenEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/access_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
        TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
      };

    private static readonly MessageReceivingEndpoint _GetFavoritesEndpoint = new MessageReceivingEndpoint("http://twitter.com/favorites.xml", HttpDeliveryMethods.GetRequest);
    private static readonly MessageReceivingEndpoint _GetFriendTimelineStatusEndpoint = new MessageReceivingEndpoint("http://twitter.com/statuses/friends_timeline.xml", HttpDeliveryMethods.GetRequest);
    private static readonly MessageReceivingEndpoint _UpdateProfileBackgroundImageEndpoint = new MessageReceivingEndpoint("http://twitter.com/account/update_profile_background_image.xml", HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);
    private static readonly MessageReceivingEndpoint _UpdateProfileImageEndpoint = new MessageReceivingEndpoint("http://twitter.com/account/update_profile_image.xml", HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);
    private static readonly MessageReceivingEndpoint _VerifyCredentialsEndpoint = new MessageReceivingEndpoint("http://api.twitter.com/1/account/verify_credentials.xml", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);

    private static readonly object _signInConsumerInitLock = new object();

    private static volatile WebConsumer _signInConsumer;

    #region Constructor(s)

    static TwitterConsumer() {
      // Twitter can't handle the Expect 100 Continue HTTP header.
      ServicePointManager.FindServicePoint(_GetFavoritesEndpoint.Location).Expect100Continue = false;
    }

    public TwitterConsumer(IConsumerTokenManager consumerTokenManager) {
      Guard.ArgNotNull(consumerTokenManager, "consumerTokenManager");

      _consumerTokenManager = consumerTokenManager;
    }

    #endregion

    #region ITwitterConsumer Members

    public XDocument GetUpdates(ConsumerBase twitter, string accessToken) {
      IncomingWebResponse response = twitter.PrepareAuthorizedRequestAndSend(_GetFriendTimelineStatusEndpoint, accessToken);

      return XDocument.Load(XmlReader.Create(response.GetResponseReader()));
    }

    public XDocument GetFavorites(ConsumerBase twitter, string accessToken) {
      IncomingWebResponse response = twitter.PrepareAuthorizedRequestAndSend(_GetFavoritesEndpoint, accessToken);

      return XDocument.Load(XmlReader.Create(response.GetResponseReader()));
    }

    public XDocument UpdateProfileBackgroundImage(ConsumerBase twitter, string accessToken, string image, bool tile) {
      string imageExtension = Path.GetExtension(image);

      if (string.IsNullOrEmpty(imageExtension)) {
        throw new ArgumentException("Image extension can't be empty.", "image");
      }

      var parts =
        new[] {
          MultipartPostPart.CreateFormFilePart("image", image, "image/" + imageExtension.Substring(1).ToLowerInvariant()),
          MultipartPostPart.CreateFormPart("tile", tile.ToString().ToLowerInvariant()),
        };

      HttpWebRequest request = twitter.PrepareAuthorizedRequest(_UpdateProfileBackgroundImageEndpoint, accessToken, parts);

      request.ServicePoint.Expect100Continue = false;

      IncomingWebResponse response = twitter.Channel.WebRequestHandler.GetResponse(request);
      string responseString = response.GetResponseReader().ReadToEnd();

      return XDocument.Parse(responseString);
    }

    public XDocument UpdateProfileImage(ConsumerBase twitter, string accessToken, string pathToImage) {
      string imageExtension = Path.GetExtension(pathToImage);

      if (string.IsNullOrEmpty(imageExtension)) {
        throw new ArgumentException("Image extension can't be empty.", "pathToImage");
      }

      string contentType = "image/" + imageExtension.Substring(1).ToLowerInvariant();

      return UpdateProfileImage(twitter, accessToken, File.OpenRead(pathToImage), contentType);
    }

    public XDocument UpdateProfileImage(ConsumerBase twitter, string accessToken, Stream image, string contentType) {
      var parts =
        new[] {
          MultipartPostPart.CreateFormFilePart("image", "twitterPhoto", contentType, image),
        };

      HttpWebRequest request = twitter.PrepareAuthorizedRequest(_UpdateProfileImageEndpoint, accessToken, parts);
      IncomingWebResponse response = twitter.Channel.WebRequestHandler.GetResponse(request);
      string responseString = response.GetResponseReader().ReadToEnd();

      return XDocument.Parse(responseString);
    }

    public XDocument VerifyCredentials(ConsumerBase twitter, string accessToken) {
      IncomingWebResponse response = twitter.PrepareAuthorizedRequestAndSend(_VerifyCredentialsEndpoint, accessToken);

      return XDocument.Load(XmlReader.Create(response.GetResponseReader()));
    }

    public string GetUsername(ConsumerBase twitter, string accessToken) {
      XDocument xml = VerifyCredentials(twitter, accessToken);
      XPathNavigator nav = xml.CreateNavigator();

      XPathNavigator screenNameNode = nav.SelectSingleNode("/user/screen_name");

      if (screenNameNode == null) {
        throw new Exception("Couldn't get screen name because the /user/screen_name node doesn't exist.");
      }

      return screenNameNode.Value;
    }

    public void StartSignInWithTwitter(string callbackUrl, bool forceNewLogin = false) {
      WebConsumer twitterSignInConsumer = GetTwitterSignInConsumer();

      Uri callbackUri = new Uri(callbackUrl);
      var redirectParameters = new Dictionary<string, string>();

      if (forceNewLogin) {
        redirectParameters["force_login"] = "true";
      }

      UserAuthorizationRequest request =
        twitterSignInConsumer.PrepareRequestUserAuthorization(
          callbackUri,
          null,
          redirectParameters);

      OutgoingWebResponse response =
        twitterSignInConsumer.Channel
          .PrepareResponse(request);

      response.Send();
    }

    public bool TryFinishSignInWithTwitter(out int userId, out string screenName) {
      userId = -1;
      screenName = null;

      WebConsumer twitterSignInConsumer = GetTwitterSignInConsumer();

      AuthorizedTokenResponse response =
        twitterSignInConsumer.ProcessUserAuthorization();

      if (response == null) {
        return false;
      }

      screenName = response.ExtraData["screen_name"];
      userId = int.Parse(response.ExtraData["user_id"]);

      return true;
    }

    #endregion

    #region Private helper methods

    private WebConsumer GetTwitterSignInConsumer() {
      if (_signInConsumer == null) {
        lock (_signInConsumerInitLock) {
          if (_signInConsumer == null) {
            _signInConsumer = new WebConsumer(SignInWithTwitterServiceDescription, _consumerTokenManager);
          }
        }
      }

      return _signInConsumer;
    }

    #endregion
  }

}
