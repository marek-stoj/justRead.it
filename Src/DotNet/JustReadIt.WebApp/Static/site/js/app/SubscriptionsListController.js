app.controller('SubscriptionsListController', ['$rootScope', '$scope', 'appModel', '$resource', '$timeout', 'objectSyncer', function($rootScope, $scope, appModel, $resource, $timeout, objectSyncer) {

  $scope.subscrsResource = $resource('app/api/subscriptions');
  $scope.showFeedsWithoutUnreadItems = false; // TODO IMM HI: get from user prefs

  var _getAllSubscriptions = function() {
    if (!$scope.subscrsList || !$scope.subscrsList.groups) {
      return [];
    }

    var flatSubscrs =
      _.reduce($scope.subscrsList.groups, function(memo, group) {
        return memo.concat(group.subscriptions);
      }, []);

    return flatSubscrs;
  };

  var _selectSubscrById = function(subscrId) {
    if (!$scope.subscrsList || !$scope.subscrsList.groups) {
      return;
    }

    var allSubscrs = _getAllSubscriptions();

    var subscrToSelect =
      _.find(allSubscrs, function(subscr) {
        return subscr.id === subscrId;
      });

    if (subscrToSelect !== undefined) {
      $scope.selectSubscr(subscrToSelect);
    }
  };

  $scope.openAddSubscriptionModal = function() {
    $rootScope.$emit('openAddSubscriptionModal');
  };

  $scope.refreshSubscrsList = function(onFinishedCallback) {
    var subscrsList =
      $scope.subscrsResource.get(function() {
        // TODO IMM HI: shouldn't we use real classes for view models?
        _.each(subscrsList.groups, function(subscrGroup) {
          _.each(subscrGroup.subscriptions, function(subscr) {
            subscr.containsUnreadItems = function() {
              return this.unreadItemsCount > 0;
            };

            subscr.isVisible = function() {
              return this.unreadItemsCount > 0 || $scope.showFeedsWithoutUnreadItems;
            };
          });

          subscrGroup.containsVisibleFeeds = function() {
            return subscrGroup.subscriptions.length > 0
              && _.some(subscrGroup.subscriptions, function(subscr) {
                return subscr.isVisible();
              });
          };
        });
        
        if ($scope.subscrsList === undefined) {
          $scope.subscrsList = {};
        }

        objectSyncer.sync($scope.subscrsList, subscrsList);

        appModel.model.setSubscrsList($scope.subscrsList);

        if (onFinishedCallback !== undefined) {
          onFinishedCallback();
        }
      });
  };

  (function refreshSubscrsListPeriodically() {
    $scope.refreshSubscrsList();

    $timeout(refreshSubscrsListPeriodically, 900000);
  })();

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

  $rootScope.$on('onSubscriptionAdded', function(ev, subscriptionId) {
    $scope.refreshSubscrsList(function() {
      _selectSubscrById(subscriptionId);
    });
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
      }
      else {
        subscription.unreadItemsCount++;
      }
    }
  });

  $rootScope.$on('onAllItemsMarkedAsRead', function(ev, subscr) {
    subscr.unreadItemsCount = 0;
  });

}]);
