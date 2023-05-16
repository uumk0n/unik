import numpy as np
import pandas as pd
from sklearn import datasets, linear_model
from sklearn.metrics import mean_squared_error
from sklearn.model_selection import train_test_split

# Load diabetes dataset
diabetes = datasets.load_diabetes()
X = diabetes.data
y = diabetes.target

# Split dataset into training and test sets
X_train, X_test, y_train, y_test = train_test_split(X[:, :1], y, test_size=0.2)

# Define regressor from lab 6
class Regressor:
    def __init__(self, learning_rate=0.1, n_iterations=1000):
        self.learning_rate = learning_rate
        self.n_iterations = n_iterations
    
    def fit(self, X, y):
        X = np.insert(X, 0, 1, axis=1)
        n_samples, n_features = X.shape
        self.weights = np.zeros(n_features)
        for _ in range(self.n_iterations):
            y_predicted = X.dot(self.weights)
            grad_w = 1/n_samples * X.T.dot(y_predicted - y)
            self.weights -= self.learning_rate * grad_w
    
    def predict(self, X):
        X = np.insert(X, 0, 1, axis=1)
        return X.dot(self.weights)

# Instantiate regressor and fit to training data
reg = Regressor()
reg.fit(X_train, y_train)

# Make predictions on test set using both regressors
y_pred_reg = reg.predict(X_test)
y_pred_lin = linear_model.LinearRegression().fit(X_train, y_train).predict(X_test)

# Calculate mean squared error for both regressors
mse_reg = mean_squared_error(y_test, y_pred_reg)
mse_lin = mean_squared_error(y_test, y_pred_lin)

print("MSE of lab 6 regressor: ", mse_reg)
print("MSE of sklearn LinearRegression: ", mse_lin)

# Run regressors on diabetes dataset with 1 input parameter and 10 input parameters
reg1 = Regressor()
reg1.fit(X[:, :1], y)
y_pred_reg1 = reg1.predict(X[:, :1])
mse_reg1 = mean_squared_error(y, y_pred_reg1)

lin1 = linear_model.LinearRegression()
lin1.fit(X[:, :1], y)
y_pred_lin1 = lin1.predict(X[:, :1])
mse_lin1 = mean_squared_error(y, y_pred_lin1)

lin10 = linear_model.LinearRegression()
lin10.fit(X, y)
y_pred_lin10 = lin10.predict(X)
mse_lin10 = mean_squared_error(y, y_pred_lin10)

print("MSE of lab 6 regressor on 1 input parameter: ", mse_reg1)
print("MSE of sklearn LinearRegression on 1 input parameter: ", mse_lin1)
print("MSE of sklearn LinearRegression on 10 input parameters: ", mse_lin10)
