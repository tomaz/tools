//
// Created by Tomaz on 31. 03. 20.
//

#include "UnitConversionUnit.h"

namespace UnitConversion {

	Unit::Unit() {
		_name = L"";
		_scale = BigDecimal();
	}

	Unit::Unit(wstring const& name, BigDecimal scale) {
		_name = name;
		_scale = scale;
	}

	BigDecimal Unit::toStandard(BigDecimal value) {
		return value * _scale;
	}

	BigDecimal Unit::fromStandard(BigDecimal standard) {
		return standard / _scale;
	}

	wstring Unit::name() {
		return _name;
	}

	BigDecimal Unit::scale() {
		return _scale;
	}

}
