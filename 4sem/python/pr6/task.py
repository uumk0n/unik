import random
import numpy as np
import matplotlib.pyplot as plt

# Generate random leg and height dimensions
data = []
for i in range(15):
    base = random.gauss(100, 5)
    leg = base / 3.8 + random.gauss(0, 1)
    height = base * 1.7 + random.gauss(0, 2)
    data.append([leg, height])


data = np.array(data)
# Visualize the input data using scatter plot
plt.scatter(data[:,0], data[:,1])
plt.xlabel('Размер ноги')
plt.ylabel('Рост')
plt.title('Входные данные')
plt.show()

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

# Visualize the output data with the learned linear model
k, c, errors = perceptron(data,0.001,10000)

x_vals = data[:,0]
y_vals = k[0] * x_vals + c[0]
plt.scatter(data[:,0], data[:,1])
plt.plot(x_vals, y_vals, c='r')
plt.xlabel('Размер ноги')
plt.ylabel('Рост')
plt.title('Результат работы перцептрона')
plt.show()

plt.plot(range(len(errors)), errors)
plt.xlabel('Шаг обучения (100 шагов на точку)')
plt.ylabel('RMS ошибка')
plt.title('Изменение ошибки при обучении перцептрона')
plt.show()

# с изменением времени
k, c, errors = perceptron(data,0.001,10000,True)

x_vals = data[:,0]
y_vals = k[0] * x_vals + c[0]
plt.scatter(data[:,0], data[:,1])
plt.plot(x_vals, y_vals, c='r')
plt.xlabel('Размер ноги')
plt.ylabel('Рост')
plt.title('Результат работы перцептрона с изменением времени')
plt.show()

plt.plot(range(len(errors)), errors)
plt.xlabel('Шаг обучения (100 шагов на точку)')
plt.ylabel('RMS ошибка')
plt.title('Изменение ошибки при обучении перцептрона с изменением времени')
plt.show()