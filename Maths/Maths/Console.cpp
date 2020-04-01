#include "Console.h"

#pragma region Initialization & Disposal

Console::Console() {
	_handle = GetStdHandle(STD_OUTPUT_HANDLE);
}

#pragma endregion

#pragma region Console handling

HANDLE Console::handle() {
	return _handle;
}

WORD Console::attributes() {
	CONSOLE_SCREEN_BUFFER_INFO info;
	GetConsoleScreenBufferInfo(_handle, &info);
	return info.wAttributes;
}

void Console::setAttributes(WORD attributes) {
	SetConsoleTextAttribute(_handle, attributes);
}

#pragma endregion

#pragma region Attributes

ScreenBuilder Console::screen(bool clear) {
	if (clear) {
		system("cls");
	}
	return ScreenBuilder(*this);
}

AttributeBuilder Console::attr(string_view s) {
	return AttributeBuilder(*this, s);
}

#pragma endregion
