//
// Created by Tomaz on 31. 03. 20.
//

#include "Console.h"
#include "AttributeBuilder.h"

#pragma region Initialization & Disposal

AttributeBuilder::AttributeBuilder(Console const &console, string_view const &s) {
	_console = const_cast<Console *>(&console);
	_text = s;
	_attributes = _console->attributes();
	_originalAttributes = _attributes;
}

#pragma endregion

#pragma region Attributes

AttributeBuilder &AttributeBuilder::title() {
	return color(217);
}

#pragma endregion

#pragma region Builder

AttributeBuilder &AttributeBuilder::color(WORD color) {
	return update(color);
}

AttributeBuilder &AttributeBuilder::emphasized() {
	return color(0);
}

AttributeBuilder &AttributeBuilder::dimmed() {
	return color(1);
}

AttributeBuilder &AttributeBuilder::bold() {
	return update(FOREGROUND_INTENSITY);
}

AttributeBuilder &AttributeBuilder::underline() {
	return update(COMMON_LVB_UNDERSCORE);
}

AttributeBuilder &AttributeBuilder::reversed() {
	return update(BACKGROUND_INTENSITY);
}

AttributeBuilder &AttributeBuilder::update(WORD attributes) {
	_attributes += attributes;
	return *this;
}

void AttributeBuilder::applyAttributesToConsole(WORD attributes) {
	_console->setAttributes(attributes);
}

#pragma endregion

#pragma region Operators

ostream &operator<<(ostream &os, AttributeBuilder &a) {
	a.applyAttributesToConsole(a._attributes);
	os << a._text;
	a.applyAttributesToConsole(a._originalAttributes);
	return os;
}

#pragma endregion