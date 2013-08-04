using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace JustReadIt.WebApp.Core.MvcEx {

  /// <remarks>
  /// Taken from: http://blogs.infosupport.com/asp-net-mvc-4-rc-getting-webapi-and-areas-to-play-nicely/
  /// </remarks>
  public class AreaHttpControllerSelector : DefaultHttpControllerSelector {

    private const string AreaRouteVariableName = "area";

    private readonly HttpConfiguration _configuration;
    private readonly IEnumerable<Assembly> _controllersAssemblies;
    private readonly Lazy<ConcurrentDictionary<string, Type>> _apiControllerTypes;

    public AreaHttpControllerSelector(HttpConfiguration configuration, IEnumerable<Assembly> controllersAssemblies)
      : base(configuration) {
      _configuration = configuration;
      _controllersAssemblies = new List<Assembly>(controllersAssemblies.Distinct(new AssembleyEqualityComparer()));
      _apiControllerTypes = new Lazy<ConcurrentDictionary<string, Type>>(GetControllerTypes);
    }

    public override HttpControllerDescriptor SelectController(HttpRequestMessage request) {
      return GetApiController(request);
    }

    private ConcurrentDictionary<string, Type> GetControllerTypes() {
      Dictionary<string, Type> types =
        _controllersAssemblies
          .SelectMany(
            a =>
            a.GetTypes()
              .Where(
                t =>
                !t.IsAbstract &&
                t.Name.EndsWith(ControllerSuffix, StringComparison.OrdinalIgnoreCase) &&
                typeof(IHttpController).IsAssignableFrom(t)))
          .ToDictionary(t => t.FullName, t => t);

      return new ConcurrentDictionary<string, Type>(types);
    }

    private HttpControllerDescriptor GetApiController(HttpRequestMessage request) {
      var areaName = GetAreaName(request);
      var controllerName = GetControllerName(request);
      var type = GetControllerType(areaName, controllerName);

      return new HttpControllerDescriptor(_configuration, controllerName, type);
    }

    private Type GetControllerType(string areaName, string controllerName) {
      var query = _apiControllerTypes.Value.AsEnumerable();

      if (string.IsNullOrEmpty(areaName)) {
        query = query.WithoutAreaName();
      }
      else {
        query = query.ByAreaName(areaName);
      }

      return query
        .ByControllerName(controllerName)
        .Select(x => x.Value)
        .Single();
    }

    private static string GetAreaName(HttpRequestMessage request) {
      var data = request.GetRouteData();
      if (data.Route.DataTokens == null) {
        return null;
      }
      else {
        object areaName;
        return data.Route.DataTokens.TryGetValue(AreaRouteVariableName, out areaName) ? areaName.ToString() : null;
      }
    }

  }

}
