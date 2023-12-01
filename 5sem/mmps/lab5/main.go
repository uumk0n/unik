package main

import (
	"fmt"
	"math"
	"math/rand"
	"sort"
)

func gauss(A [][]float64, b []float64) []float64 {
	matrix := make([][]float64, len(A))
	for i := range matrix {
		matrix[i] = make([]float64, len(A[i])+1)
		copy(matrix[i], A[i])
		matrix[i] = append(matrix[i], b[i])
	}

	m, n := len(matrix), len(matrix[0])

	for i := 0; i < m; i++ {
		for j := i + 1; j < m; j++ {
			ratio := matrix[j][i] / matrix[i][i]
			for k := 0; k < n; k++ {
				matrix[j][k] -= ratio * matrix[i][k]
			}
		}
	}

	for i := m - 1; i >= 0; i-- {
		diagonal := matrix[i][i]
		for j := i; j < n; j++ {
			matrix[i][j] /= diagonal
		}
		for j := i - 1; j >= 0; j-- {
			ratio := matrix[j][i]
			for k := 0; k < n; k++ {
				matrix[j][k] -= ratio * matrix[i][k]
			}
		}
	}

	result := make([]float64, m)
	for i := 0; i < m; i++ {
		result[i] = matrix[i][n-1]
	}

	return result
}

func mse(y, yHat []float64) float64 {
	var sum float64
	for i := range y {
		sum += math.Pow(y[i]-yHat[i], 2)
	}
	return sum / float64(len(y))
}

func lsm(x, y []float64, m int) []float64 {
	n := len(x)
	pows := make([][]float64, n)
	for i := range pows {
		pows[i] = make([]float64, m+1)
		for j := 0; j <= m; j++ {
			pows[i][j] = math.Pow(x[i], float64(j))
		}
	}

	var A [][]float64
	A = make([][]float64, m+1)
	for i := range A {
		A[i] = make([]float64, m+1)
		for j := range A[i] {
			for k := 0; k < n; k++ {
				A[i][j] += pows[k][i] * pows[k][j]
			}
		}
	}

	var b []float64
	b = make([]float64, m+1)
	for i := range b {
		for j := 0; j < n; j++ {
			b[i] += pows[j][i] * y[j]
		}
	}

	return gauss(A, b)
}

func main() {
	n := 2

	rand.Seed(42) // For reproducibility
	x := make([]float64, n)
	for i := range x {
		x[i] = rand.Float64()*20 - 10
	}
	sort.Float64s(x)

	y := make([]float64, n)
	for i := range y {
		y[i] = math.Pow(x[i], 3) + rand.NormFloat64()*10
	}

	fmt.Println("Data:")
	fmt.Println("x:", x)
	fmt.Println("y:", y)

	m := n
	w := lsm(x, y, m)
	fmt.Println("Coefficients (w):", w)

	yHat := make([]float64, n)
	for i := range yHat {
		for j := range w {
			yHat[i] += w[j] * math.Pow(x[i], float64(j))
		}
	}

	fmt.Printf("MSE for m=%d: %f\n", m, mse(y, yHat))
}
