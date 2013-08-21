app.controller('ImportSubscriptionsController', ['$rootScope', '$scope', 'commonOptionsSvc', '$timeout', function($rootScope, $scope, commonOptionsSvc, $timeout) {
  
  $scope.closeImportSubscriptionsModal = function() {
    $scope.isImportSubscriptionsModalOpen = false;
  };

  $scope.importSubscriptionsModalOpts = commonOptionsSvc.modalOpts;

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
  
}]);
