#ifndef _PARSER_H
#define _PARSER_H

#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>
#include "Fsitem.h"
#include "FsitemManager.h"

using namespace std;

class Parser
{
public:
	Parser();
	~Parser();


	void setManager(FsitemManager* theManager);
	void parse(const char* dataFilePath);
	static vector<string> split_string(const string& s, char delim);

private:
	FsitemManager* manager;
	void _processline(const string line);
	void _processitem(const vector<string>& items, int count, int size, int type);
	static vector<string>& _split_string(const string& s, char delim, vector<string>& elems);
};
#endif 
