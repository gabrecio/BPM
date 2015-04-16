angular.module('myapp').filter('comment', function() {
  return function(input, pagingInfo) {
    if (!input) {
      return input;
    }
    return input.slice(parseInt((pagingInfo.page - 1) * pagingInfo.itemsPerPage), parseInt(pagingInfo.page * pagingInfo.itemsPerPage + 1) - 1);
  }
});