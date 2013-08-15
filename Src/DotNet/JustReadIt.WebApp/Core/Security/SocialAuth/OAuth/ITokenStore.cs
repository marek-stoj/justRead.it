using System;

namespace JustReadIt.WebApp.Core.Security.SocialAuth.OAuth {

  public interface ITokenStore {

    Guid Store(string token, string secret);

    string Get(string token);

    void Remove(string token);

  }

}
