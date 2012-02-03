
coffeeTest = (x) -> x * x * x
coffeeTest2 = (x) -> x * x * x

class KoffeeKlazz
	constructor: (@name) ->
	move: (meters) ->
		alert @name + " moved #{meters}m."