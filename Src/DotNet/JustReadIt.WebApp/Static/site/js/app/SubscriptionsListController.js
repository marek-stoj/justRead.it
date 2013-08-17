app.controller('SubscriptionsListController', ['$rootScope', '$scope', '$resource', function($rootScope, $scope, $resource) {

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

  $rootScope.$on('onFeedItemIsReadChanged', function(ev, feedItem) {
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
      if (feedItem.isRead) {
        subscription.unreadItemsCount--;

        if (subscription.unreadItemsCount < 0) {
          subscription.unreadItemsCount = 0;
        }

        if (subscription.unreadItemsCount === 0) {
          subscription.containsUnreadItems = false;
        }
      }
      else {
        subscription.unreadItemsCount++;

        if (subscription.unreadItemsCount > 0) {
          subscription.containsUnreadItems = true;
        }
      }
    }
  });

  $rootScope.$on('onAllItemsMarkedAsRead', function(ev, subscr) {
    subscr.unreadItemsCount = 0;
    subscr.containsUnreadItems = false;
  });

}]);
