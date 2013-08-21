app.controller('AddSubscriptionController', ['$rootScope', '$scope', 'commonOptionsSvc', '$resource', '$timeout', function($rootScope, $scope, commonOptionsSvc, $resource, $timeout) {
  
  $scope.addSubscrResource = $resource('app/api/subscriptions/add');

  $scope.closeAddSubscriptionModal = function() {
    $scope.isAddSubscriptionModalOpen = false;
  };

  $scope.addSubscriptionModalOpts = commonOptionsSvc.modalOpts;

  $scope.addSubscription = function() {
    // TODO IMM HI: xxx category
    $scope.addSubscrResource.save({
      url: $scope.url,
    }, function(response) {
      $scope.addSubscriptionForm.url.$setValidity('isValid', response.isUrlValid);
    });
  };

  $rootScope.$on('openAddSubscriptionModal', function(ev) {
    $scope.feedbackMessage = null;
    $scope.isImportButtonDisabled = false;
    $scope.isAddSubscriptionModalOpen = true;
  });
  
}]);
