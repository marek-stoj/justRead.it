app.controller('AddSubscriptionController', ['$rootScope', '$scope', '$timeout', function($rootScope, $scope, $timeout) {
  
  $scope.closeAddSubscriptionModal = function() {
    $scope.isAddSubscriptionModalOpen = false;
  };

  // TODO IMM HI: common modal opts
  $scope.addSubscriptionModalOpts = {
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

  $rootScope.$on('openAddSubscriptionModal', function(ev) {
    $scope.feedbackMessage = null;
    $scope.isImportButtonDisabled = false;
    $scope.isAddSubscriptionModalOpen = true;
  });
  
}]);
