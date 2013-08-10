using System;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using JustReadIt.Core.Common.Logging;
using log4net;

namespace JustReadIt.WebApp.Core.WebApiEx {

  public class ExceptionLoggingHttpControllerActivator : IHttpControllerActivator {

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly IHttpControllerActivator _httpControllerActivator;

    public ExceptionLoggingHttpControllerActivator(IHttpControllerActivator httpControllerActivator) {
      _httpControllerActivator = httpControllerActivator;
    }

    public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType) {
      try {
        return _httpControllerActivator.Create(request, controllerDescriptor, controllerType);
      }
      catch (Exception exc) {
        _log.ErrorIfEnabled(() => "Error while activating HTTP controller.", exc);

        throw;
      }
    }

  }

}
