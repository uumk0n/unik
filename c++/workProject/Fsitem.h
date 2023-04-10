#ifndef _FS_ITEM_H
#define _FS_ITEM_H

#include <iostream>
#include <iomanip>
#include <fstream>
#include <vector>
#include <string>
#include <algorithm>

using namespace std;

const int FS_DIR = 1;
const int FS_FILE = 2;

const int PRINT_PARENT_CHILDREN_PATH = 1;
const int PRINT_FOR_TREE = 2;

class Fsitem
{
public :
	Fsitem();
	~Fsitem();
	int getId();
	int getPrintMode();


	void setName(const string theName);
	string getName();

	void setPath(const string thePath);
	string getPath();

	void setType(int  theType);
	int getType();

	void setDepth(int theDepth);
	int getDepth();

	void setSize(int theSize);
	int getSize();

	void setParent(Fsitem * theParetn);
	Fsitem* getParent();

	void addChild(Fsitem* theChild);
	vector<Fsitem*> getChildren();

	void printInfo(ostream& stream);
	void printTree(ostream& stream, bool isSort);
	
	friend ostream& operator<<(ostream& stream, Fsitem o);
	friend ostream& operator<<(ostream& stream, Fsitem* o);

protected:
	int printMode;
	int id;
	int depth;
	int size;
	int type;
	string name;
	string path;
	Fsitem* parent;
	vector<Fsitem*> children;
	static int id_counter;
};
bool _sortItems(Fsitem* a, Fsitem* b);
#endif 
