#pragma once

#include <iostream>

using namespace std;

class Console;

/*
 * Builds attributes for stream output.
 *
 * You typically don't have to create instances manually, use [Console] instead.
 */
class AttributeBuilder {
public:
	explicit AttributeBuilder(Console const &console, string_view const &s);

public:
	/* Applies title attributes. */
	AttributeBuilder &title();

public:
	AttributeBuilder &color(WORD color);
	AttributeBuilder &emphasized();
	AttributeBuilder &dimmed();
	AttributeBuilder &bold();
	AttributeBuilder &underline();
	AttributeBuilder &reversed();

public:
	friend ostream &operator <<(ostream &os, AttributeBuilder &a);

private:
	AttributeBuilder &update(WORD attributes);
	void applyAttributesToConsole(WORD attributes);

private:
	Console *_console;
	string_view _text;
	WORD _originalAttributes;
	WORD _attributes;
};



