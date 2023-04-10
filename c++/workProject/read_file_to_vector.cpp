#include "read_file_to_vector.h"

vector<string> read_file_to_vector()
{
    vector<string> vDataLines;
    ifstream fs(DATA_FILE_PATH.c_str());
    if (!fs)
    {
        cout << "Error opening " << DATA_FILE_PATH << " file!" << endl;
        return vDataLines;
    }

    cout << " reading file..." << endl;
  
    char buff[500];
    while(!fs.eof()) {
        fs.getline(buff, 500);
        cout << buff << endl;
        vDataLines.push_back(string(buff));
    }
    fs.close();
    return vDataLines;
}
