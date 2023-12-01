package main

import (
	"fmt"
	"math"
)

func tridiagSolve(a, b, c, d []float64) []float64 {
	n := len(d)
	alpha := make([]float64, n)
	beta := make([]float64, n)

	alpha[0] = c[0] / b[0]
	beta[0] = d[0] / b[0]

	// Прямой ход
	for i := 1; i < n; i++ {
		alpha[i] = c[i-1] / (b[i] - a[i-1]*alpha[i-1])
		beta[i] = (d[i] - a[i-1]*beta[i-1]) / (b[i] - a[i-1]*alpha[i-1])
	}

	x := make([]float64, n)
	x[n-1] = beta[n-1]

	// Обратный ход
	for i := n - 2; i >= 0; i-- {
		x[i] = beta[i] - alpha[i]*x[i+1]
	}

	return x
}

func cubicSpline(x, y []float64) func(float64) float64 {
	n := len(x)
	h := make([]float64, n-1)

	for i := 0; i < n-1; i++ {
		h[i] = x[i+1] - x[i]
	}

	a := make([]float64, n-1)
	b := make([]float64, n)
	c := make([]float64, n-1)
	d := make([]float64, n)

	// Задаем коэффициенты для трехдиагональной матрицы
	for i := 0; i < n-1; i++ {
		a[i] = h[i] / (h[i] + h[i+1])
		b[i+1] = 2.0
		c[i] = h[i+1] / (h[i] + h[i+1])
		d[i+1] = 6.0 * ((y[i+2]-y[i+1])/h[i+1] - (y[i+1]-y[i])/h[i]) / (h[i+1] + h[i])
	}

	// Используем прогонку для решения трехдиагональной матрицы
	m := tridiagSolve(a, b, c, d)

	// Вычисляем коэффициенты сплайна
	coeff := make([][]float64, n-1)
	for i := 0; i < n-1; i++ {
		coeff[i] = []float64{
			y[i],
			(y[i+1] - y[i] - (m[i+1]+2.0*m[i])*h[i]*h[i]/6.0),
			m[i] * h[i] * h[i] / 2.0,
			(m[i+1] - m[i]) * h[i] * h[i] / 6.0,
		}
	}

	spline := func(x_ float64) float64 {
		// Находим индекс интервала, в котором находится x_
		i := 0
		for i < n-1 && x_ > x[i+1] {
			i++
		}
		i = int(math.Max(0, math.Min(float64(n-2), float64(i)))) // Ограничиваем i, чтобы не выйти за границы массива

		// Вычисляем значение сплайна для x_
		z := (x_ - x[i]) / h[i]

		return coeff[i][0] + coeff[i][1]*math.Pow(z, 1) + coeff[i][2]*math.Pow(z, 2) + coeff[i][3]*math.Pow(z, 3)
	}

	return spline
}

func main() {
	// Исходные данные
	f := func(x float64) float64 {
		return math.Exp(-x) - math.Pow(x, 3)
	}

	a, b := -5.0, 5.0
	n := 10

	x := make([]float64, n)
	for i := 0; i < n; i++ {
		x[i] = a + float64(i)*(b-a)/float64(n-1)
	}

	// Вычисляем значения функции в узлах интерполяции
	y := make([]float64, n)
	for i := 0; i < n; i++ {
		y[i] = f(x[i])
	}

	// Вычисляем сплайн
	spline := cubicSpline(x, y)

	// Выводим значения сплайна на сетке
	fmt.Println("x\t\tf(x)\t\tSpline(x)\t|f(x) - Spline(x)|")
	for i := 0; i < n*2; i++ {
		x_t := a + float64(i)*(b-a)/float64(n*2-1)
		y_t := f(x_t)
		y_s := spline(x_t)
		delta := math.Abs(y_t - y_s)
		fmt.Printf("%.4f\t%.4f\t%.4f\t%.4f\n", x_t, y_t, y_s, delta)
	}
}
