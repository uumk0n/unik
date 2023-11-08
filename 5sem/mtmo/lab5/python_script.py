import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler

import json

# Чтение параметров из JSON-файла
with open('parameters.json', 'r') as file:
    parameters = json.load(file)


# Загрузите данные из файла "iris.csv"
iris_data = pd.read_csv("iris.csv")

# Загрузите дополнительные данные из файла "Dop.csv"
dop_data = pd.read_csv("Dop.csv")

# Разделите данные на признаки (X) и метки классов (y)
X = iris_data.iloc[:, :-1].values
y = iris_data.iloc[:, -1].values


# Извлечение параметров
k_user = parameters['k']
metric = parameters['metric'].lower()
normalize_data = parameters['useWeightedVoting']
if(normalize_data):
    scaler = StandardScaler()
    X = scaler.fit_transform(X)

weights = parameters['weights']

# Разделите данные на обучающий и тестовый набор
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

def calculate_distance(point1, point2, metric, weights):
    if metric == "евклидова":
        return np.sqrt(np.sum(weights * (point1 - point2) ** 2))
    elif metric == "хемминга":
        return np.sum(weights * (point1 != point2))
    elif metric == "чебышева":
        return np.max(weights * np.abs(point1 - point2))
    elif metric == "косинусная":
        return np.dot(weights * point1, weights * point2) / (np.linalg.norm(weights * point1) * np.linalg.norm(weights * point2))

def classify_point(x, k, metric, weights):
    distances = []
    for i in range(len(X_train)):
        distance = calculate_distance(x, X_train[i], metric, weights)
        distances.append((distance, y_train[i]))
    distances.sort(key=lambda x: x[0])
    k_nearest_neighbors = distances[:k]
    class_counts = {}
    for neighbor in k_nearest_neighbors:
        if neighbor[0] != 0:  # Проверка на ноль перед делением
            class_counts[neighbor[1]] = class_counts.get(neighbor[1], 0) + 1 / neighbor[0]  # Взвешенное голосование

    if not class_counts:  # Если class_counts пуст, вернуть случайный класс
        return np.random.choice(y_train)

    predicted_class = max(class_counts, key=class_counts.get)
    return predicted_class

def classify_test_set(X_test, k, metric, weights):
    predictions = []
    for point in X_test:
        predicted_class = classify_point(point, k, metric, weights)
        predictions.append(predicted_class)
    return predictions


# Классифицируйте тестовый набор данных с пользовательским значением k
predictions_user = classify_test_set(X_test, k_user, metric, weights)
accuracy_user = np.mean(predictions_user == y_test)
error_rate_user = 1 - accuracy_user
print(f"Точность классификации на тестовом наборе данных с пользовательским k: {accuracy_user * 100:.2f}%")
print(f"Процент ошибок на тестовом наборе данных с пользовательским k: {error_rate_user * 100:.2f}%")

# Перебираем разные значения k и находим оптимальное значение
best_k = 0
best_accuracy = 0
for k in range(1, len(X_train)):
    predictions = classify_test_set(X_test, k, metric, weights)
    accuracy = np.mean(predictions == y_test)
    if accuracy > best_accuracy:
        best_accuracy = accuracy
        best_k = k

print(f"Оптимальное значение k: {best_k} с точностью {best_accuracy * 100:.2f}%")

# Определите сорт всех объектов из обучающей выборки
train_predictions = classify_test_set(X_train, best_k, metric, weights)
train_accuracy = np.mean(train_predictions == y_train)
train_error_rate = 1 - train_accuracy
print(f"Точность классификации на обучающей выборке: {train_accuracy * 100:.2f}%")
print(f"Процент ошибок на обучающей выборке: {train_error_rate * 100:.2f}%")

# Определите сорт всех объектов из дополнительной выборки
dop_X = dop_data.values
if normalize_data:
    dop_X = scaler.transform(dop_X)
dop_predictions = classify_test_set(dop_X, best_k, metric, weights)

print("Результаты классификации дополнительной выборки:")

for i in range(len(dop_predictions)):

    print(f"Объект {i + 1}: Предсказанный сорт - {dop_predictions[i]}")
