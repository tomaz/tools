#pragma once

#include "Screen.h"
#include "UnitConversionUnit.h"
#include "UnitConversionSession.h"

using namespace UnitConversion;

/**
 * Runs unit conversion program.
 */
class ScreenUnitConversion : public Screen {
public:
	using Screen::Screen;

protected:
	bool onRun(Console &console) override;

private:
	void printUnit(Console &console, Unit &unit);

private:
	Session _session;
};
