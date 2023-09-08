package main

func runCode() {
	A := [][]float64{
		{4.35, 4.39, 3.67},
		{4.04, 3.65, 3.17},
		{3.14, 2.69, 2.17},
	}
	b := []float64{40.15, 36.82, 28.10}

	// a) Решение с помощью метода Гаусса (схема единственного деления)
	x := solveLinearSystem(A, b)

	// Вывести результаты
	printResults("a) Метод Гаусса (схема единственного деления)", A, b, x)

	// b) Решение с помощью метода Гаусса с выбором главного элемента
	xGaussWithPivot := solveLinearSystemWithPivot(A, b)

	// Вывести результаты
	printResults("b) Метод Гаусса с выбором главного элемента", A, b, xGaussWithPivot)

	// c) Решение с помощью LU-разложения матрицы A
	L, U := luDecomposition(A)
	y := solveLowerTriangular(L, b)
	xLU := solveUpperTriangular(U, y)

	// Вывести результаты
	printResults("c) LU-разложение матрицы A", A, b, xLU)
}
