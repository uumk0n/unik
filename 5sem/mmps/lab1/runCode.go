package main

func runCode() {
	A := [][]float64{
		{4.35, 4.39, 3.67},
		{4.04, 3.65, 3.17},
		{3.14, 2.69, 2.17},
	}
	b := []float64{
		40.15,
		36.82,
		28.10}

	x := solveLinearSystem(A, b)

	printResults("a) Метод Гаусса (схема единственного деления)", A, b, x)

	xGaussWithPivot := solveLinearSystemWithPivot(A, b)

	printResults("b) Метод Гаусса с выбором главного элемента", A, b, xGaussWithPivot)

	L, U := luDecomposition(A)
	y := solveLowerTriangular(L, b)
	xLU := solveUpperTriangular(U, y)

	printResults("c) LU-разложение матрицы A", A, b, xLU)
}
