import numpy as np
import matplotlib.pyplot as plt
num_samples = 100
legs = np.random.uniform(1.0, 4.0, num_samples)
heights = np.random.uniform(1.0, 2.5, num_samples)



plt.scatter(legs, heights)
plt.show()


def perceptron(x, y, learning_rate, num_iterations):
    weights = np.zeros((x.shape[1], 1))
    bias = 0
    for i in range(num_iterations):
        # calculate the predicted output
        y_pred = np.dot(x, weights) + bias
        
        # calculate the error
        error = y - y_pred
        
        # update the weights and bias
        weights += learning_rate * np.dot(x.T, error)
        bias += learning_rate * np.sum(error)
    return weights, bias


x = np.column_stack((legs, np.ones(num_samples)))
y = heights.reshape(num_samples, 1)
learning_rate = 0.001
num_iterations = 1000
weights, bias = perceptron(x, y, learning_rate, num_iterations)

plt.scatter(legs, heights)
plt.plot(legs, np.dot(x, weights) + bias, color='red')
plt.show()


y_pred = np.dot(x, weights) + bias
mse = np.mean((y - y_pred)**2)
rmse = np.sqrt(mse)
print(f"Root-mean-square error: {rmse}")


learning_rate = 0.001
num_iterations = 10000
errors = []
for i in range(num_iterations):
    # calculate the predicted output
    y_pred = np.dot(x, weights) + bias
    
    # calculate the error
    error = y - y_pred
    errors.append(np.mean(error**2))
    
    # update the weights and bias
    weights += learning_rate * np.dot(x.T, error)
    bias += learning_rate * np.sum(error)
    
    if i % 100 == 0:
        print(f"Iteration: {i}, Error: {errors[-1]}")

plt.plot(range(num_iterations), errors)
plt.xlabel("Iterations")
plt.ylabel("Error")
plt.show()

learning_rates = [0.0001, 0.001, 0.01, 0.1]
num_iterations = 10000
plt.figure(figsize=(10, 6))

for learning_rate in learning_rates:
    weights = np.zeros((x.shape[1], 1))
    bias = 0
    errors = []
    for i in range(num_iterations):
        # calculate the predicted output
        y_pred = np.dot(x, weights) + bias

        # calculate the error
        error = y - y_pred
        errors.append(np.mean(error**2))

        # update the weights and bias
        weights += learning_rate * np.dot(x.T, error)
        bias += learning_rate * np.sum(error)

    plt.plot(range(num_iterations), errors, label=f"Learning rate: {learning_rate}")

plt.xlabel