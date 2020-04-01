#include <conio.h>
#include <cctype>

#include "Console.h"
#include "AttributeBuilder.h"
#include "ScreenBuilder.h"

#pragma region Initialization & Disposal

ScreenBuilder::ScreenBuilder(Console const &console) {
	_console = const_cast<Console *>(&console);
}

#pragma endregion

#pragma region Builder handling

ScreenBuilder &ScreenBuilder::title(string_view name) {
	cout << endl;
	cout << _console->attr(name).title() << endl;
	cout << endl;
	return *this;
}

ScreenBuilder &ScreenBuilder::option(string_view key, string_view name) {
	_options.emplace_back(key);
	cout << "[" << attr(key).emphasized() << "] " << attr(name).dimmed() << endl;
	return *this;
}

int ScreenBuilder::wait() {
	int result = -1;

	while (result < 0) {
		int ch = _getch();

		int index = 0;
		for (auto key : _options) {
			auto first = tolower(key[0]);
			if (first == ch) {
				result = index;
				break;
			}
			index++;
		}
	}

	return result;
}

#pragma endregion

#pragma region Helpers

AttributeBuilder ScreenBuilder::attr(string_view s) {
	return _console->attr(s);
}

#pragma endregion
