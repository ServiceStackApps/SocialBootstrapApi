(function() {
  var KoffeeKlazz, coffeeTest, coffeeTest2;

  coffeeTest = function(x) {
    return x * x * x;
  };

  coffeeTest2 = function(x) {
    return x * x * x;
  };

  KoffeeKlazz = (function() {

    function KoffeeKlazz(name) {
      this.name = name;
    }

    KoffeeKlazz.prototype.move = function(meters) {
      return alert(this.name + (" moved " + meters + "m."));
    };

    return KoffeeKlazz;

  })();

}).call(this);
