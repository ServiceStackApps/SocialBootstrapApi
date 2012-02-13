(function() {
  var KoffeeKlazz, KoffeeKlazz111, coffeeTest, coffeeTest2;

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

  KoffeeKlazz111 = (function() {

    function KoffeeKlazz111(name) {
      this.name = name;
    }

    KoffeeKlazz111.prototype.move = function(meters) {
      return alert(this.name + (" moved " + meters + "m."));
    };

    return KoffeeKlazz111;

  })();

}).call(this);
