#include <cstdlib>
#include <iostream>
#include "FsitemManager.h"
#include"SmartPointer.h"
#include "Parser.h"
#define __CRTDBG_MAP_ALLOC

int main(int argc, char* argv[])
{
	{const char* dataFilePath = "input.txt";
	if (argc >= 2)
	{
		dataFilePath = argv[1];
	}
	FsitemManager manager;
	manager.setRoot();
	Parser parser = Parser();
	parser.setManager(&manager);
	parser.parse(dataFilePath);

	manager.print();
	}
	return 0;
}