using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using JustReadIt.Core.Common;
using JustReadIt.WebApp.Areas.App.Core.Controllers;
using JustReadIt.WebApp.Core.App;
using JustReadIt.WebApp.Core.MvcEx;
using JustReadIt.WebApp.Core.Security;
using JustReadIt.WebApp.Core.WebApiEx;
using Newtonsoft.Json;
using log4net;
using log4net.Config;
using JustReadIt.Core.Common.Logging;

namespace JustReadIt.WebApp {

  public class MvcApplication : HttpApplication {

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public MvcApplication() {
      AuthorizeRequest += MvcApplication_AuthorizeRequest;
    }

    protected void Application_Start() {
      XmlConfigurator.Configure();

      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      FilterConfig.RegisterGlobalWebApiFilters(GlobalConfiguration.Configuration.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);

      // makes Web API and areas play together nicely (see: http://blogs.infosupport.com/asp-net-mvc-4-rc-getting-webapi-and-areas-to-play-nicely/)
      GlobalConfiguration.Configuration.Services.Replace(
        typeof(IHttpControllerSelector),
        new AreaHttpControllerSelector(
          GlobalConfiguration.Configuration,
          new[] {
            typeof(SubscriptionsController).Assembly,
            typeof(Areas.Feedbin.Core.Controllers.SubscriptionsController).Assembly,
          }));

      GlobalConfiguration.Configuration.Services.Replace(
        typeof(IHttpControllerActivator),
        new ExceptionLoggingHttpControllerActivator(GlobalConfiguration.Configuration.Services.GetHttpControllerActivator()));

      ConfigureJsonFormatter();

      _log.InfoIfEnabled(() => "Application has started.");
    }

    protected void Application_Error() {
      Exception lastException = Server.GetLastError();

      _log.ErrorIfEnabled(() => "Application error.", lastException);
    }

    private static void MvcApplication_AuthorizeRequest(object sender, EventArgs eventArgs) {
      if (HttpContext.Current.User == null
       || HttpContext.Current.User.Identity == null
       || !HttpContext.Current.User.Identity.IsAuthenticated) {
        return;
      }

      var formsIdentity = HttpContext.Current.User.Identity as FormsIdentity;

      if (formsIdentity == null) {
        return;
      }

      var cacheService = IoC.GetCacheService();
      string username = formsIdentity.Name;
      IJustReadItPrincipal justReadItPrincipal = cacheService.GetPrincipal(username);

      if (justReadItPrincipal == null) {
        var userAccountRepository = CommonIoC.GetUserAccountRepository();
        int? userAccountId = userAccountRepository.FindIdByEmailAddress(username);

        if (!userAccountId.HasValue) {
          return;
        }

        var justReadItIdentity = new JustReadItIdentity(username);

        justReadItPrincipal = new JustReadItPrincipal(userAccountId.Value, justReadItIdentity);

        cacheService.CachePrincipal(justReadItPrincipal);
      }

      HttpContext.Current.User = justReadItPrincipal;
    }

    // TODO IMM HI: think about whether we need to force Web API to always use JSON Media Formatter when binding input models, even if content-type header was not set (it defaults to application/xml then)
    private static void ConfigureJsonFormatter() {
      HttpConfiguration configuration = GlobalConfiguration.Configuration;

      // remove XML formatter
      var xmlMediaType = configuration.Formatters.XmlFormatter.SupportedMediaTypes
        .Single(t => t.MediaType == "application/xml");

      configuration.Formatters.XmlFormatter.SupportedMediaTypes.Remove(xmlMediaType);

      // configure Json.NET
      JsonMediaTypeFormatter jsonFormatter = configuration.Formatters.JsonFormatter;
      JsonSerializerSettings jsonSerializerSettings = jsonFormatter.SerializerSettings;

      jsonSerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;

#if DEBUG
      jsonSerializerSettings.Formatting = Formatting.Indented;
#endif
    }

  }

}
