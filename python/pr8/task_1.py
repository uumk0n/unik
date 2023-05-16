import numpy as np
from sklearn.linear_model import LinearRegression
from sklearn.metrics import mean_squared_error
import random
from sklearn.datasets import load_diabetes

# Генерирование случайных размеров ног и роста
data = []
for i in range(15):
    base = random.gauss(100, 5)
    leg = base / 3.8 + random.gauss(0, 1)
    height = base * 1.7 + random.gauss(0, 2)
    data.append([leg, height])

data = np.array(data)

def proceed(x,k,c):
    return x * k+ c


def perceptron(input_data, rate, num_step, mod:bool=False):
    targets = data[:,1]
    k = random.uniform(-5, 5)
    c = random.uniform(-5, 5)
    error_history=[]
    for i in range(num_step):
        x = random.choice(input_data)
        true_result = targets[np.where((input_data == x).all(axis=1))[0][0]]
        out = proceed(x,k,c)
        delta = true_result - out
        k += delta * rate * x[0]
        c += delta * rate * x[1]
        if i % 100 == 0:
            error = np.mean((targets - proceed(input_data[:,0],k[0],c[0])) ** 2)
            error_history.append(error)
            if mod:
                rate*=0.99
   
    return k, c, error_history

# Разделите входные характеристики (размер ноги) и целевую переменную (рост)
X = data[:, 0].reshape(-1, 1)
y = data[:, 1]

# Алгоритм на основе перцептрона
k_perceptron, c_perceptron, _ = perceptron(data, 0.001, 100000)
y_pred_perceptron = k_perceptron[0] * X.flatten() + c_perceptron[0]
mse_perceptron = mean_squared_error(y, y_pred_perceptron)

# LinearRegression из scikit-learn
regressor = LinearRegression()
regressor.fit(X, y)
y_pred_regression = regressor.predict(X)
mse_regression = mean_squared_error(y, y_pred_regression)

# Выводим MSE для каждого подхода
print("MSE - Перцептрон: ", mse_perceptron)
print("MSE - Линейная регрессия: ", mse_regression)

print("\n==================\n")

# Загрузите набор данных о диабете
diabetes = load_diabetes()
X = diabetes.data[:15]
y = diabetes.target[:15]

# Выбор одного входного параметра
X_one_param = X[:, 2].reshape(-1, 1)

# Алгоритм на основе перцептрона
k_perceptron, c_perceptron, _ = perceptron(np.hstack((X_one_param, y.reshape(-1, 1))), 0.001, 100000)
y_pred_perceptron = k_perceptron[0] * X_one_param.flatten() + c_perceptron[0]
mse_perceptron = mean_squared_error(y, y_pred_perceptron)

# LinearRegression из scikit-learn с одним входным параметром
regressor_one_param = LinearRegression()
regressor_one_param.fit(X_one_param, y)
y_pred_regression_one_param = regressor_one_param.predict(X_one_param)
mse_regression_one_param = mean_squared_error(y, y_pred_regression_one_param)

# Линейная регрессия из scikit-learn с десятью входными параметрами
regressor_ten_param = LinearRegression()
regressor_ten_param.fit(X, y)
y_pred_regression_ten_param = regressor_ten_param.predict(X)
mse_regression_ten_param = mean_squared_error(y, y_pred_regression_ten_param)

# Выводим MSE для каждого подхода
print("MSE - Perceptron (one input parameter): ", mse_perceptron)
print("MSE - Linear Regression (one input parameter): ", mse_regression_one_param)
print("MSE - Linear Regression (ten input parameters): ", mse_regression_ten_param)

