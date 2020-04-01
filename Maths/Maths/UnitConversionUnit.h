#pragma once

#include <iostream>
#include "BigDecimal.h"

namespace UnitConversion {

	using namespace std;

	/*
	 * Manages a unit with its corresponding value.
	 */
	class Unit {
	public:
		Unit();
		Unit(wstring const& name, BigDecimal scale);

	public:
		/** Converts given value in this unit to standard value to be passed to destination [Unit]s [fromStandard. */
		BigDecimal toStandard(BigDecimal value);

		/** Converts given standard value to this unit. */
		BigDecimal fromStandard(BigDecimal standard);

		wstring name();
		BigDecimal scale();

	private:
		wstring _name;
		BigDecimal _scale;
	};

}
