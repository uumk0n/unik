package main

import (
	"fmt"
	"math"
)

type Table struct {
	X []float64
	Y []float64
}

func lagrangeInterpolation(x float64, table Table) float64 {
	result := 0.0

	for i := 0; i < len(table.X); i++ {
		term := table.Y[i]
		for j := 0; j < len(table.X); j++ {
			if i != j {
				term *= (x - table.X[j]) / (table.X[i] - table.X[j])
			}
		}
		result += term
	}

	return result
}

func newtonInterpolation(x float64, table Table) float64 {
	result := table.Y[0]
	factor := 1.0

	for i := 1; i < len(table.X); i++ {
		factor *= (x - table.X[i-1]) / float64(i)
		result += factor * dividedDifference(table, i)
	}

	return result
}

func dividedDifference(table Table, k int) float64 {
	if k == 1 {
		return (table.Y[1] - table.Y[0]) / (table.X[1] - table.X[0])
	}

	if len(table.X) < k || k < 1 {
		// В этом месте следует добавить соответствующую обработку ошибок
		return 0.0
	}

	result := (dividedDifference(table, k-1) - dividedDifference(table, k-2)) / (table.X[k] - table.X[k-1])
	return result
}

func theoreticalErrorEstimation(x float64, table Table, n int) float64 {
	maxDerivative := finiteDifferences(x, table, n)
	h := table.X[1] - table.X[0]
	return math.Abs((maxDerivative * math.Pow(h, float64(n+1))) / float64(factorial(uint64(n+1))))
}

func finiteDifferences(x float64, table Table, n int) float64 {
	h := table.X[1] - table.X[0]
	result := table.Y[n]

	for k := 1; k <= n; k++ {
		term := table.Y[k]
		for j := 0; j < k; j++ {
			term *= (x - table.X[j]) / float64(j+1)
		}
		result += term / math.Pow(h, float64(k))
	}

	return result
}

func factorial(n uint64) uint64 {
	if n <= 1 {
		return 1
	}
	result := uint64(1)
	for i := uint64(2); i <= n; i++ {
		result *= i
	}
	return result
}

func main() {
	table := Table{
		X: []float64{1.0, 2.0, 3.0, 4.0, 5.0},
		Y: []float64{1.0, 4.0, 9.0, 16.0, 25.0},
	}

	n := 2

	fmt.Println("xi\tf(xi)\t\tLn(xi)\t\tPn(xi)\t\tf(xi) - Ln(xi)\tRn(xi)")

	for i := 0; i < len(table.X); i++ {
		xi := table.X[i]
		fi := table.Y[i]
		lagrangeResult := lagrangeInterpolation(xi, table)
		newtonResult := newtonInterpolation(xi, table)
		fMinuLn := fi - lagrangeResult
		rnxi := theoreticalErrorEstimation(xi, table, n)

		fmt.Printf("%.2f\t%.6f\t%.6f\t%.6f\t%.6f\t%.6f\n", xi, fi, lagrangeResult, newtonResult, fMinuLn, rnxi)
	}
}
