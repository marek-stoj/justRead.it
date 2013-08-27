app.controller('AddSubscriptionController', ['$rootScope', '$scope', 'appModel', 'commonOptionsSvc', '$resource', '$timeout', function($rootScope, $scope, appModel, commonOptionsSvc, $resource, $timeout) {
  
  $scope.addSubscrResource = $resource('app/api/subscriptions/add');

  $scope.closeAddSubscriptionModal = function() {
    $scope.isAddSubscriptionModalOpen = false;
  };

  $scope.addSubscriptionModalOpts = commonOptionsSvc.modalOpts;

  var _handleAddSubscriptionResponse = function(response) {
    if (response.status === 'Success') {
      $scope.feedbackMessage = 'Subscribed successfully!';
      $scope.feedbackMessageClass = 'alert-success';

      $rootScope.$emit('onSubscriptionAdded', response.subscriptionId);
    }
    else if (response.status == 'Failed_InvalidInputData') {
      $scope.feedbackMessage = 'Invalid input data.';
      $scope.feedbackMessageClass = 'alert-warn';
      
      $scope.addSubscriptionForm.url.$setValidity('isValid', response.isUrlValid);
    }
    else {
      $scope.feedbackMessage = 'Failed to subscribe.';
      $scope.feedbackMessageClass = 'alert-error';
    }
  };

  $scope.addSubscription = function() {
    var ajaxLoaderTimeoutPromise =
      $timeout(function() {
        $scope.isAjaxLoaderVisible = true;
      }, 250);

    $scope.isSubscribeButtonDisabled = true;

    var subscrCategory =
      $scope.newCategory !== undefined && $.trim($scope.newCategory) !== ''
        ? $scope.newCategory
        : $scope.category;

    $scope.addSubscrResource.save({
      url: $scope.url,
      category: subscrCategory
    }, function(response) {
      $timeout.cancel(ajaxLoaderTimeoutPromise);

      $scope.isAjaxLoaderVisible = false;
      $scope.isSubscribeButtonDisabled = false;

      _handleAddSubscriptionResponse(response);
    });
  };

  $rootScope.$on('openAddSubscriptionModal', function(ev) {
    $scope.categories = appModel.model.getAllCategories();

    var indexOfUncategorizedCategory =
      _.findIndex($scope.categories, function(category) {
        return category === 'Uncategorized'; // TODO IMM HI: what about localization?
      });

    if (indexOfUncategorizedCategory > -1) {
      $scope.categories.splice(0, 0, $scope.categories[indexOfUncategorizedCategory]);
      $scope.categories.splice(indexOfUncategorizedCategory + 1, 1);
    }

    $scope.url = null;
    $scope.category = 'Uncategorized';
    $scope.newCategory = '';

    $scope.feedbackMessage = null;
    $scope.isSubscribeButtonDisabled = false;

    $scope.isAddSubscriptionModalOpen = true;
  });
  
}]);
