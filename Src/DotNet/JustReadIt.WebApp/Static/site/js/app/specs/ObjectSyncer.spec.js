describe('ObjectSyncer', function() {
  it('can be created', function() {
    var objSyncer = new ObjectSyncer();

    expect(objSyncer).toNotBe(null);
  });

  describe('sync()', function() {
    var objectSyncer;

    beforeEach(function() {
      objectSyncer = new ObjectSyncer();
    });

    it('should update empty object with properties from other object', function() {
      // arrange
      var dstObj = {};
      var srcObj = { id: 1, prop1: 'hello' };

      // act
      objectSyncer.sync(dstObj, srcObj, 'id');

      // assert
      expect(dstObj.hasOwnProperty('prop1')).toBeTruthy();
      expect(dstObj.prop1).toEqual('hello');
    });

    it('should throw if the destination object is not an array and the source object is an array', function() {
      // arrange
      var dstObj = {};
      var srcObj = [{ id: 1 }];

      // act & assert
      expect(function() {
        objectSyncer.sync(dstObj, srcObj, 'id');
      }).toThrow();
    });

    it('should add an object to an array if the object doesnt\'t exist', function() {
      // arrange
      var dstObj = [];
      var srcObj = [{ id: 1, prop1: 'hello' }];

      // act
      objectSyncer.sync(dstObj, srcObj, 'id');

      // assert
      expect(dstObj.length).toEqual(1);
      expect(dstObj[0].id).toEqual(1);
    });

    it('should use \'id\' as a unique property name if none was given', function() {
      // arrange
      var dstObj = [];
      var srcObj = [{ id: 1, prop1: 'hello' }];

      // act
      objectSyncer.sync(dstObj, srcObj);

      // assert
      expect(dstObj.length).toEqual(1);
      expect(dstObj[0].id).toEqual(1);
    });

    it('should handle nested arrays', function() {
      // arrange
      var dstObj = [];
      var srcObj = [[{ id: 1, prop1: 'hello' }]];

      // act
      objectSyncer.sync(dstObj, srcObj, 'id');

      // assert
      expect(dstObj.length).toEqual(1);
      expect(dstObj[0].length).toEqual(1);
      expect(dstObj[0][0].id).toEqual(1);
    });

    it('should add missing array item at an appropriate index', function() {
      // arrange
      var dstObj = [{ id: 1 }, { id: 3 }];
      var srcObj = [{ id: 1 }, { id: 2 }, { id: 3 }];

      // act
      objectSyncer.sync(dstObj, srcObj, 'id');

      // assert
      expect(dstObj.length).toEqual(3);
      expect(dstObj[1].id).toEqual(2);
    });

    it('should remove item from the destination array that isn\'t present in the source array', function() {
      // arrange
      var dstObj = [{ id: 1 }, { id: 2 }, { id: 3 }];
      var srcObj = [{ id: 1 }, { id: 3 }];

      // act
      objectSyncer.sync(dstObj, srcObj, 'id');

      // assert
      expect(dstObj.length).toEqual(2);
    });

    it('should remove items from the destination array that aren\'t present in the source array', function() {
      // arrange
      var dstObj = [{ id: 1 }, { id: 2 }, { id: 3 }];
      var srcObj = [];

      // act
      objectSyncer.sync(dstObj, srcObj, 'id');

      // assert
      expect(dstObj.length).toEqual(0);
    });
    
    it('should handle complex object structures', function() {
      // arrange
      var dstObj = {
        groups: [
          {
            id: 1,
            title: 'Group 1',
            subscriptions: [
              {
                id: 11,
                title: 'Subscr 1.1'
              },
              {
                id: 12,
                title: 'Subscr 1.2',
                isSelected: true
              }
            ],
            isCollapsed: true,
          },
          {
            id: 2,
            title: 'Group 2',
            subscriptions: [
              {
                id: 21,
                title: 'Subscr 2.1'
              }
            ],
            isCollapsed: true
          },
          {
            id: 4,
            title: 'Group 4',
            subscriptions: [
              {
                id: 41,
                title: 'Subscr 4.1'
              },
              {
                id: 42,
                title: 'Subscr 4.2'
              },
              {
                id: 43,
                title: 'Subscr 4.3'
              }
            ]
          }
        ]
      };

      var srcObj = {
        groups: [
          {
            id: 1,
            title: 'Group 1 (updated)',
            subscriptions: [
              {
                id: 11,
                title: 'Subscr 1.1 (updated)'
              },
              {
                id: 12,
                title: 'Subscr 1.2'
              },
              {
                id: 13,
                title: 'Subscr 1.3'
              }
            ]
          },
          {
            id: 3,
            title: 'Group 3',
            subscriptions: [
              {
                id: 31,
                title: 'Subscr 3.1'
              },
              {
                id: 32,
                title: 'Subscr 3.2'
              },
              {
                id: 33,
                title: 'Subscr 3.3'
              }
            ]
          },
          {
            id: 4,
            title: 'Group 4',
            subscriptions: [
              {
                id: 43,
                title: 'Subscr 4.3 (updated)'
              }
            ]
          }
        ]
      };

      // act
      objectSyncer.sync(dstObj, srcObj, 'id');

      // assert
      // TODO IMM HI: xxx asserts
      expect(true).toBeTruthy();
    });
  });
});
