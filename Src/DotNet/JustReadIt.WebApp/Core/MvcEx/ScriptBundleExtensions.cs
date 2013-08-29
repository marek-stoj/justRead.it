using System.Linq;
using System.Web.Optimization;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Core.MvcEx
{
  public static class ScriptBundleExtensions
  {

    public static Bundle WithoutMinification(this Bundle scriptBundle)
    {
      Guard.ArgNotNull(scriptBundle, "scriptBundle");

      scriptBundle.Transforms.Remove(
        scriptBundle.Transforms.First(t => t is JsMinify));

      return scriptBundle;
    }
  }
}
