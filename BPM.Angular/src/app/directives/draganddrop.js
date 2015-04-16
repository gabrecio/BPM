angular.module('myapp')
  .directive('draggable', function() {
    return {
      restrict: 'A',
      scope: {
        draggable: '@',
        draggableData: '=',
        draggableCollection: '@'
      },
      link: function(scope, element, attrs) {
        if (scope.draggable !== 'true') {
          throw 'You must set draggable to true to enable drag events.';
        }
        if (scope.draggableData === undefined) {
          throw 'You must set draggableData with what are you dragging.';
        }
        element.on('dragstart', function(event) {
          var data = {
            collection: scope.draggableCollection,
            value: scope.draggableData
          };

          event.originalEvent.dataTransfer.setData('application/json', JSON.stringify(data));
        });
      }
    };
  })
  .directive('dropzone', function() {
    return {
      restrict: 'A',
      scope: {
        dropzone: '&',
        dropzoneTarget: '=',
        dropzoneSource: '@'
      },
      link: function(scope, element, attrs) {
        if (scope.dropzone() === undefined) {
          throw 'You must set a callback';
        }
        // TODO : why not access the values directly from the scope?
        var dropzoneScope = scope;
        var dropzoneTarget = scope.dropzoneTarget;
        var dropzoneHandler = scope.dropzone();
        var dropzoneSource = scope.dropzoneSource;
        element.on('dragover', function(event) {
          event.preventDefault();
        });
        element.on('drop', function(event) {
          var data = JSON.parse(event.originalEvent.dataTransfer.getData('application/json'));

          if (dropzoneSource === undefined || dropzoneSource === data.collection) {
            dropzoneHandler(data.value, data.collection, dropzoneTarget);
          }
        });
      }
    }
  });