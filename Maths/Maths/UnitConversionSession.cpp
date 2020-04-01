#include <codecvt>
#include <random>
#include <chrono>

#include "Console.h"
#include "UnitConversionUnit.h"
#include "UnitConversionSession.h"

namespace UnitConversion {

#pragma region Initialization & Disposal

	Session::Session() {
		_units.emplace_back(Unit(L"M", BigDecimal("1000000")));
		_units.emplace_back(Unit(L"k", BigDecimal("1000")));
		_units.emplace_back(Unit(L"h", BigDecimal("100")));
		_units.emplace_back(Unit(L"da", BigDecimal("10")));
		_units.emplace_back(Unit(L"", BigDecimal("1")));
		_units.emplace_back(Unit(L"d", BigDecimal("0.1")));
		_units.emplace_back(Unit(L"c", BigDecimal("0.01")));
		_units.emplace_back(Unit(L"m", BigDecimal("0.001")));
		_units.emplace_back(Unit(L"u", BigDecimal("0.000001")));

		_measures.emplace_back(L"m");
		_measures.emplace_back(L"l");
		_measures.emplace_back(L"g");

		_measure = L"";
		_fromUnit = _units[0];
		_toUnit = _units[0];
		_question = BigDecimal();
		_solution = BigDecimal();
		_attempts = 0;
	}

#pragma endregion

#pragma region Session handling

	void Session::prepareNewConversion() {
		unsigned seed = (unsigned)chrono::system_clock::now().time_since_epoch().count();
		default_random_engine  generator(seed);

		uniform_int_distribution<int> measureDistribution(0, _measures.size() - 1);
		uniform_int_distribution<int> unitDistribution(0, _units.size() - 1);
		uniform_int_distribution<int> valueDistribution(1, 1000);

		// Prepare provisional from and to unit indexes, then make sure the two are not equal.
		int fromIndex = unitDistribution(generator);
		int toIndex = fromIndex;
		while (toIndex == fromIndex) {
			toIndex = unitDistribution(generator);
		}

		_measure = _measures[measureDistribution(generator)];
		_question = valueDistribution(generator);
		_fromUnit = _units[fromIndex];
		_toUnit = _units[toIndex];

		auto standard = _fromUnit.toStandard(_question);
		_solution = _toUnit.fromStandard(standard);

		_attempts = 0;
	}

	void Session::printQuestion(Console& console) {
		wstring_convert<codecvt_utf8<wchar_t>> converter;
		auto ws = converter.from_bytes(_question.toString());

		cout << endl << endl;
		cout << " Vnesi rezultat (x za izhod)" << endl;

		wcout << " " << ws << " " << _fromUnit.name() << _measure << " -> " << _toUnit.name() << _measure << " = ";
	}

	void Session::printSolution(Console& console) {
		wstring_convert<codecvt_utf8<wchar_t>> converter;
		auto value = converter.from_bytes(_solution.toString());

		cout << endl;
		wcout << " Resitev je: " << value << " " << _toUnit.name() << _measure << endl;
	}

	Result Session::askForSolution(Console& console) {
		_attempts++;

		string reply;
		cin >> reply;
		if (reply == "x") return Result::exit;

		BigDecimal value = BigDecimal(reply);
		if (value == _solution) {
			return Result::match;
		}

		return Result::wrong;
	}

	int Session::attempts() {
		return _attempts;
	}

#pragma endregion

#pragma region Getters & Setters

	vector<Unit> Session::units() {
		return _units;
	}

#pragma endregion
}
