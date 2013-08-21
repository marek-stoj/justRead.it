app.controller('FeedItemReaderController', ['$rootScope', '$scope', 'commonOptionsSvc', '$resource', function($rootScope, $scope, commonOptionsSvc, $resource) {
  
  $scope.feedItemContentsResource = $resource('app/api/feeditems/:feedItemId/content');
  $scope.toggleFeedItemIsReadResource = $resource('app/api/feeditems/:feedItemId/toggle-is-read?isRead=:isRead', { feedItemId: '@feedItemId', isRead: '@isRead' });

  $scope.closeReaderModal = function() {
    $scope.isReaderModalOpen = false;
  };

  $scope.readerModalOpts = commonOptionsSvc.modalOpts;
  $scope.readerModalOpts.dialogClass = 'modal feed-item-reader-modal';

  $scope.markAsRead = function(feedItem) {
    toggleFeedItemIsRead(feedItem, true);
  };

  $scope.markAsUnread = function(feedItem) {
    toggleFeedItemIsRead(feedItem, false);
  };

  var toggleFeedItemIsRead = function(feedItem, isRead) {
    if (feedItem.isRead === isRead) {
      return;
    }

    feedItem.isRead = isRead;
    
    // TODO IMM HI: there has to be a better way to sync server model
    $scope.toggleFeedItemIsReadResource.save({ feedItemId: $scope.feedItem.id, isRead: isRead });
    
    // TODO IMM HI: there has to be a better way for communicating between controllers as well ;)
    $rootScope.$emit('onFeedItemIsReadChanged', feedItem);
  };

  $rootScope.$on('showFeedItem', function(ev, feedItem) {
    $scope.feedItem = feedItem;

    toggleFeedItemIsRead(feedItem, true);

    $scope.isReaderModalOpen = true;
    $scope.feedItemContentHtml = '<div>Loading...</div>';

    $scope.feedItemContentsResource.get(
      { feedItemId: feedItem.id },
      function(result) {
        $scope.feedItemContentHtml = result.contentHtml;
      });
  });
  
}]);
