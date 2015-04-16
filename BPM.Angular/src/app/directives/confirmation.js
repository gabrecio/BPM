angular.module('myapp').directive('confirmationNeeded', function() {
  return {
    priority: 1,
    terminal: true,
    link: function(scope, element, attr) {
      var msg = attr.confirmationNeeded || "¿Esta seguro?";
      var clickAction = attr.ngClick;
      element.bind('click', function() {
        if (window.confirm(msg)) {
          scope.$eval(clickAction)
        }
      });
    }
  };
});