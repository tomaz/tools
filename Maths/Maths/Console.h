#pragma once

#include <Windows.h>
#include <iostream>
#include "ScreenBuilder.h"
#include "AttributeBuilder.h"

using namespace std;

/*
 * Manages the console.
 *
 * To use, create an instance and call various functions.
 */
class Console {
	friend class AttributeBuilder;

public:
	Console();

public:

	/** Starts a new screen. */
	ScreenBuilder screen(bool clear = true);

	/** Prepares [AttributeBuilder] used to apply various attributes. */
	AttributeBuilder attr(string_view s);

	/** Returns the underlying console handle. */
	HANDLE handle();

private:
	/** Returns current console attributes. */
	WORD attributes();

	/** Sets given attributes to console. */
	void setAttributes(WORD attributes);

private:
	HANDLE _handle;
};
