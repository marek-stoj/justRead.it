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

app.service('commonOptionsSvc', function() {
  this.modalOpts = {
    backdropFade: true,
    dialogFade: true
  };
});

app.service('objectSyncer', function() {
  var _objectSyncer = new ObjectSyncer();

  this.sync = function(dstObj, srcObj) {
    _objectSyncer.sync(dstObj, srcObj, 'id');
  };
});

// TODO IMM HI: xxx remove
var x = 1;

app.directive('focusMe', function($timeout) {
  return {
    link: function(scope, element, attrs) {
      scope.$watch(attrs.focusMe, function(value) {
        // TODO IMM HI: xxx why is it called multiple times?
        console.log(element);
        console.log('watching ' + (x++));
        
        if (value === true) {
          $timeout(function() {
            element[0].focus();
          });
        }
      });
    }
  };
});
