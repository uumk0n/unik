package main

import "math"

func solveLinearSystem(A [][]float64, b []float64) []float64 {
	n := len(A)
	x := make([]float64, n)

	for k := 0; k < n; k++ {
		maxRow := k
		for i := k + 1; i < n; i++ {
			if abs(A[i][k]) > abs(A[maxRow][k]) {
				maxRow = i
			}
		}

		// Переставляем строки
		A[k], A[maxRow] = A[maxRow], A[k]
		b[k], b[maxRow] = b[maxRow], b[k]

		// Прямой ход
		for i := k + 1; i < n; i++ {
			factor := A[i][k] / A[k][k]
			for j := k; j < n; j++ {
				A[i][j] -= factor * A[k][j]
			}
			b[i] -= factor * b[k]
		}
	}

	// Обратный ход
	for k := n - 1; k >= 0; k-- {
		x[k] = b[k]
		for j := k + 1; j < n; j++ {
			x[k] -= A[k][j] * x[j]
		}
		x[k] /= A[k][k]
	}

	return x
}

func abs(x float64) float64 {
	if x < 0 {
		return -x
	}
	return x
}

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

func calculateNorm1(vec []float64) float64 {
	norm := 0.0
	for _, val := range vec {
		norm += math.Abs(val)
	}
	return norm
}

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

func calculateNorm2(vec []float64) float64 {
	norm := 0.0
	for _, val := range vec {
		norm += val * val
	}
	return math.Sqrt(norm)
}

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
			// Переставляем строки, если необходимо
			A[k], A[maxRow] = A[maxRow], A[k]
			// И меняем знак определителя
			det *= -1
		}

		pivot := A[k][k]
		if pivot == 0 {
			// Определитель равен нулю, если на диагонали есть ноль
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

func calculateInverseMatrix(A [][]float64) [][]float64 {
	n := len(A)
	I := make([][]float64, n)
	for i := range I {
		I[i] = make([]float64, n)
		I[i][i] = 1.0 // Создаем единичную матрицу
	}

	// Приводим расширенную матрицу [A | I] к виду [I | A^-1]
	for k := 0; k < n; k++ {
		pivot := A[k][k]

		// Делим текущую строку на pivot, чтобы на диагонали была единица
		for j := 0; j < n; j++ {
			A[k][j] /= pivot
			I[k][j] /= pivot
		}

		// Вычитаем текущую строку из остальных строк, чтобы получить единичные элементы в столбце k
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

	return I // I теперь содержит обратную матрицу A^-1
}

func checkInverseMatrix(A, inverseMatrix [][]float64) bool {
	n := len(A)
	E := make([][]float64, n)
	for i := range E {
		E[i] = make([]float64, n)
		E[i][i] = 1.0 // Создаем единичную матрицу
	}

	// Умножаем матрицу A на обратную матрицу A^-1
	product := multiplyMatrices(A, inverseMatrix)

	// Проверяем, что получившаяся матрица равна единичной матрице E
	for i := 0; i < n; i++ {
		for j := 0; j < n; j++ {
			if math.Abs(product[i][j]-E[i][j]) > 1e-9 {
				// Погрешность - небольшое значение, чтобы учесть округления
				return false
			}
		}
	}

	return true
}

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

func calculateConditionNumber(A, inverseMatrix [][]float64) (float64, float64, float64) {

	// Вычислите нормы матрицы A и обратной матрицы A^-1
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

func matrixNorm(A [][]float64, p float64) float64 {
	n := len(A)
	m := len(A[0])

	// Вычислите норму матрицы в соответствии с выбранной нормой (p)
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

func calculateTheoreticalRelativeError(A [][]float64, x, b, deltaB []float64, index int) float64 {
	conditionNumber1, _, _ := calculateConditionNumber(A, calculateInverseMatrix(A))
	return conditionNumber1 * (math.Abs(deltaB[index]) / math.Abs(b[index]))
}

func solveLinearSystemWithPivot(A [][]float64, b []float64) []float64 {
	n := len(A)
	x := make([]float64, n)

	// Создаем массив для отслеживания перестановок строк
	rowPermutations := make([]int, n)
	for i := 0; i < n; i++ {
		rowPermutations[i] = i
	}

	// Прямой ход с выбором главного элемента
	for k := 0; k < n-1; k++ {
		// Находим главный элемент и переставляем строки
		maxVal := math.Abs(A[k][k])
		maxRow := k
		for i := k + 1; i < n; i++ {
			if absVal := math.Abs(A[i][k]); absVal > maxVal {
				maxVal = absVal
				maxRow = i
			}
		}
		if maxRow != k {
			// Переставляем строки в матрице A
			A[k], A[maxRow] = A[maxRow], A[k]
			// Переставляем элементы в векторе перестановок
			rowPermutations[k], rowPermutations[maxRow] = rowPermutations[maxRow], rowPermutations[k]
		}

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

	// Восстанавливаем порядок строк в решении
	orderedX := make([]float64, n)
	for i := 0; i < n; i++ {
		orderedX[rowPermutations[i]] = x[i]
	}

	return orderedX
}

func luDecomposition(A [][]float64) ([][]float64, [][]float64) {
	n := len(A)
	L := make([][]float64, n)
	U := make([][]float64, n)

	for i := 0; i < n; i++ {
		L[i] = make([]float64, n)
		U[i] = make([]float64, n)
	}

	for i := 0; i < n; i++ {
		// Верхний треугольник (U)
		for j := i; j < n; j++ {
			sum := 0.0
			for k := 0; k < i; k++ {
				sum += L[i][k] * U[k][j]
			}
			U[i][j] = A[i][j] - sum
		}

		// Нижний треугольник (L)
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
