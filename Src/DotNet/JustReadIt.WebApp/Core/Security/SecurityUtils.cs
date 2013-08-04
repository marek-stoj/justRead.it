using System;
using System.Security;
using System.Web;

namespace JustReadIt.WebApp.Core.Security {

  public static class SecurityUtils {

    public static string CurrentUsername {
      get {
        IJustReadItPrincipal justReadItPrincipal = GetJustReadItPrincipal();

        return justReadItPrincipal.Identity.Name;
      }
    }

    public static int CurrentUserAccountId {
      get {
        IJustReadItPrincipal justReadItPrincipal = GetJustReadItPrincipal();

        return justReadItPrincipal.UserAccountId;
      }
    }

    private static IJustReadItPrincipal GetJustReadItPrincipal() {
      if (HttpContext.Current == null) {
        throw new InvalidOperationException("No http context present.");
      }

      IJustReadItPrincipal justReadItPrincipal = HttpContext.Current.User as IJustReadItPrincipal;

      if (justReadItPrincipal == null) {
        throw new SecurityException("User is not signed in.");
      }

      return justReadItPrincipal;
    }

  }

}
