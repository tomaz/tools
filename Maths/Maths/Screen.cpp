//
// Created by Tomaz on 31. 03. 20.
//

#include "Screen.h"

Screen::Screen(Console const &console) {
	_console = const_cast<Console *>(&console);
}

void Screen::run() {
	while (true) {
		// Continue until subclass returns false.
		if (!onRun(*_console)) break;
	}
}

AttributeBuilder Screen::attr(string_view s) {
	return _console->attr(s);
}
