app.controller('AppController', ['$rootScope', '$scope', function($rootScope, $scope) {
  
  $scope.openImportSubscriptionsModal = function() {
    $rootScope.$emit('openImportSubscriptionsModal');
  };

  $scope.openAddSubscriptionModal = function() {
    $rootScope.$emit('openAddSubscriptionModal');
  };

  $rootScope.$on('selectSubscr', function(ev, subscr) {
    $scope.selectedSubscr = subscr;
  });
  
}]);
