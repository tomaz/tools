// CalendarCreator.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <filesystem>
#include "Parser.h"
#include "Converter.h"

using namespace std;

namespace fs = filesystem;

fs::path filename(const string &path) {
	return filesystem::path(path).filename();
}

void printHelp(const string &path) {
	cout << "USAGE " << filename(path) << " <source>" << endl;
	cout << endl;
	cout << "Source should contain list of events separated by empty line" << endl;
	cout << endl;
	cout << "Format of each event:" << endl;
	cout << "YYYYMMDD" << endl;
	cout << "HHMM-HHMM" << endl;
	cout << "title wstring" << endl;
	cout << "location wstring" << endl;
	cout << endl;
	cout << "Output: <source>.ics" << endl;
}

int main(int argc, char **argv) {
	if (argc <= 1) {
		printHelp(argv[0]);
		return 0;
	}

	auto path = fs::path(argv[1]);
	auto canonicalPath = fs::canonical(path);
	auto source = canonicalPath.string();
	cout << "Parsing " << path.string() << endl;

	Parser parser{};
	auto events = parser.parse(source);
	cout << "Found " << events.size() << " events" << endl;

	Converter converter{};
	auto output = canonicalPath.replace_extension("ics");
	auto destination = output.string();
	converter.convert(events, destination);

	return 0;
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
