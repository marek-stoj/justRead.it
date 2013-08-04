angular.module('JustReadIt', ['ngResource']);

function SubscriptionsListController($scope, $resource) {
  $scope.subscrsResource = $resource('app/api/subscriptions');
  $scope.subscrsList = $scope.subscrsResource.get();

  $scope.getSubscrsCount = function() {
    if (!$scope.subscrsList.groups) {
      return 0;
    }

    var subscrsCount =
      _.reduce(
        $scope.subscrsList.groups,
        function(memo, group) {
          return memo + group.subscriptions.length;
        },
        0);

    return subscrsCount;
  };
}
