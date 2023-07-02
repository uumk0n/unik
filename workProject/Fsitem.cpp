#include "Fsitem.h"
#include "FsitemManager.h"

int Fsitem::id_counter = 0;

ostream& operator<<(ostream& stream, Fsitem o)
{
	int mode = o.getPrintMode();
	if (mode == PRINT_PARENT_CHILDREN_PATH)
	{
		o.printInfo(stream);
	}
	else if (mode == PRINT_FOR_TREE)
	{
		o.printTree(stream, true);
	}
	return stream;
}
ostream& operator << (ostream & stream, Fsitem * o)
{
	int mode = o->getPrintMode();
	if (mode == PRINT_PARENT_CHILDREN_PATH) {
		o->printInfo(stream);
	}
	else if (mode == PRINT_FOR_TREE) {
		o->printTree(stream, true);
	}
	return stream;
}
void Fsitem::printInfo(ostream& stream)
{
	stream << "[Fsitem #" << id << ":" << path << "]";
	if (parent)
	{
		stream << endl << "parent: " << parent->getPath();
	}
	size_t cSize = children.size();
	if (cSize)
	{
		stream << endl << "children:" << endl;
		for (int i = 0; i < cSize; i++)
		{
			stream << "        " << children[i] -> getPath() << endl;
		}
	}
}
void Fsitem::printTree(ostream& stream, bool isSort = false)
{
	string before(depth, ' ');
	stream << before << name;

	if (isSort)
	{
		sort(children.begin(), children.end(), _sortItems);
	}
	size_t cSize = children.size();
	if (cSize)
	{
		for (int i = 0; i < cSize; i++)
		{
			stream << endl << children[i];
		}
	}
	stream.clear();
}
Fsitem::Fsitem()
{
	parent = NULL;
	id = Fsitem::id_counter++;
	printMode = PRINT_FOR_TREE;
}
Fsitem::~Fsitem()
{	
	if (parent)
		delete parent;
	children.clear();
}

int Fsitem::getPrintMode()
{
	return printMode;
}
int Fsitem::getId()
{
	return id;
}
void Fsitem::setName(const string theName)
{
	name = theName;
}
string Fsitem::getName()
{
	return name;
}

void Fsitem::setPath(const string thePath)
{
	path = thePath;
}
string Fsitem::getPath()
{
	return path;
}

void Fsitem::setType(int theType)
{
	type = theType == FS_FILE ? FS_FILE : FS_DIR;
}
int Fsitem::getType()
{
	return type;
}

void Fsitem::setDepth(int theDepth)
{
	depth = theDepth;
}
int Fsitem::getDepth()
{
	return depth;
}

void Fsitem::setSize(int theSize)
{
	size = theSize;
}
int Fsitem::getSize()
{
	return size;
}

void Fsitem::setParent(Fsitem* theParent)
{
	parent = theParent;
}
Fsitem* Fsitem::getParent()
{
	return parent;
}

void Fsitem::addChild(Fsitem* theChild)
{
	size_t cSize = children.size();
	for (int i = 0; i < cSize; i++)
	{
		if (theChild == children[i]) return;
	}
	children.push_back(theChild);
}
vector<Fsitem*> Fsitem::getChildren()
{
	return children;
}

bool _sortItems(Fsitem* a, Fsitem* b)
{
	int aType = a->getType(),
		bType = b->getType(),
		aSize = a->getSize(),
		bSize = b->getSize();
	string aName = a->getName(),
		bName = b->getName();

	if (aType!=bType)//директории
	{
		return aType < bType;
	}
	if (aType == FS_DIR && bType == FS_DIR)//директории по имени
	{
		return aName < bName;
	}
	if (aType == FS_FILE && bType == FS_FILE && aSize != bSize)//файлы по размеру
	{
		return aSize > bSize;
	}
	return aName < bName;
}