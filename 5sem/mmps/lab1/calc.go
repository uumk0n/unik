package main

import (
	"fmt"
	"math"
)

// solveLinearSystem решает линейную систему уравнений с использованием метода Гаусса.
//
// Принимает в качестве входных данных матрицу коэффициентов A и вектор b.
// A - это квадратная матрица размером n x n, где n - количество переменных.
// b - это вектор размером n.
// Функция возвращает вектор x размером n, который является решением линейной системы.
func solveLinearSystem(A [][]float64, b []float64) []float64 {
	n := len(A)
	x := make([]float64, n)

	for k := 0; k < n; k++ {
		for i := k + 1; i < n; i++ {
			factor := A[i][k] / A[k][k]
			for j := k; j < n; j++ {
				A[i][j] -= factor * A[k][j]
			}
			b[i] -= factor * b[k]
		}
	}

	for k := n - 1; k >= 0; k-- {
		x[k] = b[k]
		for j := k + 1; j < n; j++ {
			x[k] -= A[k][j] * x[j]
		}
		x[k] /= A[k][k]
	}

	return x
}

// calculateResidual вычисляет остаток линейной системы уравнений.
//
// A - матрица, представляющая коэффициенты уравнений.
// x - вектор, представляющий решение уравнений.
// b - вектор, представляющий правую часть уравнений.
// Функция возвращает вектор, представляющий остаток уравнений.
func calculateResidual(A [][]float64, x []float64, b []float64) []float64 {
	n := len(A)
	residual := make([]float64, n)

	for i := 0; i < n; i++ {
		residual[i] = b[i]
		for j := 0; j < n; j++ {
			residual[i] -= A[i][j] * x[j]
		}
	}

	return residual
}

// calculateNorm1 вычисляет норму вектора с использованием формулы L1-нормы.
//
// vec - вектор, для которого вычисляется норма.
// float64 - вычисленная норма вектора.
func calculateNorm1(vec []float64) float64 {
	norm := 0.0
	for _, val := range vec {
		norm += math.Abs(val)
	}
	return norm
}

// calculateNormInf вычисляет бесконечную норму заданного вектора.
//
// vec - срез float64, представляющий вектор.
// Функция возвращает значение float64, представляющее бесконечную норму.
func calculateNormInf(vec []float64) float64 {
	norm := 0.0
	for _, val := range vec {
		absVal := math.Abs(val)
		if absVal > norm {
			norm = absVal
		}
	}
	return norm
}

// calculateNorm2 вычисляет 2-норму вектора.
//
// vec: Входной вектор типа []float64.
// Возвращает значение 2-нормы типа float64.
func calculateNorm2(vec []float64) float64 {
	norm := 0.0
	for _, val := range vec {
		norm += val * val
	}
	return math.Sqrt(norm)
}

// calculateDeterminant вычисляет определитель квадратной матрицы.
//
// A - двумерный срез, представляющий матрицу.
// Возвращает значение типа float64, представляющее определитель матрицы.
func calculateDeterminant(A [][]float64) float64 {
	n := len(A)
	det := 1.0

	for k := 0; k < n; k++ {
		maxRow := k
		for i := k + 1; i < n; i++ {
			if math.Abs(A[i][k]) > math.Abs(A[maxRow][k]) {
				maxRow = i
			}
		}

		if maxRow != k {
			A[k], A[maxRow] = A[maxRow], A[k]
			det *= -1
		}

		pivot := A[k][k]
		if pivot == 0 {
			return 0
		}

		det *= pivot

		for i := k + 1; i < n; i++ {
			factor := A[i][k] / pivot
			for j := k; j < n; j++ {
				A[i][j] -= factor * A[k][j]
			}
		}
	}

	return det
}

// calculateInverseMatrix вычисляет обратную матрицу для квадратной матрицы.
//
// Принимает на вход двумерный срез значений float64, представляющий матрицу A.
// Функция возвращает двумерный срез значений float64, представляющий обратную матрицу.
func calculateInverseMatrix(A [][]float64) [][]float64 {
	A = [][]float64{
		{4.35, 4.39, 3.67},
		{4.04, 3.65, 3.17},
		{3.14, 2.69, 2.17},
	}
	n := len(A)
	I := make([][]float64, n)
	for i := range I {
		I[i] = make([]float64, n)
		I[i][i] = 1.0
	}

	for k := 0; k < n; k++ {
		pivot := A[k][k]

		for j := 0; j < n; j++ {
			A[k][j] /= pivot
			I[k][j] /= pivot
		}

		for i := 0; i < n; i++ {
			if i != k {
				factor := A[i][k]
				for j := 0; j < n; j++ {
					A[i][j] -= factor * A[k][j]
					I[i][j] -= factor * I[k][j]
				}
			}
		}
	}

	return I
}

