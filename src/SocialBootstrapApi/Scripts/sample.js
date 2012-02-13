(function() {
  var cube, five, square;

  square = function(x) {
    return x * x;
  };

  cube = function(x) {
    return x * x * x;
  };

  five = function(x) {
    return square(cube(x));
  };

}).call(this);
