package main

import (
	"fmt"
	"math"
)

func printVectorWithPrecision(v []float64, precision int) {
	for i := range v {
		fmt.Printf("%.15f\n", v[i])
	}
}

func printMatrixWithPrecision(mat [][]float64, precision int) {
	for i := range mat {
		for j := range mat[i] {
			fmt.Printf("%.15f ", mat[i][j])
		}
		fmt.Println()
	}
}

func printResults(methodName string, A [][]float64, b, x []float64) {
	fmt.Printf("\n%s:\n", methodName)

	fmt.Println("Матрица A:")
	printMatrixWithPrecision(A, 15)

	fmt.Println("\nВектор b:")
	printVectorWithPrecision(b, 15)

	fmt.Println("\nРешение системы:")
	printVectorWithPrecision(x, 15)

	// Вычислить вектор невязки
	residual := calculateResidual(A, x, b)
	fmt.Println("\nВектор невязки:")
	printVectorWithPrecision(residual, 15)

	// Вычислить нормы вектора невязки
	norm1 := calculateNorm1(residual)
	normInf := calculateNormInf(residual)
	norm2 := calculateNorm2(residual)
	fmt.Printf("Норма ||r||1 = %.15f\n", norm1)
	fmt.Printf("Норма ||r||∞ = %.15f\n", normInf)
	fmt.Printf("Норма ||r||2 = %.15f\n", norm2)

	deltaB := []float64{0.1, -0.2, 0.3} // Пример погрешности Δb
	bWithError := make([]float64, len(b))
	for i := range b {
		bWithError[i] = b[i] + deltaB[i]
	}

	xWithError := solveLinearSystem(A, bWithError)

	relativeErrors := make([]float64, len(x))
	for i := range x {
		relativeErrors[i] = math.Abs(x[i]-xWithError[i]) / math.Abs(x[i])
	}
	fmt.Println("\nВектор относительных погрешностей решения:")
	printVectorWithPrecision(relativeErrors, 15)

	practicalRelativeErrors := make([]float64, len(x))
	for i := range x {
		practicalRelativeErrors[i] = math.Abs(x[i]-xWithError[i]) / math.Abs(x[i])
	}

	theoreticalRelativeErrors := make([]float64, len(x))
	for i := range x {
		theoreticalRelativeErrors[i] = calculateTheoreticalRelativeError(A, x, b, deltaB, i)
	}

	fmt.Println("\nПрактическая относительная погрешность:")
	printVectorWithPrecision(practicalRelativeErrors, 15)

	fmt.Println("\nТеоретическая относительная погрешность:")
	printVectorWithPrecision(theoreticalRelativeErrors, 15)
}
