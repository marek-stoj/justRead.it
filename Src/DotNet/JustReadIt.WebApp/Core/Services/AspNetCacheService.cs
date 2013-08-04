using System;
using System.Web;
using System.Web.Caching;
using JustReadIt.Core.Common;
using JustReadIt.WebApp.Core.Security;

namespace JustReadIt.WebApp.Core.Services {

  public class AspNetCacheService : ICacheService {

    private static readonly TimeSpan _CachePrincipalSlidingExpiration = TimeSpan.FromMinutes(20.0);

    private const string _CacheKeyPrefix_Principal = "Principal_";

    public void CachePrincipal(IJustReadItPrincipal principal) {
      Guard.ArgNotNull(principal, "principal");

      Cache cache = GetCache();
      string username = principal.Identity.Name;
      string cacheKey = CreatePrincipalCacheKey(username);

      cache.Insert(
        cacheKey,
        principal,
        null,
        Cache.NoAbsoluteExpiration,
        _CachePrincipalSlidingExpiration);
    }

    public IJustReadItPrincipal GetPrincipal(string username) {
      Guard.ArgNotNullNorEmpty(username, "username");

      Cache cache = GetCache();
      string cacheKey = CreatePrincipalCacheKey(username);
      object cachedObject = cache.Get(cacheKey);

      return cachedObject as IJustReadItPrincipal;
    }

    private static Cache GetCache() {
      HttpContext httpContext = HttpContext.Current;

      if (httpContext == null) {
        throw new InvalidOperationException("No http context present.");
      }

      Cache cache = httpContext.Cache;

      if (cache == null) {
        throw new InvalidOperationException("No cache present.");
      }

      return cache;
    }

    private static string CreatePrincipalCacheKey(string username) {
      return string.Format("{0}{1}", _CacheKeyPrefix_Principal, username);
    }

  }

}
