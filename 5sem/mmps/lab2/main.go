package main

import (
	"fmt"

	"github.com/gonum/matrix/mat64"
)

func main() {
	// Ваша матрица A и вектор b
	A := mat64.NewDense(4, 4, []float64{
		16.0000, 0.0688, 0.0829, 0.0970,
		0.0496, 15.1000, 0.0777, 0.0918,
		0.0444, 0.0585, 14.2000, 0.0867,
		0.0393, 0.0534, 0.0674, 13.3000,
	})

	b := mat64.NewDense(4, 1, []float64{24.4781, 26.0849, 27.3281, 28.2078})

	epsilons := []float64{1e-8, 1e-12, 1e-15}

	for _, epsilon := range epsilons {
		fmt.Printf("\n Epsilon: %e\n", epsilon)
		x, iterations := simpleIterationMethod(A, b, epsilon)
		fmt.Printf("Решение методом простых итераций (Якоби):\n")
		fmt.Printf("x = \n%v\n", mat64.Formatted(x, mat64.Prefix(""), mat64.Squeeze()))
		fmt.Printf("Количество итераций: %d\n\n", iterations)

		x, iterations = gaussSeidelMethod(A, b, epsilon)
		fmt.Printf("Решение методом Зейделя:\n")
		fmt.Printf("x = \n%v\n", mat64.Formatted(x, mat64.Prefix(""), mat64.Squeeze()))
		fmt.Printf("Количество итераций: %d\n\n", iterations)

		xGauss := gaussElimination(A, b)

		fmt.Printf("Решение методом Гаусса:\n")
		fmt.Printf("xGauss = \n%v\n", mat64.Formatted(xGauss, mat64.Prefix(""), mat64.Squeeze()))

		fmt.Printf("Решение методом простых итераций:\n")
		fmt.Printf("x = \n%v\n", mat64.Formatted(x, mat64.Prefix(""), mat64.Squeeze()))

		// Вычисление абсолютной и относительной погрешности
		absErr, relErr := computeErrors(xGauss, x)
		fmt.Printf("Абсолютная погрешность:\n")
		fmt.Printf("L1-норма: %e\n", absErr[0])
		fmt.Printf("L∞-норма: %e\n", absErr[1])
		fmt.Printf("L2-норма: %e\n", absErr[2])

		fmt.Printf("Относительная погрешность:\n")
		fmt.Printf("L1-норма: %e\n", relErr[0])
		fmt.Printf("L∞-норма: %e\n", relErr[1])
		fmt.Printf("L2-норма: %e\n", relErr[2])
	}
}
