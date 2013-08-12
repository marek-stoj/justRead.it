app.controller('FeedItemsController', ['$rootScope', '$scope', '$resource', function($rootScope, $scope, $resource) {
  $scope.feedItemsResource = $resource('app/api/subscriptions/:subscrId/items?returnRead=:returnRead');
  $scope.showReadItems = false; // TODO IMM HI: get from user prefs

  $scope.showFeedItem = function(feedItem) {
    $rootScope.$emit('showFeedItem', feedItem);
  };

  $scope.reloadItems = function() {
    var selectedSubscr = $scope.selectedSubscr;
    var showReadItems = $scope.showReadItems;

    $scope.feedItemsList =
      $scope.feedItemsResource.get(
        {
          subscrId: selectedSubscr.id,
          returnRead: showReadItems
        });
  };

  $scope.$watch('showReadItems', function(newValue, oldValue) {
    if (newValue !== oldValue) {
      $scope.reloadItems();
    }
  });

  $rootScope.$on('selectSubscr', function(ev, subscr) {
    $scope.reloadItems();
  });
}]);
