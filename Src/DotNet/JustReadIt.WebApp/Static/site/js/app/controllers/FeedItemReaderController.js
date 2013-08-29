app.controller('FeedItemReaderController', ['$rootScope', '$scope', 'commonOptionsSvc', '$resource', '$timeout', function($rootScope, $scope, commonOptionsSvc, $resource, $timeout) {
  
  $scope.feedItemContentsResource = $resource('app/api/feeditems/:feedItemId/content');
  $scope.toggleFeedItemIsReadResource = $resource('app/api/feeditems/:feedItemId/toggle-is-read?isRead=:isRead', { feedItemId: '@feedItemId', isRead: '@isRead' });

  $scope.isReaderModalOpen = false;

  $scope.closeReaderModal = function() {
    _closeTextSelectionContextMenu();
    $scope.isReaderModalOpen = false;
  };

  $scope.readerModalOpts = commonOptionsSvc.modalOpts;
  $scope.readerModalOpts.dialogClass = 'modal feed-item-reader-modal';

  $scope.markAsRead = function(feedItem) {
    _toggleFeedItemIsRead(feedItem, true);
  };

  $scope.markAsUnread = function(feedItem) {
    _toggleFeedItemIsRead(feedItem, false);
  };
  
  var _toggleFeedItemIsRead = function(feedItem, isRead) {
    if (feedItem.isRead === isRead) {
      return;
    }

    feedItem.isRead = isRead;

    // TODO IMM HI: there has to be a better way to sync server model
    $scope.toggleFeedItemIsReadResource.save({ feedItemId: $scope.feedItem.id, isRead: isRead });

    // TODO IMM HI: there has to be a better way for communicating between controllers as well ;)
    $rootScope.$emit('onFeedItemIsReadChanged', feedItem);
  };

  var _openTextSelectionContextMenu = function(x, y, selectedText) {
    $('#feed-item-reader-ctx-menu')
      .css({ left: x + 'px', top: y + 'px' })
      .show();
  };

  var _closeTextSelectionContextMenu = function() {
    $('#feed-item-reader-ctx-menu')
      .hide();
  };

  $(document).ready(function() {
    $(document).mouseup(function(ev) {
      if (!$scope.isReaderModalOpen) {
        return;
      }

      var $target = $(ev.target);

      if ($target.attr('id') === 'feed-item-reader-ctx-menu'
       || $target.parents('#feed-item-reader-ctx-menu').length > 0) {
        return;
      }

      $timeout(function() {
        var selectedText = jri.util.getSelectedText();

        if (selectedText === null || selectedText === '') {
          return;
        }

        _openTextSelectionContextMenu(ev.pageX, ev.pageY, selectedText);
      }, 1);
    });

    $(document).mousedown(function(ev) {
      var $target = $(ev.target);

      if ($target.attr('id') === 'feed-item-reader-ctx-menu'
       || $target.parents('#feed-item-reader-ctx-menu').length > 0) {
        return;
      }

      _closeTextSelectionContextMenu();
    });
  });

  $rootScope.$on('showFeedItem', function(ev, feedItem) {
    $scope.feedItem = feedItem;

    _toggleFeedItemIsRead(feedItem, true);

    $scope.isReaderModalOpen = true;
    $scope.feedItemContentHtml = '<div>Loading...</div>';

    $scope.feedItemContentsResource.get(
      { feedItemId: feedItem.id },
      function(result) {
        $scope.feedItemContentHtml = result.contentHtml;
      });
  });
  
}]);
