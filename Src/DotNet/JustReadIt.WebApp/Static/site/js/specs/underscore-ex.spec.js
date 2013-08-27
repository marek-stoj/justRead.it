describe('underscore-ex', function() {

  describe('findIndex', function() {

    it('finds nothing in empty collection', function() {
      // arrange
      var array = [];
      
      // act
      var index =
        _.findIndex(array, function(elem) {
          return true;
        });
      
      // assert
      expect(index).toEqual(-1);
    });

    it('finds nothing in non-empty collection if no element satisfies the predicate', function() {
      // arrange
      var array = [1, 3, 5];
      
      // act
      var index =
        _.findIndex(array, function(elem) {
          return elem % 2 === 0;
        });
      
      // assert
      expect(index).toEqual(-1);
    });

    it('finds the first element that satisfies the predicate', function() {
      // arrange
      var array = [2, 4];
      
      // act
      var index =
        _.findIndex(array, function(elem) {
          return elem % 2 === 0;
        });
      
      // assert
      expect(index).toEqual(0);
    });

    it('finds the first element that satisfies the predicate when it\'s at the end of collection', function() {
      // arrange
      var array = [1, 3, 5, 6];
      
      // act
      var index =
        _.findIndex(array, function(elem) {
          return elem % 2 === 0;
        });
      
      // assert
      expect(index).toEqual(3);
    });

  });
  
});
