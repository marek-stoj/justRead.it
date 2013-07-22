namespace JustReadIt.WebApp.Core.Services {

  public interface IAuthenticationService {

    void SignIn(string emailAddress, bool createPersistentCookie);

    void SignOut();

  }

}
