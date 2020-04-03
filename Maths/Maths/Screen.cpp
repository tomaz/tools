#include "Screen.h"

#pragma region Initialization & Disposal

Screen::Screen() {
	_window = nullptr;
	_width = 0;
	_height = 0;
	_isStarted = false;
}

Screen::~Screen() {
	end();
}

#pragma endregion

#pragma region Creation

void Screen::start() {
	if (_isStarted) return;

	_isStarted = true;

	// Enable UTF-8. Must be called prior to initializing the screen.
	setlocale(LC_ALL, "");

	_window = initscr();
	enableCursor(false);
	noecho();

	getmaxyx(stdscr, _height, _width);

	start_color();
}

void Screen::end() {
	if (!_isStarted) return;
	endwin();
}

#pragma endregion

#pragma region Handling

void Screen::enableCursor(bool enable) const {
	curs_set(enable ? 1 : 0);
}

void Screen::registerColors(short slot, short foreground, short background) const {
	init_pair(slot, foreground, background);
}

void Screen::attributed(chtype attributes, std::function<void(const Screen &)> handler) const {
	attron(attributes);
	handler(*this);
	attroff(attributes);
}

void Screen::draw(std::function<void(const Screen &)> handler) const {
	handler(*this);
	refresh();
}

void Screen::restorePosition(std::function<void(const Screen &)> handler) const {
	auto x = currentX();
	auto y = currentY();

	handler(*this);

	move(y, x);
}

#pragma endregion

#pragma region Getters & Setters

int Screen::currentX() const {
	return getcurx(_window);
}

int Screen::currentY() const {
	return getcury(_window);
}

int Screen::width() const {
	return _width;
}

int Screen::height() const {
	return _height;
}

#pragma endregion

#pragma region Helpers

void printeol(std::string s) {
	int width, height;
	getmaxyx(stdscr, height, width);

	int x, y;
	getyx(stdscr, y, x);

	while (x < width) {
		if (x + s.length() <= (size_t)width) {
			printstd(s);
		} else if (x < width - 1) {
			printstd(s.substr(0, width - x - 1));
		}
		x += s.length();
	}
}

void printstd(std::string s) {
	printw(s.c_str());
}

void printstdw(std::wstring s) {
	for (auto ch : s) {
		addrawch(ch);
	}
}

void mvprintstdw(int y, int x, std::wstring s) {
	move(y, x);
	printstdw(s);
}

void mvprintstdwc(int y, std::wstring s) {
	int width, height;
	getmaxyx(stdscr, height, width);

	mvprintstdw(y, (width - s.length()) / 2, s);
}

void mvprintstd(int y, int x, std::string s) {
	mvprintw(y, x, s.c_str());
}

void mvprintstdc(int y, std::string s) {
	int width, height;
	getmaxyx(stdscr, height, width);

	mvprintstd(y, (width - s.length()) / 2, s);
}

void mvprintc(int y, const char *s) {
	int width, height;
	getmaxyx(stdscr, height, width);
	mvprintw(y, (width - strlen(s)) / 2, s);
}

#pragma endregion
