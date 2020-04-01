#pragma once

#include "Screen.h"

/**
 * Handles the main menu screen.
 */
class ScreenMenu : public Screen {
public:
	using Screen::Screen;

protected:
	bool onRun(Console &console) override;

private:
	Console *_console;
};
