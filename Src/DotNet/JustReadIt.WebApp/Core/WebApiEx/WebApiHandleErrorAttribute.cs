using System.Reflection;
using System.Web.Http.Filters;
using JustReadIt.Core.Common.Logging;
using log4net;

namespace JustReadIt.WebApp.Core.WebApiEx {

  public class WebApiHandleErrorAttribute : ExceptionFilterAttribute {

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public override void OnException(HttpActionExecutedContext actionExecutedContext) {
      _log.ErrorIfEnabled(() => "Unhandled exception.", actionExecutedContext.Exception);

      base.OnException(actionExecutedContext);
    }

  }

}
