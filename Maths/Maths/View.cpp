#include "View.h"

#pragma region Initialization & Disposal

View::View() {
	_screen = Screen();
}

View::~View() {
	_screen.end();
}

#pragma endregion

#pragma region Public

void View::run() {
	_screen.start();

	onInit(_screen);
	redraw();

	while (onHandle()) {}

	_screen.end();
}

#pragma endregion

#pragma region Subclass

void View::onInit(const Screen &screen) {
	// Nothing to do here by default.
}

bool View::onHandle() {
	return false;
}

#pragma endregion

#pragma region Helpers

void View::redraw() {
	clear();

	onDraw(_screen);
}

void View::draw(std::function<void(const Screen &)> handler) {
	handler(_screen);

	refresh();
}

void View::restorePosition(std::function<void(const Screen &)> handler) {
	_screen.restorePosition(handler);
}

void View::switchView(std::function<void(void)> handler) {
	_screen.end();

	handler();

	_screen.start();

	redraw();
}

#pragma endregion
