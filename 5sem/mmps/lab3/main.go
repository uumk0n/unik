package main

import (
	"fmt"
	"math"
)

const epsilon = 1e-5

// Заданная функция
func f(x float64) float64 {
	return math.Cos(x*x) - 10*x
}

// Производная функции f(x)
func fPrime(x float64) float64 {
	return -2*x*math.Sin(x*x) - 10
}

// Метод бисекций
func bisection(a, b float64) (float64, int) {
	k := 0
	var x float64

	for (b-a)/2 > epsilon {
		x = (a + b) / 2
		if f(x) == 0 {
			break
		} else if f(x)*f(a) < 0 {
			b = x
		} else {
			a = x
		}

		k++
	}

	return x, k
}

// Метод хорд
func chord(a, b float64) (float64, int) {
	k := 0
	var x float64 = a

	for math.Abs(f(x)) > epsilon {
		x = a - (f(a)*(b-a))/(f(b)-f(a))
		if f(x) == 0 {
			break
		} else if f(x)*f(a) < 0 {
			b = x
		} else {
			a = x
		}

		k++
	}

	return x, k
}

// Метод Ньютона
func newton(x0 float64) (float64, int) {
	k := 0
	var x float64 = x0

	for math.Abs(f(x)) > epsilon {
		x = x - f(x)/fPrime(x)
		k++
	}

	return x, k
}

// Метод секущих
func secant(x0, x1 float64) (float64, int) {
	k := 0
	var x float64

	for math.Abs(f(x1)) > epsilon {
		x = x1 - (f(x1)*(x1-x0))/(f(x1)-f(x0))
		x0 = x1
		x1 = x

		k++
	}

	return x, k
}

// Функция phi(x) = arcsin(x) + 2*x - 2
func phi(x float64) float64 {
	return math.Asin(x) + 2*x - 2
}

// Метод простых итераций
func simpleIteration(phi func(float64) float64, x0 float64) (float64, int) {
	k := 0
	var x float64 = x0

	for math.Abs(phi(x)-x) > epsilon {
		x = phi(x)
		k++
	}

	return x, k
}

func main() {
	a := -2.0
	b := 2.0

	// Найдем отрезок [a, b], на котором f(x) имеет единственный корень
	for f(a)*f(b) > 0 {
		a += 0.1
		b -= 0.1
	}

	fmt.Println("Epsilon =", epsilon)

	rootBisection, iterBisection := bisection(a, b)
	fmt.Printf("Bisection: root = %v, iterations = %v\n", rootBisection, iterBisection)

	rootChord, iterChord := chord(a, b)
	fmt.Printf("Chord: root = %v, iterations = %v\n", rootChord, iterChord)

	rootNewton, iterNewton := newton(a)
	fmt.Printf("Newton: root = %v, iterations = %v\n", rootNewton, iterNewton)

	rootSecant, iterSecant := secant(a, b)
	fmt.Printf("Secant: root = %v, iterations = %v\n", rootSecant, iterSecant)

	// Добавим метод простых итераций
	rootPI, iterPI := simpleIteration(phi, a)
	fmt.Printf("Simple Iteration: root = %v, iterations = %v\n", rootPI, iterPI)
}
