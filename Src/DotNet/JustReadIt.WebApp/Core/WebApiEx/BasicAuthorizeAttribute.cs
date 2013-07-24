using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Core.WebApiEx {

  public abstract class BasicAuthorizeAttribute : AuthorizationFilterAttribute {

    private const string _Header_Authorization = "Authorization";
    private const string _Header_Basic = "Basic ";

    private static readonly Encoding _Encoding = Encoding.GetEncoding("iso-8859-1");

    private readonly string _basicAuthRealm;

    protected BasicAuthorizeAttribute(string basicAuthRealm) {
      Guard.ArgNotNullNorEmpty(basicAuthRealm, "basicAuthRealm");

      if (basicAuthRealm.IndexOf("\"", StringComparison.OrdinalIgnoreCase) > -1) {
        throw new ArgumentException("Argument must not contain double quote characters.", "basicAuthRealm");
      }

      _basicAuthRealm = basicAuthRealm;
    }

    public override void OnAuthorization(HttpActionContext actionContext) {
      if (IsRequestAuthenticated(actionContext.Request)) {
        return;
      }

      var httpResponseMessage =
        new HttpResponseMessage(HttpStatusCode.Unauthorized) {
          Content = new StringContent("401 Unauthorized"),
        };

      httpResponseMessage.Headers.WwwAuthenticate
        .Add(
          new AuthenticationHeaderValue(
            "Basic",
            string.Format("realm=\"{0}\"", _basicAuthRealm)));

      actionContext.Response = httpResponseMessage;
    }

    private bool IsRequestAuthenticated(HttpRequestMessage httpRequestMessage) {
      string authorizationHeader = null;
      IEnumerable<string> authorizationHeaderValues;

      if (httpRequestMessage.Headers.TryGetValues(_Header_Authorization, out authorizationHeaderValues)) {
        authorizationHeader = authorizationHeaderValues.FirstOrDefault();
      }

      if (string.IsNullOrEmpty(authorizationHeader)) {
        return false;
      }

      if (!authorizationHeader.StartsWith(_Header_Basic, StringComparison.OrdinalIgnoreCase)) {
        return false;
      }

      string credentialsString = authorizationHeader.Substring(_Header_Basic.Length);

      string[] credentials =
        _Encoding.GetString(Convert.FromBase64String(credentialsString))
          .Split(':');

      if (credentials.Length != 2) {
        return false;
      }

      string username = credentials[0];
      string password = credentials[1];

      if (!AreCredentialsValid(username, password)) {
        return false;
      }

      SetCurrentContextUser(username);

      return true;
    }

    protected abstract bool AreCredentialsValid(string username, string password);

    protected virtual void SetCurrentContextUser(string username) {
      HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(username), null);
    }

  }

}
