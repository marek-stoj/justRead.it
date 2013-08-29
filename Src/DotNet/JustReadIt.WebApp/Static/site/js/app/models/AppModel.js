var AppModel = function() {

  var _subscrsList = {};

  this.setSubscrsList = function(value) {
    _subscrsList = value;
  };

  this.getAllCategories = function() {
    if (!_subscrsList) {
      return [];
    }

    return _.map(_subscrsList.groups, function(group) {
      return group.title;
    });
  };

};
