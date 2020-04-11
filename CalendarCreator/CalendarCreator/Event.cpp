#include <sstream>
#include <locale>
#include <iomanip>
#include "utils.h"
#include "Event.h"

using namespace std;

namespace operators {
	ostream &operator<<(ostream &os, const vector<wstring> &input) {
		for (auto const &s : input) {
			os << s.c_str() << endl;
		}
		return os;
	}
}

#pragma region Initialization & Disposal

Event::Event(std::vector<std::wstring> data) {
	parse(data);
}

#pragma endregion

#pragma region Getters & Setters

std::wstring Event::name() {
	return _name;
}

std::wstring Event::location() {
	return _location;
}

std::tm Event::fromTime() {
	return _fromTime;
}

std::tm Event::toTime() {
	return _toTime;
}

#pragma endregion

#pragma region Helpers

void Event::parse(std::vector<std::wstring> data) {
	using namespace operators;

	if (data.size() < 4) {
		cerr << "ERROR: Data has " << data.size() << " lines, 4 expected!" << endl;
		cerr << data << endl;
		return;
	}

	parseDate(utils::wstring_to_string(data[0]));
	parseTime(utils::wstring_to_string(data[1]));

	_name = data[2];
	_location = data[3];

	wcout << "- " << _fromTime.tm_year + 1900 << "-";
	wcout << setfill(L'0') << setw(2) << _fromTime.tm_mon << "-";
	wcout << setfill(L'0') << setw(2) << _fromTime.tm_mday;
	wcout << " '" << _name << "'" << endl;
}

void Event::parseDate(std::string data) {
	// YYYYMMDD - can't use get_time as there's no delimiter...
	stringstream sy(data.substr(0, 4));
	sy >> _fromTime.tm_year;
	_fromTime.tm_year -= 1900;	// year is based off 1900
	_toTime.tm_year = _fromTime.tm_year;

	stringstream sm(data.substr(4, 2));
	sm >> _fromTime.tm_mon;
	_fromTime.tm_mon--;			// month is 0 based
	_toTime.tm_mon = _fromTime.tm_mon;

	stringstream sd(data.substr(6, 2));
	sd >> _fromTime.tm_mday;
	_toTime.tm_mday = _fromTime.tm_mday;
}

void Event::parseTime(std::string data) {
	// HHMM-HHMM
	vector<string> times = utils::split(data, '-');

	if (times.size() < 2) {
		cerr << "ERROR: Time has " << times.size() << " items, 2 expected!" << endl;
		cerr << data << endl;
		return;
	}

	auto parser = [&](const string &str, tm &time) {
		// HHMM
		stringstream sh(str.substr(0, 2));
		sh >> time.tm_hour;

		stringstream sm(str.substr(2, 2));
		sm >> time.tm_min;
	};

	parser(times[0], _fromTime);
	parser(times[1], _toTime);
}

#pragma endregion
