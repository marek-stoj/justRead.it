using System;
using System.Collections.Generic;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using DotNetOpenAuth.OpenId.Extensions.OAuth;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Core.Security.SocialAuth.OAuth {

  public class TransientOAuthTokenManager : IConsumerTokenManager, IOpenIdOAuthTokenManager {

    private readonly Dictionary<string, string> _tokensAndSecrets;

    public TransientOAuthTokenManager(string consumerKey, string consumerSecret) {
      Guard.ArgNotNullNorEmpty(consumerKey, "consumerKey");
      Guard.ArgNotNullNorEmpty(consumerSecret, "consumerSecret");

      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;

      _tokensAndSecrets = new Dictionary<string, string>();
    }

    #region IConsumerTokenManager

    public string ConsumerKey { get; private set; }

    public string ConsumerSecret { get; private set; }

    #endregion

    #region ITokenManager Members

    public string GetTokenSecret(string token) {
      return _tokensAndSecrets[token];
    }

    public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response) {
      _tokensAndSecrets[response.Token] = response.TokenSecret;
    }

    public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret) {
      _tokensAndSecrets.Remove(requestToken);
      _tokensAndSecrets[accessToken] = accessTokenSecret;
    }

    public TokenType GetTokenType(string token) {
      throw new NotSupportedException();
    }

    #endregion

    #region IOpenIdOAuthTokenManager Members

    public void StoreOpenIdAuthorizedRequestToken(string consumerKey, AuthorizationApprovedResponse authorization) {
      _tokensAndSecrets[authorization.RequestToken] = "";
    }

    #endregion
  }

}
