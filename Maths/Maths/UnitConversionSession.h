#pragma once

#include <iostream>
#include <vector>

#include "BigDecimal.h"

namespace UnitConversion {

	using namespace std;

	enum class Result;

	/// Helper class for managing a session within <see cref="ScreenUnitConversion"/>.
	class Session {
	public:
		Session();

	public:
		vector<Unit> units();

		void prepareNewConversion();
		void printQuestion(Console& console);
		void printSolution(Console& console);
		Result askForSolution(Console& console);

		int attempts();

	private:
		wstring _measure;
		Unit _fromUnit;
		Unit _toUnit;
		BigDecimal _question;
		BigDecimal _solution;
		int _attempts;

		vector<Unit> _units;
		vector<wstring> _measures;
	};

	enum class Result {
		exit,
		match,
		wrong,
	};
}