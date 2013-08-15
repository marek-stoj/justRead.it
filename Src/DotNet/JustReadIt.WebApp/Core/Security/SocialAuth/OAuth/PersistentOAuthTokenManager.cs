using System;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using DotNetOpenAuth.OpenId.Extensions.OAuth;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Core.Security.SocialAuth.OAuth {

  public class PersistentOAuthTokenManager : IConsumerTokenManager, IOpenIdOAuthTokenManager {

    private readonly ITokenStore _tokenStore;

    #region Constructor(s)

    public PersistentOAuthTokenManager(string consumerKey, string consumerSecret, ITokenStore tokenStore) {
      Guard.ArgNotNullNorEmpty(consumerKey, "consumerKey");
      Guard.ArgNotNullNorEmpty(consumerSecret, "consumerSecret");
      Guard.ArgNotNull(tokenStore, "tokenStore");

      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      _tokenStore = tokenStore;
    }

    #endregion

    #region IConsumerTokenManager Members

    public string ConsumerKey { get; private set; }

    public string ConsumerSecret { get; private set; }

    #endregion

    #region ITokenManager Members

    public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret) {
      Guard.ArgNotNullNorEmpty(consumerKey, "consumerKey");
      Guard.ArgNotNullNorEmpty(requestToken, "requestToken");
      Guard.ArgNotNullNorEmpty(accessToken, "accessToken");
      Guard.ArgNotNullNorEmpty(accessTokenSecret, "accessTokenSecret");

      _tokenStore.Remove(requestToken);
      _tokenStore.Store(accessToken, accessTokenSecret);
    }

    public string GetTokenSecret(string token) {
      Guard.ArgNotNullNorEmpty(token, "token");

      string secret = _tokenStore.Get(token);

      if (secret == null) {
        throw new ArgumentException(string.Format("Couldn't find value for token '{0}'.", token));
      }

      return secret;
    }

    public TokenType GetTokenType(string token) {
      throw new NotSupportedException();
    }

    public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response) {
      Guard.ArgNotNull(request, "request");
      Guard.ArgNotNull(response, "response");

      _tokenStore.Store(response.Token, response.TokenSecret);
    }

    #endregion

    #region IOpenIdOAuthTokenManager Members

    public void StoreOpenIdAuthorizedRequestToken(string consumerKey, AuthorizationApprovedResponse authorization) {
      throw new NotSupportedException();
    }

    #endregion
  }

}
