using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Security {

  public class JustReadItIdentity : IJustReadItIdentity {

    public JustReadItIdentity(string name) {
      Guard.ArgNotNullNorEmpty(name, "name");

      Name = name;
    }

    public string Name { get; private set; }

    public string AuthenticationType {
      get {
        return "JustReadIt";
      }
    }

    public bool IsAuthenticated {
      get {
        return true;
      }
    }

  }

}
