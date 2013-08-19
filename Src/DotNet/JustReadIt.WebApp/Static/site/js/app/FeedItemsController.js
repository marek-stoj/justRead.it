app.controller('FeedItemsController', ['$rootScope', '$scope', '$resource', function($rootScope, $scope, $resource) {
  
  $scope.feedItemsResource = $resource('app/api/subscriptions/:subscrId/items?returnRead=:returnRead');
  $scope.markAllFeedItemsAsReadResource = $resource('app/api/subscriptions/:subscrId/mark-all-items-as-read', { subscrId: '@subscrId' });
  $scope.showReadItems = false; // TODO IMM HI: get from user prefs

  $scope.showFeedItem = function(feedItem) {
    $rootScope.$emit('showFeedItem', feedItem);
  };

  $scope.reloadItems = function() {
    var selectedSubscr = $scope.selectedSubscr;
    var showReadItems = $scope.showReadItems;
    
    if ($scope.feedItemsList) {
      $scope.feedItemsList.containsUnreadItems = function() {
        return false;
      };
    }

    $scope.feedItemsList =
      $scope.feedItemsResource.get({
          subscrId: selectedSubscr.id,
          returnRead: showReadItems
        },
        function() {
          $scope.feedItemsList.containsUnreadItems = function() {
            return _.some(this.items, function(feedItem) {
              return !feedItem.isRead;
            });
          };
        });
  };

  $scope.markAllItemsAsRead = function() {
    var selectedSubscr = $scope.selectedSubscr;

    $scope.markAllFeedItemsAsReadResource.save(
      {
        subscrId: selectedSubscr.id
      });
    
    if ($scope.showReadItems) {
      _.each($scope.feedItemsList.items, function(feedItem) {
        feedItem.isRead = true;
      });
    }
    else {
      $scope.feedItemsList.items = [];
    }

    $rootScope.$emit('onAllItemsMarkedAsRead', selectedSubscr);
  };

  $scope.$watch('showReadItems', function(newValue, oldValue) {
    if (newValue !== oldValue) {
      $scope.reloadItems();
    }
  });

  $rootScope.$on('selectSubscr', function(ev, subscr) {
    $scope.showReadItems = false;
    $scope.reloadItems();
  });
  
}]);
