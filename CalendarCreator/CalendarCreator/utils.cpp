#include <sstream>
#include <locale>
#include <codecvt>
#include <string>
#include "utils.h"

#include <random>
#include <sstream>

namespace uuid {
	static std::random_device              rd;
	static std::mt19937                    gen(rd());
	static std::uniform_int_distribution<> dis(0, 15);
	static std::uniform_int_distribution<> dis2(8, 11);

	std::string generate_uuid_v4() {
		std::stringstream ss;
		int i;
		ss << std::hex;
		for (i = 0; i < 8; i++) {
			ss << dis(gen);
		}
		ss << "-";
		for (i = 0; i < 4; i++) {
			ss << dis(gen);
		}
		ss << "-4";
		for (i = 0; i < 3; i++) {
			ss << dis(gen);
		}
		ss << "-";
		ss << dis2(gen);
		for (i = 0; i < 3; i++) {
			ss << dis(gen);
		}
		ss << "-";
		for (i = 0; i < 12; i++) {
			ss << dis(gen);
		};
		return ss.str();
	}
}

namespace utils {

	std::string &rtrim(std::string &s, const char *t) {
		s.erase(s.find_last_not_of(t) + 1);
		return s;
	}

	std::string &ltrim(std::string &s, const char *t) {
		s.erase(0, s.find_first_not_of(t));
		return s;
	}

	std::string &trim(std::string &s, const char *t) {
		return ltrim(rtrim(s, t), t);
	}

	std::string &replace(std::string &str, const std::string &from, const std::string &to) {
		if (from.empty()) return str;

		size_t start_pos = 0;
		while ((start_pos = str.find(from, start_pos)) != std::string::npos) {
			str.replace(start_pos, from.length(), to);
			start_pos += to.length(); // In case 'to' contains 'from', like replacing 'x' with 'yx'
		}

		return str;
	}

	std::vector<std::string> split(const std::string &s, const char delimiter) {
		std::vector<std::string> result;
		std::stringstream ss(s);
		std::string part;
		
		while (std::getline(ss, part, delimiter)) {
			result.push_back(part);
		}

		return result;
	}

	std::string wstring_to_string(std::wstring source) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		return converter.to_bytes(source);
	}

	std::wstring string_to_wstring(std::string source) {
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		return converter.from_bytes(source);
	}

	std::tm now() {
		std::tm result;
		auto t = std::time(nullptr);
		localtime_s(&result, &t);
		return result;
	}
}