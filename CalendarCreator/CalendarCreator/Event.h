#pragma once

#include <iostream>
#include <vector>

class Event {
public:
	Event(std::vector<std::wstring> data);

public:
	std::wstring name();
	std::wstring location();
	std::tm fromTime();
	std::tm toTime();

private:
	void parse(std::vector<std::wstring> data);
	void parseDate(std::string data);
	void parseTime(std::string data);
	
private:
	std::tm _fromTime{};
	std::tm _toTime{};
	std::wstring _name{ L"" };
	std::wstring _location{ L"" };
};
