angular.module(
  'JustReadIt',
  ['ngResource', 'ui.bootstrap', 'ngUpload'],
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
  $scope.openImportSubscriptionsModal = function() {
    $rootScope.$emit('openImportSubscriptionsModal');
  };

  $rootScope.$on('selectSubscr', function(ev, subscr) {
    $scope.selectedSubscr = subscr;
  });
}

function SubscriptionsListController($rootScope, $scope, $resource) {
  $scope.subscrsResource = $resource('app/api/subscriptions');

  $scope.refreshSubscrsList = function() {
    $scope.subscrsList = $scope.subscrsResource.get();
  };

  $scope.refreshSubscrsList();

  var prevSelectedSubscr = null;

  $scope.selectSubscr = function(subscr) {
    if (prevSelectedSubscr !== null) {
      prevSelectedSubscr.isSelected = false;
    }

    subscr.isSelected = true;
    prevSelectedSubscr = subscr;

    $rootScope.$emit('selectSubscr', subscr);
  };

  $rootScope.$on('onSubscriptionsImported', function(ev) {
    $scope.refreshSubscrsList();
  });
  
  $rootScope.$on('onUnreadFeedItemMarkedAsRead', function(ev, feedItem) {
    var flatSubscriptionsList =
      _.reduce(
        $scope.subscrsList.groups,
        function(memo, group) {
          return memo.concat(group.subscriptions);
        },
        []);

    var subscription =
      _.find(flatSubscriptionsList, function(s) {
        return s.feedId === feedItem.feedId;
      });

    if (subscription) {
      subscription.unreadItemsCount--;
      
      if (subscription.unreadItemsCount < 0) {
        subscription.unreadItemsCount = 0;
      }
      
      if (subscription.unreadItemsCount === 0) {
        subscription.containsUnreadItems = false;
      }
    }
  });
}

function FeedItemsController($rootScope, $scope, $resource) {
  $scope.feedItemsResource = $resource('app/api/subscriptions/:subscrId/items');

  $scope.showFeedItem = function(feedItem) {
    $rootScope.$emit('showFeedItem', feedItem);
  };

  $rootScope.$on('selectSubscr', function(ev, subscr) {
    $scope.feedItemsList = $scope.feedItemsResource.get({ subscrId: subscr.id });
  });
}

function FeedItemReaderController($rootScope, $scope, $resource) {
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
}

function ImportSubscriptionsController($rootScope, $scope, $timeout) {
  $scope.closeImportSubscriptionsModal = function() {
    $scope.isImportSubscriptionsModalOpen = false;
  };

  // TODO IMM HI: common modal opts
  $scope.importSubscriptionsModalOpts = {
    backdropFade: true,
    dialogFade: true
  };

  $scope.onUploadSubmit = function(content, completed) {
    if (!completed) {
      return;
    }

    // TODO IMM HI: what about localization?
    if (content.status === 'Success') {
      $scope.feedbackMessage = 'Import was successful!';
      $scope.feedbackMessageClass = 'alert-success';
      $timeout(function() {
        $scope.isImportButtonDisabled = true;
      }, 1);
      $rootScope.$emit('onSubscriptionsImported');
    }
    else if (content.status === 'Failed_NoFileUploaded') {
      $scope.feedbackMessage = 'Please select a file first.';
      $scope.feedbackMessageClass = 'alert-error';
    }
    else if (content.status === 'Failed_UnsupportedFileExtension') {
      $scope.feedbackMessage = 'Unsupported file type.';
      $scope.feedbackMessageClass = 'alert-error';
    }
    else {
      $scope.feedbackMessage = 'Import failed.';
      $scope.feedbackMessageClass = 'alert-error';
    }
  };

  $rootScope.$on('openImportSubscriptionsModal', function(ev) {
    $scope.feedbackMessage = null;
    $scope.isImportButtonDisabled = false;
    $scope.isImportSubscriptionsModalOpen = true;
  });
}
