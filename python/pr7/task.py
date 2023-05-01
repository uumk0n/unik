import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.datasets import load_iris

#Выбор параметров сепала и длины лепестка в качестве
#функций, а также выбор только образцов с метками классов 0 или 1 (setosa и versicolor).

iris = load_iris()


X = iris.data[:, [0, 2]]
y = iris.target


X = X[:100]
y = y[:100]
y[y == 0] = -1

# Определение класса Perceptron с сигмоидной активацией и методами fit и predict.
class Perceptron:
    def __init__(self, eta=0.01, n_iter=50, random_state=None, alpha=0.1):
        self.eta = eta
        self.n_iter = n_iter
        self.random_state = random_state
        self.alpha = alpha
        
    def sigmoid(self, z):
        return 1 / (1 + np.exp(-z))
    
    def net_input(self, X):
        return np.dot(X, self.w_[1:].reshape(-1, 1)) + self.w_[0]

    def predict(self, X):
        return np.where(self.sigmoid(self.net_input(X)) >= 0.5, 1, -1)

    
    def fit(self, X, y):
        if self.random_state:
            np.random.seed(self.random_state)
            
        self.w_ = np.zeros(1 + X.shape[1])
        self.errors_ = []
        
        for i in range(self.n_iter):
            errors = 0
            
            for xi, yi in zip(X, y):
                update = self.eta * (yi - self.predict(xi))
                self.w_[1:] += update * xi - self.alpha * self.w_[1:]
                self.w_[0] += update
                errors += int(update != 0.0)
            
            self.errors_.append(errors)
        
        return self


# Разделение данных на тренировочный и тестовый наборы (соотношение 40/10). Обучение персептрона с двумя функциями.
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.1, random_state=1)

ppn2 = Perceptron(eta=0.1, n_iter=100)
ppn2.fit(X_train, y_train)

# Построение графика ошибок для персептрона с двумя функциями.
plt.plot(range(1, len(ppn2.errors_) + 1), ppn2.errors_, marker='o')
plt.xlabel('Эпохи')
plt.ylabel('Количество обновлений')
plt.show()
from matplotlib.colors import ListedColormap

# Определение colormap с двумя цветами и маркерами. Построение графика 
# рассеяния с линией разделения для персептрона с двумя функциями.
cmap = ListedColormap(['#FF0000', '#0000FF'])
markers = ('s', 'x', 'o', '^', 'v')

plt.scatter(X_train[y_train == 1, 0], X_train[y_train == 1, 1], color='blue', marker='x', label='setosa')
plt.scatter(X_train[y_train == -1, 0], X_train[y_train == -1, 1], color='red', marker='o', label='versicolor')
x_min, x_max = X_train[:, 0].min() - 1, X_train[:, 0].max() + 1
y_min, y_max = X_train[:, 1].min() - 1, X_train[:, 1].max() + 1
xx1, xx2 = np.meshgrid(np.arange(x_min, x_max, 0.1), np.arange(y_min, y_max, 0.1))
Z = ppn2.predict(np.array([xx1.ravel(), xx2.ravel()]).T)
Z = Z.reshape(xx1.shape)

plt.contourf(xx1, xx2, Z.reshape(xx1.shape), alpha=0.3, cmap=cmap)
plt.xlim(xx1.min(), xx1.max())
plt.ylim(xx2.min(), xx2.max())

for idx, cl in enumerate(np.unique(y)):
    plt.scatter(x=X[y == cl, 0], y=X[y == cl, 1], alpha=0.8, c=cmap(idx),
                marker=markers[idx], label=cl)

plt.xlabel('sepal длина [см]')
plt.ylabel('petal длина [см]')
plt.legend(loc='upper left')
plt.title('Перцептрон - 2 параметра')
plt.show()

perceptron = Perceptron(eta=0.1, n_iter=100,random_state=1,alpha=0.1)
perceptron.fit(X, y)

plt.plot(range(1, len(perceptron.errors_) + 1), perceptron.errors_, marker='o')
plt.xlabel('Эпохи')
plt.ylabel('Количество обновлений')
plt.show()