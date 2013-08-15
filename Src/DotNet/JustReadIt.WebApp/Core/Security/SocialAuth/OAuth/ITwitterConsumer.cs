using System.IO;
using System.Xml.Linq;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;

namespace JustReadIt.WebApp.Core.Security.SocialAuth.OAuth {

  public interface ITwitterConsumer {

    /// <summary>
    /// Prepares a redirect that will send the user to Twitter to sign in.
    /// </summary>
    /// <param name="callbackUrl">Url to be redirected to when the sign in process is finished.</param>
    /// <param name="forceNewLogin">if set to <c>true</c> the user will be required to re-enter their Twitter credentials even if already logged in to Twitter.</param>
    /// <returns>The redirect message.</returns>
    /// <remarks>
    /// Call <see cref="OutgoingWebResponse.Send()"/> or
    /// <c>return StartSignInWithTwitter().<see cref="MessagingUtilities.AsActionResult">AsActionResult()</see></c>
    /// to actually perform the redirect.
    /// </remarks>
    void StartSignInWithTwitter(string callbackUrl, bool forceNewLogin = false);

    /// <summary>
    /// Checks the incoming web request to see if it carries a Twitter authentication response,
    /// and provides the user's Twitter screen name and unique id if available.
    /// </summary>
    /// <param name="userId">The user's Twitter unique user ID.</param>
    /// <param name="screenName">The user's Twitter screen name.</param>
    /// <returns>
    /// A value indicating whether Twitter authentication was successful;
    /// otherwise <c>false</c> to indicate that no Twitter response was present.
    /// </returns>
    bool TryFinishSignInWithTwitter(out int userId, out string screenName);

    XDocument GetUpdates(ConsumerBase twitter, string accessToken);

    XDocument GetFavorites(ConsumerBase twitter, string accessToken);

    XDocument UpdateProfileBackgroundImage(ConsumerBase twitter, string accessToken, string image, bool tile);

    XDocument UpdateProfileImage(ConsumerBase twitter, string accessToken, string pathToImage);

    XDocument UpdateProfileImage(ConsumerBase twitter, string accessToken, Stream image, string contentType);

    XDocument VerifyCredentials(ConsumerBase twitter, string accessToken);

    string GetUsername(ConsumerBase twitter, string accessToken);

  }

}
