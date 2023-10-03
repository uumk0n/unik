package main

import (
	"math"

	"github.com/gonum/matrix/mat64"
)

// simpleIterationMethod - функция, которая реализует метод простой итерации для решения системы линейных уравнений.
//
// Параметры:
// - A: Указатель на матрицу mat64.Dense, представляющую матрицу коэффициентов системы.
// - b: Указатель на матрицу mat64.Dense, представляющую вектор констант системы.
// - epsilon: Значение типа float64, представляющее допустимую погрешность для сходимости.
//
// Возвращает:
// - x: Указатель на матрицу mat64.Dense, представляющую вектор решения системы.
// - iterations: Целое число, представляющее количество выполненных итераций.
func simpleIterationMethod(A *mat64.Dense, b *mat64.Dense, epsilon float64) (*mat64.Dense, int) {
	n := A.RawMatrix().Rows
	x := mat64.NewDense(n, 1, nil)
	xPrev := mat64.NewDense(n, 1, nil)
	iterations := 0

	for {
		iterations++
		xPrev.Copy(x)

		for i := 0; i < n; i++ {
			sum := 0.0
			for j := 0; j < n; j++ {
				if j != i {
					sum += A.At(i, j) * xPrev.At(j, 0)
				}
			}
			x.Set(i, 0, (b.At(i, 0)-sum)/A.At(i, i))
		}

		diff := mat64.NewDense(n, 1, nil)
		diff.Sub(x, xPrev)

		if mat64.Norm(diff, 2) < epsilon {
			break
		}
	}

	return x, iterations
}

// gaussSeidelMethod реализует метод Гаусса-Зейделя для решения системы линейных уравнений.
//
// Параметры:
// - A: Указатель на mat64.Dense, представляющий матрицу коэффициентов системы.
// - b: Указатель на mat64.Dense, представляющий вектор констант системы.
// - epsilon: Значение типа float64, представляющее желаемую точность решения.
//
// Возвращает:
// - x: Указатель на mat64.Dense, представляющий вектор решения системы.
// - iterations: Целое число, представляющее количество итераций, необходимых для сходимости к решению.
func gaussSeidelMethod(A *mat64.Dense, b *mat64.Dense, epsilon float64) (*mat64.Dense, int) {
	n := A.RawMatrix().Rows
	x := mat64.NewDense(n, 1, nil)
	xPrev := mat64.NewDense(n, 1, nil)
	iterations := 0

	for {
		iterations++
		xPrev.Copy(x)

		for i := 0; i < n; i++ {
			sum1 := 0.0
			sum2 := 0.0
			for j := 0; j < i; j++ {
				sum1 += A.At(i, j) * x.At(j, 0)
			}
			for j := i + 1; j < n; j++ {
				sum2 += A.At(i, j) * xPrev.At(j, 0)
			}
			x.Set(i, 0, (b.At(i, 0)-sum1-sum2)/A.At(i, i))
		}

		diff := mat64.NewDense(n, 1, nil)
		diff.Sub(x, xPrev)

		if mat64.Norm(diff, 2) < epsilon {
			break
		}
	}

	return x, iterations
}

func computeErrors(xExact, xApprox *mat64.Dense) ([]float64, []float64) {
	n := xExact.RawMatrix().Rows
	absErr := make([]float64, 3)
	relErr := make([]float64, 3)

	// Вычисление абсолютной погрешности в нормах L1, L∞ и L2
	for norm := 0; norm < 3; norm++ {
		for i := 0; i < n; i++ {
			diff := math.Abs(xExact.At(i, 0) - xApprox.At(i, 0))
			absErr[norm] += diff
			if norm == 1 && diff > absErr[1] {
				absErr[1] = diff
			} else if norm == 2 {
				absErr[2] += diff * diff
			}
		}
		if norm == 2 {
			absErr[2] = math.Sqrt(absErr[2])
		}
	}

	// Вычисление относительной погрешности в нормах L1, L∞ и L2
	for norm := 0; norm < 3; norm++ {
		for i := 0; i < n; i++ {
			relDiff := math.Abs(xExact.At(i, 0)-xApprox.At(i, 0)) / math.Abs(xExact.At(i, 0))
			relErr[norm] += relDiff
			if norm == 1 && relDiff > relErr[1] {
				relErr[1] = relDiff
			} else if norm == 2 {
				relErr[2] += relDiff * relDiff
			}
		}
		if norm == 2 {
			relErr[2] = math.Sqrt(relErr[2])
		}
	}

	return absErr, relErr
}

func gaussElimination(A *mat64.Dense, b *mat64.Dense) *mat64.Dense {
	n := A.RawMatrix().Rows
	x := mat64.NewDense(n, 1, nil)

	// Создаем augmented matrix
	augmented := mat64.NewDense(n, n+1, nil)
	for i := 0; i < n; i++ {
		for j := 0; j < n; j++ {
			augmented.Set(i, j, A.At(i, j))
		}
		augmented.Set(i, n, b.At(i, 0))
	}

	// Прямой ход
	for k := 0; k < n; k++ {
		// Поиск максимального элемента для частичного выбора
		maxRow := k
		maxVal := augmented.At(k, k)
		for i := k + 1; i < n; i++ {
			val := augmented.At(i, k)
			if math.Abs(val) > math.Abs(maxVal) {
				maxRow = i
				maxVal = val
			}
		}
		// Перестановка строк
		augmented.SetRow(k, augmented.RawRowView(maxRow))
		// Обнуление нижних элементов столбца
		for i := k + 1; i < n; i++ {
			factor := augmented.At(i, k) / augmented.At(k, k)
			augmented.Set(i, k, 0)
			for j := k + 1; j <= n; j++ {
				augmented.Set(i, j, augmented.At(i, j)-factor*augmented.At(k, j))
			}
		}
	}

	// Обратный ход
	for i := n - 1; i >= 0; i-- {
		sum := augmented.At(i, n)
		for j := i + 1; j < n; j++ {
			sum -= augmented.At(i, j) * x.At(j, 0)
		}
		x.Set(i, 0, sum/augmented.At(i, i))
	}

	return x
}
