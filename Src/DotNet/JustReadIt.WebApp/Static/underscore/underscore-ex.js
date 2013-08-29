_.mixin({
  findIndex: function(obj, iterator, context) {
    var foundIndex = -1;

    _.find(
      obj,
      function(value, index, list) {
        var isSatisfied = iterator.call(context, value, index, list);

        if (isSatisfied) {
          foundIndex = index;
        }

        return isSatisfied;
      },
      context);

    return foundIndex;
  }
});
