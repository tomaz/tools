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

void View::onInit() {
	// Nothing to do here by default.
}

void View::run() {
	onInit();

	_screen.start();

	drawView();

	while (onHandle()) {}

	_screen.end();
}

#pragma endregion

#pragma region Subclass

bool View::onHandle() {
	return false;
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

	drawView();
}

#pragma endregion

#pragma region Helpers

void View::drawView() {
	clear();

	onDraw(_screen);
}

#pragma endregion
