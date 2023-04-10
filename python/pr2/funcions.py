import numpy as np


def fin1(list,key):
    if key in list:return True
    else:return False

def fin2(array,key):
    if np.where(array==key):return True
    else:return False

def fin3(list,key):
    for el in list:
        if el == key:
            return True
    return False 

def fin4(list, key):
    find=False
    i=0
    while find==False:
        if(list[i]==key):
            find=True
        i+=1
    return find

#----------NOT IN----------
def fNin1(list,key):
    if key not in list:return True
    else:return False

def fNin2(array,key):
    if np.where(array!=key):return True
    else:return False

def fNin3(list,key):
    for el in list:
        if el != key:
            return True
    return False 

def fNin4(list, key):
    notfind=False
    i=0
    while notfind==False:
        if(list[i]!=key):
            notfind=True
        i+=1
    return notfind

def binary_search(list, item):
    low = 0
    high = len(list) - 1

    while low <= high:
        mid = (low + high) // 2
        guess = list[mid]
        if guess == item:
            return True
        if guess > item:
            high = mid - 1
        else:
            low = mid + 1
    return False