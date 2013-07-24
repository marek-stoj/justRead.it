using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;

namespace JustReadIt.WebApp {

  public class MvcApplication : HttpApplication {

    protected void Application_Start() {
      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);

      ConfigureJsonFormatter();
    }

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
