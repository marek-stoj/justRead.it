var ObjectSyncer = function() {

  function _createMapFromArray(array, uniquePropName) {
    var map = {};

    for (var i = 0; i < array.length; i++) {
      var obj = array[i];

      map[obj[uniquePropName]] = obj;
    }

    return map;
  }
  
  function _isArray(obj) {
    return Object.prototype.toString.call(obj) === '[object Array]';
  }

  this.sync = function(dstObj, srcObj, uniquePropertyName) {
    var i;

    uniquePropertyName = uniquePropertyName || 'id';

    if (!_isArray(srcObj)) {
      for (var propName in srcObj) {
        dstObj[propName] = srcObj[propName];
      }
    }
    else {
      if (!_isArray(dstObj)) {
        throw 'The destination object must be an array if the source object is an array.';
      }

      // add new or update existing elements
      var dstObjsMap = _createMapFromArray(dstObj, uniquePropertyName);

      for (i = 0; i < srcObj.length; i++) {
        var srcElem = srcObj[i];
        var srcElemUniquePropertyValue = srcElem[uniquePropertyName];
        var dstElem = dstObjsMap[srcElemUniquePropertyValue];

        if (dstElem === undefined) {
          if (!_isArray(srcElem)) {
            dstElem = {};
          }
          else {
            dstElem = [];
          }
          
          dstObj.splice(i, 0, dstElem);
        }

        arguments.callee(dstElem, srcElem, uniquePropertyName);
      }
      
      // remove elements
      var srcObjsMap = _createMapFromArray(srcObj, uniquePropertyName);

      for (i = dstObj.length - 1; i >= 0; i--) {
        if (srcObjsMap[dstObj[i][uniquePropertyName]] === undefined) {
          dstObj.splice(i, 1);
        }
      }
    }
  };

};
