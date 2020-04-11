#pragma once

#include <iostream>
#include <vector>
#include "Event.h"

class Parser {
public:
	std::vector<Event> parse(std::string filename);
};