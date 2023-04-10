import time
from random import randint
import numpy as np
import funcions

print("Введите диапазон")
x=int(input())
y=int(input())


list = []
for i in range(10**7):
    list.append(randint(x, y))

array = np.random.randint(x, y, size=(10**7))

def arrayTime(f,fn):
    start_time=time.time()
    f(array,6) 
    end_time=time.time()
    startT=time.time()
    fn(array,6)
    endT=time.time()
    return print(f(array,6),(end_time - start_time),"/",fn(array,6),(endT-startT))

def listTime(f,fn):
    start_time=time.time()
    f(list,6) 
    end_time=time.time()
    startT=time.time()
    fn(list,6)
    endT=time.time()
    return print(f(list,6),(end_time - start_time),"/",fn(list,6),(endT-startT))

def myFunc(f):
    start_time=time.time()
    f(list,6) 
    end_time=time.time()
    return print(f(list,6),(end_time - start_time))

print("Intel® Core™ i5-10210U CPU @ 1.60GHz × 8")

# for a in array in range(0,10):
#     print(a)
# for l in list in range(0,10):
#     print(l)

listTime(funcions.fin1,funcions.fNin1)

arrayTime(funcions.fin2,funcions.fNin2)
  
listTime(funcions.fin3,funcions.fNin3)

listTime(funcions.fin4,funcions.fNin4)

arrayTime(funcions.fin1,funcions.fNin1)

arrayTime(funcions.fin3,funcions.fNin3)

arrayTime(funcions.fin4,funcions.fNin4)

myFunc(funcions.binary_search)