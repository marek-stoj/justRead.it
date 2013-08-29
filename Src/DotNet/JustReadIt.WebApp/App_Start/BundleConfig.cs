using System.Web.Optimization;
using JustReadIt.WebApp.Core.MvcEx;

namespace JustReadIt.WebApp.App_Start
{
  // TODO IMM HI: xxx to minify or not to minify?
  public class BundleConfig
  {
    public static void RegisterBundles(BundleCollection bundles)
    {
      RegisterStyleBundles(bundles);
      RegisterScriptBundles(bundles);
    }

    private static void RegisterStyleBundles(BundleCollection bundles)
    {
      // bootstrap
      bundles.Add(
        new StyleBundle("~/bundles/css/bootstrap")
          .Include("~/Static/bootstrap/css/bootstrap.min.css"));

      // bootstrap
      bundles.Add(
        new StyleBundle("~/bundles/css/google-fonts")
          .Include("~/Static/google-fonts/cardo.css")
          .Include("~/Static/google-fonts/pt-sans-narrow.css"));

      // app
      bundles.Add(
        new StyleBundle("~/bundles/css/app")
          .Include("~/Static/site/css/app.css")
          .Include("~/Static/site/css/app-*"));

      // app
      bundles.Add(
        new StyleBundle("~/bundles/css/app")
          .Include("~/Static/site/css/app.css")
          .Include("~/Static/site/css/app-*"));

      // webkit-scrollbars
      bundles.Add(
        new StyleBundle("~/bundles/css/webkit-scrollbars")
          .Include("~/Static/site/css/webkit-scrollbars.css"));
    }

    private static void RegisterScriptBundles(BundleCollection bundles)
    {
      // js - jquery
      bundles.Add(
        new ScriptBundle("~/bundles/js/jquery")
          .Include("~/Static/jquery/jquery.min.js")
          .WithoutMinification());

      // js - bootstrap
      bundles.Add(
        new ScriptBundle("~/bundles/js/bootstrap")
          .Include("~/Static/bootstrap/js/bootstrap.min.js")
          .WithoutMinification());

      // js - angular
      bundles.Add(
        new ScriptBundle("~/bundles/js/angular")
          .Include("~/Static/angularjs/angular.min.js")
          .Include("~/Static/angularjs/angular-resource.min.js")
          .Include("~/Static/ui-bootstrap/ui-bootstrap-0.5.0.min.js")
          .Include("~/Static/ngUpload/ng-upload.min.js")
          .WithoutMinification());

      // js - underscore
      bundles.Add(
        new ScriptBundle("~/bundles/js/underscore")
          .Include("~/Static/underscore/underscore-min.js")
          .Include("~/Static/underscore/underscore-ex.js"));

      // js - app
      bundles.Add(
        new ScriptBundle("~/bundles/js/app")
          .Include("~/Static/site/js/app/util/*.js")
          .Include("~/Static/site/js/app/models/*.js")
          .Include("~/Static/site/js/app/*.js")
          .Include("~/Static/site/js/app/controllers/*.js")
          .WithoutMinification());
    }
  }
}