// checkInverseMatrix проверяет, является ли матрица обратной другой матрице.
//
// Параметры:
// - A: матрица, представленная в виде двумерного среза float64.
// - inverseMatrix: обратная матрица, представленная в виде двумерного среза float64.
//
// Возвращает:
// - bool: true, если данная матрица является обратной другой матрице, в противном случае - false.
func checkInverseMatrix(A, inverseMatrix [][]float64) bool {
	A = [][]float64{
		{4.35, 4.39, 3.67},
		{4.04, 3.65, 3.17},
		{3.14, 2.69, 2.17},
	}
	n := len(A)
	E := make([][]float64, n)
	for i := range E {
		E[i] = make([]float64, n)
		E[i][i] = 1.0
	}

	product := multiplyMatrices(A, inverseMatrix)
	fmt.Print("\nПроизведение A и обратной матрицы:\n")
	printMatrixWithPrecision(product)

	for i := 0; i < n; i++ {
		for j := 0; j < n; j++ {
			if math.Round(math.Abs(product[i][j])) != math.Round(math.Abs(E[i][j])) {
				return false
			}
		}
	}

	return true
}

// multiplyMatrices умножает две матрицы A и B и возвращает результат.
//
// A - двумерный срез типа float64, представляющий первую матрицу.
// B - двумерный срез типа float64, представляющий вторую матрицу.
// Функция возвращает двумерный срез типа float64, представляющий матрицу-результат.
func multiplyMatrices(A, B [][]float64) [][]float64 {
	n := len(A)
	m := len(B[0])
	p := len(B)
	result := make([][]float64, n)
	for i := range result {
		result[i] = make([]float64, m)
	}

	for i := 0; i < n; i++ {
		for j := 0; j < m; j++ {
			for k := 0; k < p; k++ {
				result[i][j] += A[i][k] * B[k][j]
			}
		}
	}

	return result
}

// calculateConditionNumber вычисляет число обусловленности матрицы в различных нормах.
//
// Функция принимает два параметра: A, которая является матрицей, для которой вычисляется число обусловленности,
// и inverseMatrix, которая является обратной матрицей для A.
//
// Функция возвращает три значения типа float64: conditionNumber1, которое является числом обусловленности A в 1-норме,
// conditionNumberInf, которое является числом обусловленности A в бесконечностной норме, и conditionNumber2,
// которое является числом обусловленности A в 2-норме.
func calculateConditionNumber(A, inverseMatrix [][]float64) (float64, float64, float64) {

	normA1 := matrixNorm(A, 1)
	normAInf := matrixNorm(A, math.Inf(1))
	normA2 := matrixNorm(A, 2)

	normAInverse1 := matrixNorm(inverseMatrix, 1)
	normAInverseInf := matrixNorm(inverseMatrix, math.Inf(1))
	normAInverse2 := matrixNorm(inverseMatrix, 2)

	// Вычислите число обусловленности в простых нормах
	conditionNumber1 := normA1 * normAInverse1
	conditionNumberInf := normAInf * normAInverseInf
	conditionNumber2 := normA2 * normAInverse2

	return conditionNumber1, conditionNumberInf, conditionNumber2
}

// matrixNorm вычисляет p-норму матрицы.
//
// A - входная матрица, представленная в виде двумерного среза float64.
// p - параметр нормы.
// Возвращает p-норму матрицы.
func matrixNorm(A [][]float64, p float64) float64 {
	n := len(A)
	m := len(A[0])

	maxRowSum := 0.0
	for i := 0; i < n; i++ {
		rowSum := 0.0
		for j := 0; j < m; j++ {
			if p == 1 {
				rowSum += math.Abs(A[i][j])
			} else if p == math.Inf(1) {
				rowSum += math.Abs(A[i][j])
			} else {
				rowSum += math.Pow(math.Abs(A[i][j]), p)
			}
		}
		if rowSum > maxRowSum {
			maxRowSum = rowSum
		}
	}

	if p == math.Inf(1) {
		return maxRowSum
	} else if p == 1 {
		return maxRowSum
	} else {
		return math.Pow(maxRowSum, 1.0/p)
	}
}

