#pragma once

#include <iostream>
#include <vector>

using namespace std;

class Console;
class AttributeBuilder;

/*
 * Helper class for unifying common screen related functionality.
 */
class ScreenBuilder {
public:
	explicit ScreenBuilder(Console const &console);

public:
	/* Renders a title element with the given name. */
	ScreenBuilder &title(string_view name);

	/* Renders an option with the given key and name. */
	ScreenBuilder &option(string_view key, string_view name);

	/* Waits for next char and returns matched option index. */
	int wait();

private:
	AttributeBuilder attr(string_view s);

private:
	Console *_console;
	vector<string_view> _options;
};
