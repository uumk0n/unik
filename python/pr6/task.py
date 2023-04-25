import random
import numpy as np
import matplotlib.pyplot as plt

# Определение функции перцептрона
def perceptron(x, y, learning_rate, num_iterations,changeble:bool=False):
    # Масштабирование входных данных
    x = (x - np.mean(x, axis=0)) / np.std(x, axis=0)
    # Добавление смещения к входным данным
    x = np.hstack([x, np.ones((x.shape[0], 1))])
    weights = np.zeros((x.shape[1], 1))
    bias = 0
    errors = []
    for i in range(num_iterations):
        # Рассчитать прогнозируемые значения
        y_pred = np.dot(x, weights) + bias
        # Рассчитываем ошибку
        error = y - y_pred
        # Обновляем весовые коэффициенты и смещение
        weights += learning_rate * np.dot(x.T, error)
        bias += learning_rate * np.sum(error)
        # Рассчитываем среднюю квадратичную ошибку
        mse = np.mean(error ** 2)
        errors.append(mse)
        # Понижение скорости обучения с течением времени
        if changeble and i % 100 == 0:
            learning_rate *= 0.99
    # Рассчитываем окончательные прогнозные значения
    y_pred = np.dot(x, weights) + bias
    # Рассчитываем среднеквадратичную ошибку
    rms_error = np.sqrt(np.mean((y - y_pred) ** 2))
    return weights, bias, errors, rms_error


mu = 100
sigma = 5
base = np.random.normal(mu, sigma)
leg = base / 3.8 + np.random.normal(0, 1)
heigh = base * 1.7 + np.random.normal(0, 2)
input_data = np.array([[leg, 1], [leg, heigh]])
# Создаём данные и добавляем их во входной массив
for i in range(15):
    base = np.random.normal(mu, sigma)
    leg = base / 3.8 + np.random.normal(0, 1)
    heigh = base * 1.7 + np.random.normal(0, 2)
    
    new_data = np.array([[leg, 1], [leg, heigh]])
    input_data = np.vstack((input_data, new_data))


# Используя pyplot.scatter, визуализируем и проверяем адекватность исходных данных
plt.scatter(input_data[:, 0], input_data[:, 1], marker='o', color='blue')
plt.xlabel('Нога')
plt.ylabel('Рост')
plt.title('Входные данные')
plt.show()


# Используя перцептрон, изучаем взаимосвязь между ростом и длиной ноги
x = input_data[:, :-1]
y = input_data[:, -1].reshape(-1, 1)

# Обучение перцептрона с изменением скорости обучения
weights, bias, errors, rms_error = perceptron(x, y, 0.0001, 10000, True)

x = np.hstack([x, np.ones((x.shape[0], 1))])

# Рисуем диаграмму рассеяния и прямую линию
plt.scatter(x[:, 0], y, marker='o', color='blue')
plt.plot(x[:, 0], np.dot(x, weights) + bias, color='red')
plt.xlabel('Нога')
plt.ylabel('Рост')
plt.title('Результаты работы перцептрона')
plt.show()

# Вычисляем среднеквадратичную ошибку для перцептрона
y_pred = np.dot(x, weights) + bias
rms_error = np.sqrt(np.mean((y - y_pred) ** 2))
print('RMS error:', rms_error)


# Строим график изменения ошибки со временем
plt.plot(range(len(errors)), errors)
plt.xlabel('Итерации')
plt.ylabel('Средняя квадратичная ошибка')
plt.title('ошибки с изменением времени')
plt.show()