// TODO IMM HI: do we really need underscore lib?

angular.module(
  'JustReadIt',
  ['ngResource', 'ui.bootstrap'],
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

function AppController($rootScope, $scope) {
  $rootScope.$on('selectSubscr', function (ev, subscr) {
    $scope.selectedSubscr = subscr;
  });
}

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

    $rootScope.$emit('selectSubscr', subscr);
  };
}

function FeedItemsController($rootScope, $scope, $resource) {
  $scope.feedItemsResource = $resource('app/api/subscriptions/:subscrId/items');

  $scope.showFeedItem = function (feedItem) {
    $rootScope.$emit('showFeedItem', feedItem);
  };

  $rootScope.$on('selectSubscr', function (ev, subscr) {
    $scope.feedItemsList = $scope.feedItemsResource.get({ subscrId: subscr.id });
  });
}

function FeedItemReaderController($rootScope, $scope, $resource) {
  $scope.feedItemContentsResource = $resource('app/api/feeditems/:feedItemId/content');

  $scope.openReaderModal = function () {
    $scope.isReaderModalOpen = true;
  };

  $scope.closeReaderModal = function () {
    $scope.isReaderModalOpen = false;
  };

  $scope.readerModalOpts = {
    dialogClass: 'modal feed-item-reader-modal',
    backdropFade: true,
    dialogFade: true
  };

  $rootScope.$on('showFeedItem', function (ev, feedItem) {
    $scope.feedItem = feedItem;

    $scope.isReaderModalOpen = true;
    $scope.feedItemContentHtml = '<div>Loading...</div>';

    $scope.feedItemContentsResource.get(
      { feedItemId: feedItem.id },
      function (result) {
        $scope.feedItemContentHtml = result.contentHtml;
      });
  });
}
