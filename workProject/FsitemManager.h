#ifndef  _FS_ITEM_MANAGER_H
#define _FS_ITEM_MANAGER_H

#include<istream>
#include <fstream>
#include <vector>
#include "Fsitem.h"

using namespace std;

class FsitemManager
{
public:
	FsitemManager();
	~FsitemManager();


	void print();
	bool addPath(const string path);
	void addFsitem(Fsitem* item);
	void addFsitem(Fsitem* item, const string prePath);
	void setRoot();
	Fsitem* getRoot();
protected:
	vector<string> paths;
	vector<Fsitem*> items;

};
#endif 
