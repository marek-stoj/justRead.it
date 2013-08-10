using System.Web.Http.Filters;
using System.Web.Mvc;
using JustReadIt.WebApp.Core.MvcEx;
using JustReadIt.WebApp.Core.WebApiEx;

namespace JustReadIt.WebApp {

  public class FilterConfig {

    public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
      filters.Add(new WebAppHandleErrorAttribute());
    }

    public static void RegisterGlobalWebApiFilters(HttpFilterCollection filters) {
      filters.Add(new WebApiHandleErrorAttribute());
    }

  }

}
