#pragma once

#include <iostream>
#include <vector>

namespace uuid {

    std::string generate_uuid_v4();
}

namespace utils {

    const static char *ws = " \t\n\r\f\v";

    // trim from end of string (right)
    std::string &rtrim(std::string &s, const char *t = ws);

    // trim from beginning of string (left)
    std::string &ltrim(std::string &s, const char *t = ws);

    // trim from both ends of string (right then left)
    std::string &trim(std::string &s, const char *t = ws);

    // replaces all occurences of from string with to
    std::string &replace(std::string &s, const std::string &from, const std::string &to);

    // splits given string by the given delimiter into array of strings
    std::vector<std::string> split(const std::string &s, const char delimiter);

    // converts wstring to string
    std::string wstring_to_string(std::wstring source);

    // converts string to wstring
    std::wstring string_to_wstring(std::string source);

    // returns current system time
    std::tm now();
}
