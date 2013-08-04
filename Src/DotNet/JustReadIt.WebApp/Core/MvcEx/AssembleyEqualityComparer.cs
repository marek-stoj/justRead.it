using System.Collections.Generic;
using System.Reflection;

namespace JustReadIt.WebApp.Core.MvcEx {

  public class AssembleyEqualityComparer : IEqualityComparer<Assembly> {

    public bool Equals(Assembly x, Assembly y) {
      return string.Equals(x.FullName, y.FullName);
    }

    public int GetHashCode(Assembly obj) {
      return obj.FullName.GetHashCode();
    }

  }

}
