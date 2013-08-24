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
  
  function _isOfSimpleType(obj) {
    if (obj === null || obj === undefined) {
      return true;
    }

    var objCtor = obj.constructor;

    return objCtor === Number
        || objCtor === String
        || objCtor === Boolean;
  }
  
  function _isOfFunctionType(obj) {
    return _.isFunction(obj);
  }

  function _sync(dstObj, srcObj, uniquePropertyName) {
    var i;
    var uniquePropertyValue;

    if (!_isArray(srcObj)) {
      for (var propName in srcObj) {
        var srcPropValue = srcObj[propName];

        if (_isOfSimpleType(srcPropValue) || _isOfFunctionType(srcPropValue)) {
          dstObj[propName] = srcPropValue;
        }
        else {
          if (dstObj[propName] === undefined) {
            if (!_isArray(srcObj[propName])) {
              dstObj[propName] = {};
            }
            else {
              dstObj[propName] = [];
            }
          }

          arguments.callee(dstObj[propName], srcObj[propName], uniquePropertyName);
        }
      }
    }
    else {
      if (!_isArray(dstObj)) {
        dstObj = [];
      }

      // add new or update existing elements
      var dstObjsMap = _createMapFromArray(dstObj, uniquePropertyName);

      for (i = 0; i < srcObj.length; i++) {
        var srcElem = srcObj[i];

        uniquePropertyValue = srcElem[uniquePropertyName];

        if (uniquePropertyValue === undefined) {
          throw 'No property named \'' + uniquePropertyName + '\' on source array element.';
        }

        var dstElem = dstObjsMap[uniquePropertyValue];

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
        uniquePropertyValue = dstObj[i][uniquePropertyName];

        if (uniquePropertyValue === undefined) {
          throw 'No property named \'' + uniquePropertyName + '\' on destination object.';
        }

        if (srcObjsMap[uniquePropertyValue] === undefined) {
          dstObj.splice(i, 1);
        }
      }
    }
  };

  this.sync = function(dstObj, srcObj, uniquePropertyName) {
    if (_isArray(srcObj) && !_isArray(dstObj)) {
      throw 'Destination object is not array but the source object is an array.';
    }

    if (!_isArray(srcObj) && _isArray(dstObj)) {
      throw 'Destination object is an array but the source object is not an array.';
    }

    uniquePropertyName = uniquePropertyName || 'id';

    _sync(dstObj, srcObj, uniquePropertyName);
  };

};
