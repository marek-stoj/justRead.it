// TODO IMM HI: do we really need underscore lib?

angular.module(
  'JustReadIt',
  ['ngResource'],
  function($routeProvider, $locationProvider, $httpProvider) {
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
            return;
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

function SubscriptionsListController($rootScope, $scope, $resource) {
  $scope.subscrsResource = $resource('app/api/subscriptions');
  $scope.subscrsList = $scope.subscrsResource.get();

  var prevSelectedSubscr = null;

  $scope.selectSubscr = function (subscr) {
    if (prevSelectedSubscr !== null) {
      prevSelectedSubscr.isSelected = false;
    }
    
    subscr.isSelected = true;
    prevSelectedSubscr = subscr;

    $rootScope.$emit('selectSubscr', subscr.id);
  };
}

function FeedItemsController($rootScope, $scope, $resource) {
  $scope.feedItemsResource = $resource('app/api/subscriptions/:subscrId/items');

  $scope.showFeedItem = function (feedItem) {
    $rootScope.$emit('showFeedItem', feedItem);
  };

  $rootScope.$on('selectSubscr', function (ev, subscrId) {
    $scope.feedItemsList = $scope.feedItemsResource.get({ subscrId: subscrId });
  });
}

function FeedItemReaderController($rootScope, $scope, $resource) {
  $scope.feedItemContentsResource = $resource('app/api/feeditems/:feedItemId/content');

  $rootScope.$on('showFeedItem', function (ev, feedItem) {
    $scope.feedItemContentsResource.get(
      { feedItemId: feedItem.id },
      function(result) {
        $scope.feedItemContentHtml = result.contentHtml;
        console.log('content html: ' + $scope.feedItemContentHtml);
      });
  });
}
