#include "Parser.h"

Parser::Parser()
{
	manager = nullptr;
}

Parser::~Parser()
{
	manager = NULL;
		delete manager;
}

void Parser::setManager(FsitemManager* theManager)
{
	manager = theManager;
}

void Parser::parse(const char* dataFilePath)
{
	ifstream file(dataFilePath);
	if (!file)
	{
		cout << "error opening " << dataFilePath << "file" << endl;
	}
	cout << "reading file" << endl;
	string line;
	while (getline(file, line))
	{
		_processline(line);
	}
	file.close();
	return;
}

void Parser::_processline(const string line)
{
	vector<string> parts = Parser::split_string(line, ' ');
	size_t partsSize = parts.size();
	int itemType = FS_DIR;
	int fileSize = 0;
	if (partsSize < 1)
	{
		return;
	}
	else if (partsSize == 2)//тип последнего элемента
	{
		itemType = FS_FILE;
		istringstream ss(parts[1]);
		ss >> fileSize;
	}
	vector<string> items = Parser::split_string(parts[0], '\\');
	for (int i = 0; i < items.size(); i++)
	{
		Parser::_processitem(items, i, fileSize, itemType);
	}
		
}

void Parser::_processitem(const vector<string>& items, int count, int size, int itemType)
{
	size_t iSize = items.size();
	string path = "/", prePath;
	int segType = FS_DIR;

	for (int i = 0; i <= count; i++)
	{
		if (i == count)
		{
			prePath = path;
		}
		path += items[i];
		if (i == count && iSize - 1 == count && itemType == FS_FILE)
		{
			segType = FS_FILE;
		}
		else path += "/";
	}
	if (!manager->addPath(path)) return;

	if (segType != FS_FILE) size = 0;

	Fsitem* item = new Fsitem();
	item->setName(items[count]);
	item->setPath(path);
	item->setType(segType);
	item->setDepth(count + 1);
	item->setSize(size);
	manager->addFsitem(item, prePath);
}
vector <string>& Parser::_split_string(const string& s, char delim, vector<string>& parts)
{
	stringstream ss(s);
	string part;
	while (getline(ss, part, delim))
	{
		parts.push_back(part);
	}
	return parts;
}
vector <string> Parser::split_string(const string& s, char delim)
{
	vector<string> parts;
	return Parser::_split_string(s, delim, parts);
}