using System.Reflection;
using System.Web.Mvc;
using log4net;
using JustReadIt.Core.Common.Logging;

namespace JustReadIt.WebApp.Core.MvcEx {

  public class WebAppHandleErrorAttribute : HandleErrorAttribute {

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public override void OnException(ExceptionContext filterContext) {
      _log.ErrorIfEnabled(() => "Unhandled exception.", filterContext.Exception);
      
      base.OnException(filterContext);
    }

  }

}
