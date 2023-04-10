#ifndef  _SMART_POINTER_H
#define _SMART_POINTER_H
#include "Fsitem.h"
#include "FsitemManager.h"
#include "Parser.h"
using namespace std;

template<typename T>
class SmartPointer
{
public:
	SmartPointer(T* ptr)
	{
		this->ptr = ptr;
	}
	~SmartPointer()
	{
		delete ptr;
	}
	T& operator*()
	{
		return *ptr;
	}
private:
	T* ptr;
};
#endif 