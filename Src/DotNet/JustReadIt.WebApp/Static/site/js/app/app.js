var app = angular.module('JustReadIt', ['ngResource', 'ui.bootstrap', 'ngUpload'], function($routeProvider, $locationProvider, $httpProvider) {
  var interceptor =
  [
    '$rootScope',
    '$q',
    function(scope, $q) {

      function success(response) {
        return response;
      }

      function error(response) {
        var status = response.status;

        if (status === 401 || status === 403) {
          window.location = "/Account/SignIn";

          return null;
        }

        // otherwise
        return $q.reject(response);
      }

      return function(promise) {
        return promise.then(success, error);
      };
    }];

  $httpProvider.responseInterceptors.push(interceptor);
});
