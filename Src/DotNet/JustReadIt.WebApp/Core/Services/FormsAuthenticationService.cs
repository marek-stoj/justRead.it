using System.Web.Security;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Core.Services {

  public class FormsAuthenticationService : IAuthenticationService {

    public void SignIn(string emailAddress, bool createPersistentCookie) {
      Guard.ArgNotNullNorEmpty(emailAddress, "emailAddress");

      FormsAuthentication.SetAuthCookie(emailAddress, createPersistentCookie);
    }

    public void SignOut() {
      FormsAuthentication.SignOut();
    }

  }

}
