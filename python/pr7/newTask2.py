import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.svm import LinearSVC
from sklearn.model_selection import train_test_split
from sklearn.datasets import load_iris
from matplotlib.colors import ListedColormap


iris = load_iris()
# выбор длины sepal и длины petal
X = iris.data[:, [0, 2]]
y = iris.target

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


X = X[50:]
y = y[50:]
y[y == 0] = -1

# Разделение данных на обучающий и тестовый наборы
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.4, random_state=1)

# Создание экземпляра классификатора Perceptron
ppn2 = Perceptron(eta=0.1, n_iter=100)
ppn2.fit(X_train, y_train)

# Создание экземпляра классификатора LinearSVC
svm2 = LinearSVC(random_state=1, max_iter=10000)
svm2.fit(X_train, y_train)

# Построение границ принятия решения и точек данных для линейной модели SVC с 2 параметрами
cmap = ListedColormap(['#FF0000', '#0000FF'])
markers = ('s', 'x', 'o', '^', 'v')

x_min, x_max = X[:, 0].min() - 1, X[:, 0].max() + 1
y_min, y_max = X[:, 1].min() - 1, X[:, 1].max() + 1
xx1, xx2 = np.meshgrid(np.arange(x_min, x_max, 0.1), np.arange(y_min, y_max, 0.1))
Z_svm2 = svm2.predict(np.c_[xx1.ravel(), xx2.ravel()])
Z_svm2 = Z_svm2.reshape(xx1.shape)

plt.subplot(121)
plt.contourf(xx1, xx2, Z_svm2, alpha=0.3, cmap=cmap)
plt.scatter(X_train[y_train == 1, 0], X_train[y_train == 1, 1], color='blue', marker='x', label='versicolor')
plt.scatter(X_train[y_train == -1, 0], X_train[y_train == -1, 1], color='red', marker='o', label='virginica')
plt.xlabel('sepal length [cm]')
plt.ylabel('petal length [cm]')
plt.legend(loc='upper left')
plt.title('LinearSVC - 2 parameters')

# Построение границ принятия решения и точек данных для перцептрона с 2 параметрами
Z_ppn2 = ppn2.predict(np.array([xx1.ravel(), xx2.ravel()]).T)
Z_ppn2 = Z_ppn2.reshape(xx1.shape)

plt.subplot(122)
plt.contourf(xx1, xx2, Z_ppn2, alpha=0.3, cmap=cmap)
plt.scatter(X_train[y_train == 1, 0], X_train[y_train == 1, 1], color='blue', marker='x', label='versicolor')
plt.scatter(X_train[y_train == -1, 0], X_train[y_train == -1, 1], color='red', marker='o', label='virginica')
plt.xlabel('sepal length [cm]')
plt.ylabel('petal length [cm]')
plt.legend(loc='upper left')
plt.title('Perceptron - 2 parameters')

plt.tight_layout()
plt.show()


plt.plot(range(1, len(ppn2.errors_) + 1), ppn2.errors_, marker='o')
plt.xlabel('Epochs')
plt.ylabel('Number of misclassifications')
plt.title('Perceptron Training Error')
plt.show()

#4 param
iris = load_iris()
X = iris.data[:, [0, 1, 2, 3]]
y = iris.target

X = X[50:]
y = y[50:]
y[y == 0] = -1

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.4, random_state=1)

ppn4 = Perceptron(eta=0.1, n_iter=100)
ppn4.fit(X_train, y_train)

svm4 = LinearSVC(random_state=1, max_iter=1000)
svm4.fit(X_train, y_train)

cmap = ListedColormap(['#FF0000', '#0000FF'])
markers = ('s', 'x', 'o', '^', 'v')

x_min, x_max = X[:, 0].min() - 1, X[:, 0].max() + 1
y_min, y_max = X[:, 2].min() - 1, X[:, 2].max() + 1
xx1, xx2 = np.meshgrid(np.arange(x_min, x_max, 0.1), np.arange(y_min, y_max, 0.1))
Z_svm4 = svm4.predict(np.c_[xx1.ravel(), xx2.ravel(), xx1.ravel(), xx2.ravel()])
Z_svm4 = Z_svm4.reshape(xx1.shape)

plt.subplot(121)
plt.contourf(xx1, xx2, Z_svm4, alpha=0.3, cmap=cmap)
plt.scatter(X_train[y_train == 1, 0], X_train[y_train == 1, 2], color='blue', marker='x', label='versicolor')
plt.scatter(X_train[y_train == -1, 0], X_train[y_train == -1, 2], color='red', marker='o', label='virginica')
plt.xlabel('sepal length [cm]')
plt.ylabel('petal length [cm]')
plt.legend(loc='upper left')
plt.title('LinearSVC - 4 parameters')

Z_ppn4 = ppn4.predict(np.array([xx1.ravel(), xx2.ravel(), xx1.ravel(), xx2.ravel()]).T)
Z_ppn4 = Z_ppn4.reshape(xx1.shape)

plt.subplot(122)
plt.contourf(xx1, xx2, Z_ppn4, alpha=0.3, cmap=cmap)
plt.scatter(X_train[y_train == 1, 0], X_train[y_train == 1, 2], color='blue', marker='x', label='versicolor')
plt.scatter(X_train[y_train == -1, 0], X_train[y_train == -1, 2], color='red', marker='o', label='virginica')
plt.xlabel('sepal length [cm]')
plt.ylabel('petal length [cm]')
plt.legend(loc='upper left')
plt.title('Perceptron - 4 parameters')

plt.tight_layout()
plt.show()

plt.plot(range(1, len(ppn4.errors_) + 1), ppn4.errors_, marker='o')
plt.xlabel('Epochs')
plt.ylabel('Number of misclassifications')
plt.title('Perceptron Training Error (4 parameters)')
plt.show()



plt.scatter(X_train[y_train == 1, 0], X_train[y_train == 1, 1], color='blue', marker='x', label='versicolor')
plt.scatter(X_train[y_train == -1, 0], X_train[y_train == -1, 1], color='red', marker='o', label='virginica')

# Построение границы принятия решения (прямая линия)
x_boundary = np.array([X_train[:, 0].min() - 1, X_train[:, 0].max() + 1])
y_boundary = -(ppn2.w_[0] + ppn2.w_[1] * x_boundary) / ppn2.w_[2]
plt.plot(x_boundary, y_boundary, color='black', linewidth=2, label='Граница принятия решения')

plt.xlabel('sepal length [cm]')
plt.ylabel('petal length [cm]')
plt.legend()
plt.title('Граница принятия решения перцептрона')
plt.show()
