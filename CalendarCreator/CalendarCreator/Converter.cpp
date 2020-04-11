#include <ctime>
#include <fstream>
#include <sstream>
#include <iomanip>
#include "utils.h"
#include "Converter.h"

using namespace std;

void Converter::convert(std::vector<Event> events, std::string path) {
	wofstream output(path);

	output << L"BEGIN:VCALENDAR" << endl;
	output << L"PRODID:-//Tomaz Kragelj//Calendar//EN" << endl;
	output << L"VERSION:2.0" << endl;
	output << L"CALSCALE:GREGORIAN" << endl;
	output << L"METHOD:PUBLISH" << endl;
	output << L"X-WR-CALNAME:\(arguments.calendarName)" << endl;
	output << L"X-WR-TIMEZONE:UTC" << endl;
	output << L"X-WR-CALDESC:" << endl;

	for (auto event : events) {
		wcout << "- Generating '" << event.name() << "'" << endl;

		auto id = uuid::generate_uuid_v4();
		auto uuid = utils::replace(id, "-", "");

		output << L"BEGIN:VEVENT" << endl;
		output << L"DTSTART:" << formatTime(event.fromTime()) << endl;
		output << L"DTEND:" << formatTime(event.toTime()) << endl;
		output << L"DTSTAMP:" << formatTime(event.fromTime()) << endl;
		output << L"UID:" << utils::string_to_wstring(uuid) << L"@gentlebytes.com" << endl;
		output << L"CREATED:" << formatTime(utils::now()) << endl;
		output << L"DESCRIPTION:" << endl;
		output << L"LAST-MODIFIED:" << formatTime(utils::now()) << endl;
		output << L"LOCATION:" << event.location() << endl;
		output << L"SEQUENCE:0" << endl;
		output << L"STATUS:CONFIRMED" << endl;
		output << L"SUMMARY:" << event.name() << endl;
		output << L"TRANSP:TRANSPARENT" << endl;
		output << L"END:VEVENT" << endl;
	}

	output << L"END:VCALENDAR" << endl;

	output.close();
}

std::wstring Converter::formatTime(const std::tm &time) const {
	ostringstream ss;
	ss << put_time(&time, "%Y-%m-%dT%H:%M:%S");
	return utils::string_to_wstring(ss.str());
}
