app.controller('AppController', ['$rootScope', '$scope', function($rootScope, $scope) {
  $scope.openImportSubscriptionsModal = function() {
    $rootScope.$emit('openImportSubscriptionsModal');
  };

  $rootScope.$on('selectSubscr', function(ev, subscr) {
    $scope.selectedSubscr = subscr;
  });
}]);
