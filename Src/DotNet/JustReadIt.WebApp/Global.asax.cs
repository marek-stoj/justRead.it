using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using JustReadIt.Core.Common;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Security;
using JustReadIt.WebApp.Core.App;
using Newtonsoft.Json;
using log4net;
using log4net.Config;
using JustReadIt.Core.Common.Logging;

namespace JustReadIt.WebApp {

  public class MvcApplication : HttpApplication {

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public MvcApplication() {
      AuthenticateRequest += MvcApplication_AuthenticateRequest;
    }

    protected void Application_Start() {
      XmlConfigurator.Configure();

      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);

      ConfigureJsonFormatter();

      _log.InfoIfEnabled(() => "Application has started.");
    }

    private static void MvcApplication_AuthenticateRequest(object sender, EventArgs eventArgs) {
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
        int? userAccountId = userAccountRepository.FindUserAccountIdByEmailAddress(username);

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
