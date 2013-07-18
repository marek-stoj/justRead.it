using System.Web.Mvc;

namespace JustReadIt.WebApp {

  public class FilterConfig {

    public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
      filters.Add(new HandleErrorAttribute());
    }

  }

}