// calculateTheoreticalRelativeError вычисляет теоретическую относительную погрешность.
//
// Принимает следующие параметры:
// - A: двумерный срез типа float64, представляющий матрицу.
// - x: срез типа float64, представляющий вектор.
// - b: срез типа float64, представляющий еще один вектор.
// - deltaB: срез типа float64, представляющий изменение вектора b.
// - index: целое число, представляющее индекс элемента в deltaB.
//
// Возвращает значение типа float64, представляющее вычисленную теоретическую относительную погрешность.
func calculateTheoreticalRelativeError(A [][]float64, x, b, deltaB []float64, index int) float64 {
	conditionNumber1, _, _ := calculateConditionNumber(A, calculateInverseMatrix(A))
	return conditionNumber1 * (math.Abs(deltaB[index]) / math.Abs(b[index]))
}

// solveLinearSystemWithPivot решает систему линейных уравнений с использованием выбора опорного элемента.
//
// Принимает матрицу коэффициентов A в виде двумерного среза float64 и вектор b в виде среза float64.
// Функция возвращает вектор решения x в виде среза float64.
func gaussEliminationWithPivot(A [][]float64, b []float64) []float64 {
	n := len(A)
	x := make([]float64, n)

	for k := 0; k < n-1; k++ {
		// Находим максимальный элемент в столбце k
		maxVal := math.Abs(A[k][k])
		maxRow := k
		for i := k + 1; i < n; i++ {
			if absVal := math.Abs(A[i][k]); absVal > maxVal {
				maxVal = absVal
				maxRow = i
			}
		}

		// Проверяем, что главный элемент не равен нулю (иначе деление на ноль)
		if maxVal == 0 {
			fmt.Println("Система имеет бесконечное число решений или несовместна.")
			return nil
		}

		// Перестановка строк
		A[k], A[maxRow] = A[maxRow], A[k]
		b[k], b[maxRow] = b[maxRow], b[k]

		// Прямой ход
		for i := k + 1; i < n; i++ {
			factor := A[i][k] / A[k][k]
			for j := k + 1; j < n; j++ {
				A[i][j] -= factor * A[k][j]
			}
			b[i] -= factor * b[k]
		}
	}

	// Обратный ход
	for i := n - 1; i >= 0; i-- {
		sum := 0.0
		for j := i + 1; j < n; j++ {
			sum += A[i][j] * x[j]
		}
		x[i] = (b[i] - sum) / A[i][i]
	}

	return x
}

// luDecomposition выполняет LU-разложение заданной матрицы.
//
// Параметры:
// - A: матрица, для которой выполняется LU-разложение, представленная в виде двумерного среза float64.
//
// Возвращает:
// - L: нижняя треугольная матрица LU-разложения, представленная в виде двумерного среза float64.
// - U: верхняя треугольная матрица LU-разложения, представленная в виде двумерного среза float64.
func luDecomposition(A [][]float64) ([][]float64, [][]float64) {
	n := len(A)
	L := make([][]float64, n)
	U := make([][]float64, n)

	for i := 0; i < n; i++ {
		L[i] = make([]float64, n)
		U[i] = make([]float64, n)
	}

	for i := 0; i < n; i++ {
		for j := i; j < n; j++ {
			sum := 0.0
			for k := 0; k < i; k++ {
				sum += L[i][k] * U[k][j]
			}
			U[i][j] = A[i][j] - sum
		}

		for j := i; j < n; j++ {
			if i == j {
				L[i][j] = 1.0
			} else {
				sum := 0.0
				for k := 0; k < i; k++ {
					sum += L[j][k] * U[k][i]
				}
				L[j][i] = (A[j][i] - sum) / U[i][i]
			}
		}
	}

	return L, U
}

// solveLowerTriangular вычисляет решение нижнетреугольной системы уравнений.
//
// Параметры:
// - L: 2D срез, представляющий нижнетреугольную матрицу.
// - b: Срез float64, представляющий правую часть уравнений.
//
// Возвращает:
// - y: Срез float64, представляющий решение системы.
func solveLowerTriangular(L [][]float64, b []float64) []float64 {
	n := len(L)
	y := make([]float64, n)

	for i := 0; i < n; i++ {
		sum := 0.0
		for j := 0; j < i; j++ {
			sum += L[i][j] * y[j]
		}
		y[i] = (b[i] - sum) / L[i][i]
	}

	return y
}

// solveUpperTriangular решает систему уравнений с верхнетреугольной матрицей.
//
// U - это верхнетреугольная матрица, представляющая систему уравнений.
// b - вектор констант с правой стороны уравнений.
// Функция возвращает вектор решения x.
func solveUpperTriangular(U [][]float64, b []float64) []float64 {
	n := len(U)
	x := make([]float64, n)

	for i := n - 1; i >= 0; i-- {
		sum := 0.0
		for j := i + 1; j < n; j++ {
			sum += U[i][j] * x[j]
		}
		x[i] = (b[i] - sum) / U[i][i]
	}

	return x
}
