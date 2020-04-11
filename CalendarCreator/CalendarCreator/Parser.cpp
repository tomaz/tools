#include <fstream>
#include <string>
#include "utils.h"
#include "Parser.h"

using namespace std;
using namespace utils;

std::vector<Event> Parser::parse(std::string filename) {
	vector<Event> result;
	vector<wstring> eventLines;

	wifstream file(filename);
	
	wstring line;
	while (std::getline(file, line)) {
		// Create event with gathered data once we reach empty line.
		if (line.empty()) {
			result.push_back(Event(eventLines));
			eventLines.clear();
			continue;
		}

		// Otherwise fill in the lines to temporary array.
		eventLines.push_back(line);
	}

	return result;
}
