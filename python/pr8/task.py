import numpy as np
from sklearn.datasets import make_classification
from sklearn.model_selection import train_test_split

def activation(z):
    return np.where(z > 0, 1, -1)

def perceptron(X, y, learning_rate, epochs):
    # Инициализация весов и смещения
    n_features = X.shape[1]
    weights = np.zeros(n_features)
    bias = 0
    errors = []
    
    # Обучение
    for epoch in range(epochs):
        error = 0
        for xi, yi in zip(X, y):
            # Вычисление прогноза и ошибки
            z = np.dot(xi, weights) + bias
            y_pred = activation(z)
            delta = learning_rate * (yi - y_pred)
            weights += delta * xi
            bias += delta
            error += int(delta != 0.0)
        errors.append(error)
        
    # Вычисление среднеквадратичной ошибки
    rms_error = np.sqrt(np.mean(errors))
    
    return weights, bias, errors, rms_error

# Создание синтетических данных
X, y = make_classification(n_samples=100, n_features=80, n_informative=40, random_state=42)

# Разделение на обучающую и тестовую выборки
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Обучение персептрона
weights, bias, errors, rms_error = perceptron(X_train, y_train, 0.01, 1000)

# Прогнозирование
y_pred = np.dot(X_test, weights) + bias
y_pred = activation(y_pred)

# Вычисление точности
accuracy = np.mean(y_pred == y_test)

print(f"Accuracy: {accuracy}")
