#pragma once

#include "Console.h"

/**
 * Base class for managing screens.
 */
class Screen {
public:
	explicit Screen(Console const &console);

	/** Runs the screen until end condition is met. */
	virtual void run();

protected:
	/** Runs the screen and returns true if run loop should continue, or false if it should end. */
	virtual bool onRun(Console &console) = 0;

	/** Prepares [AttributeBuilder] for the given string. */
	AttributeBuilder attr(string_view s);

private:
	Console *_console;
};
