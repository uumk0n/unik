package main

import (
	"fmt"
	"math"
)

// RiemannSumLeft вычисляет левую сумму Римана для функции.
func RiemannSumLeft(f func(float64) float64, a, b float64, n int) float64 {
	h := (b - a) / float64(n)
	sum := 0.0

	for i := 0; i < n; i++ {
		x := a + float64(i)*h
		sum += f(x)
	}

	return sum * h
}

// RiemannSumRight вычисляет правую сумму Римана для функции.
func RiemannSumRight(f func(float64) float64, a, b float64, n int) float64 {
	h := (b - a) / float64(n)
	sum := 0.0

	for i := 1; i <= n; i++ {
		x := a + float64(i)*h
		sum += f(x)
	}

	return sum * h
}

// RiemannSumMidpoint вычисляет среднюю сумму Римана для функции.
func RiemannSumMidpoint(f func(float64) float64, a, b float64, n int) float64 {
	h := (b - a) / float64(n)
	sum := 0.0

	for i := 0; i < n; i++ {
		x := a + (float64(i)+0.5)*h
		sum += f(x)
	}

	return sum * h
}

// RiemannSumTrapezoid вычисляет трапециевидную сумму Римана для функции.
func RiemannSumTrapezoid(f func(float64) float64, a, b float64, n int) float64 {
	h := (b - a) / float64(n)
	sum := 0.0

	for i := 1; i < n; i++ {
		x := a + float64(i)*h
		sum += f(x)
	}

	return h * (0.5*f(a) + sum + 0.5*f(b))
}

// SimpsonRule вычисляет интеграл с использованием правила Симпсона.
func SimpsonRule(f func(float64) float64, a, b float64, n int) float64 {
	h := (b - a) / float64(n)
	sum := f(a) + f(b)

	for i := 1; i < n; i++ {
		x := a + float64(i)*h
		weight := 2.0
		if i%2 != 0 {
			weight = 4.0
		}
		sum += weight * f(x)
	}

	return sum * h / 3.0
}

func main() {
	f := func(x float64) float64 {
		return math.Sin(x)
	}

	a := 0.0
	b := math.Pi
	eps := 1e-5

	F := func(x float64) float64 {
		return -math.Cos(x)
	}

	I := F(b) - F(a)

	fmt.Println("I =", I)
	fmt.Printf("%-25s%-23s%-15s\n", "Метод", "Приближенное I", "Число Узлов")

	for _, method := range []struct {
		name string
		fn   func(func(float64) float64, float64, float64, int) float64
	}{
		{"RiemannSumLeft", RiemannSumLeft},
		{"RiemannSumRight", RiemannSumRight},
		{"RiemannSumMidpoint", RiemannSumMidpoint},
		{"RiemannSumTrapezoid", RiemannSumTrapezoid},
		{"SimpsonRule", SimpsonRule},
	} {
		var nNodes int
		for {
			nNodes++
			prevResult := method.fn(f, a, b, nNodes-1)
			currResult := method.fn(f, a, b, nNodes)
			err := math.Abs(currResult - prevResult)

			if err < eps {
				fmt.Printf("%-25s%-23.6f%-15d\n", method.name, currResult, nNodes)
				break
			}
		}
	}
}
