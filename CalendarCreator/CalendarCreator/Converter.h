#pragma once

#include <iostream>
#include <vector>
#include "Event.h"

class Converter {
public:
	void convert(std::vector<Event> events, std::string path);

private:
	std::wstring formatTime(const std::tm &time) const;
};
