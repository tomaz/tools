#pragma once

#include <iostream>
#include "BigDecimal.h"

namespace UnitConversion {

	class Unit {
	public:
		Unit();
		Unit(std::wstring const& name, BigDecimal scale);

	public:
		/** Converts given value in this unit to standard value to be passed to destination [Unit]s [fromStandard. */
		BigDecimal toStandard(BigDecimal &value);

		/** Converts given standard value to this unit. */
		BigDecimal fromStandard(BigDecimal &standard);

		std::wstring name();
		BigDecimal scale();
		bool isStandard();

	private:
		std::wstring _name;
		BigDecimal _scale;
	};
}
