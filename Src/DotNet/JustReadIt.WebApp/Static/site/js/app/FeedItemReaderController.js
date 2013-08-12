app.controller('FeedItemReaderController', ['$rootScope', '$scope', '$resource', function($rootScope, $scope, $resource) {
  $scope.feedItemContentsResource = $resource('app/api/feeditems/:feedItemId/content');
  $scope.markFeedItemAsReadResource = $resource('app/api/feeditems/:feedItemId/mark-as-read', { feedItemId: '@feedItemId' });

  $scope.closeReaderModal = function() {
    $scope.isReaderModalOpen = false;
  };

  // TODO IMM HI: common modal opts
  $scope.readerModalOpts = {
    dialogClass: 'modal feed-item-reader-modal',
    backdropFade: true,
    dialogFade: true
  };

  $rootScope.$on('showFeedItem', function(ev, feedItem) {
    $scope.feedItem = feedItem;

    if (!$scope.feedItem.isRead) {
      $scope.feedItem.isRead = true;
      $scope.markFeedItemAsReadResource.save({ feedItemId: $scope.feedItem.id }); // TODO IMM HI: there has to be a better way to sync server model
      $rootScope.$emit('onUnreadFeedItemMarkedAsRead', feedItem); // TODO IMM HI: there has to be a better way for this as well ;)
    }

    $scope.isReaderModalOpen = true;
    $scope.feedItemContentHtml = '<div>Loading...</div>';

    $scope.feedItemContentsResource.get(
      { feedItemId: feedItem.id },
      function(result) {
        $scope.feedItemContentHtml = result.contentHtml;
      });
  });
}]);
