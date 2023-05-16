import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.datasets import load_iris
from sklearn.linear_model import Perceptron
from sklearn.svm import LinearSVC

# load the iris dataset
iris = load_iris()

# extract the first two and first four features of the dataset
X_2d = iris.data[:, :2]
X_4d = iris.data[:, :4]

# select only the versicolor and virginica classes
y = iris.target
mask = (y == 1) | (y == 2)
X_2d = X_2d[mask]
X_4d = X_4d[mask]
y = y[mask] - 1

# split the data into a training set and a test set
X_train_2d, X_test_2d, y_train, y_test = train_test_split(X_2d, y, test_size=0.2, random_state=42)
X_train_4d, X_test_4d, y_train, y_test = train_test_split(X_4d, y, test_size=0.2, random_state=42)

# create instances of the Perceptron and LinearSVC classifiers
perceptron_2d = Perceptron(random_state=42)
perceptron_4d = Perceptron(random_state=42)
svm_2d = LinearSVC(random_state=42)
svm_4d = LinearSVC(random_state=42)

# fit the classifiers to the training data
perceptron_2d.fit(X_train_2d, y_train)
perceptron_4d.fit(X_train_4d, y_train)
svm_2d.fit(X_train_2d, y_train)
svm_4d.fit(X_train_4d, y_train)

# calculate the percentage of classification errors for the test data
perceptron_2d_error = 1 - perceptron_2d.score(X_test_2d, y_test)
perceptron_4d_error = 1 - perceptron_4d.score(X_test_4d, y_test)
svm_2d_error = 1 - svm_2d.score(X_test_2d, y_test)
svm_4d_error = 1 - svm_4d.score(X_test_4d, y_test)

# output the classification errors as percentages
print("Perceptron (2d): {:.2%}".format(perceptron_2d_error))
print("Perceptron (4d): {:.2%}".format(perceptron_4d_error))
print("SVM (2d): {:.2%}".format(svm_2d_error))
print("SVM (4d): {:.2%}".format(svm_4d_error))
