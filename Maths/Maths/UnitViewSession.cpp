#include <codecvt>
#include <random>
#include <chrono>

#include "UnitViewSession.h"

namespace UnitConversion {

	using namespace std;

#pragma region Initialization & Disposal

	Session::Session() {
		_units = {
			Unit(L"M", BigDecimal("1000000")),
			Unit(L"k", BigDecimal("1000")),
			Unit(L"h", BigDecimal("100")),
			Unit(L"da", BigDecimal("10")),
			Unit(L"", BigDecimal("1")),
			Unit(L"d", BigDecimal("0.1")),
			Unit(L"c", BigDecimal("0.01")),
			Unit(L"m", BigDecimal("0.001")),
			Unit(L"µ", BigDecimal("0.000001")),
		};

		_measures = {
			L"m",
			L"l",
			L"g",
		};
	}

#pragma endregion

#pragma region Public

	Equation Session::prepareNewEquation() {
		_equationsCount++;

		unsigned seed = (unsigned)chrono::system_clock::now().time_since_epoch().count();
		default_random_engine generator(seed);

		// Prepare distributions for randomizing.
		uniform_int_distribution<int> measureDistribution(0, _measures.size() - 1);
		uniform_int_distribution<int> unitDistribution(0, _units.size() - 1);
		uniform_int_distribution<int> valueDistribution(1, 1000);

		// Prepare provisional from and to unit indexes, then make sure the two are not equal.
		int fromIndex = unitDistribution(generator);
		int toIndex = fromIndex;
		while (toIndex == fromIndex) {
			toIndex = unitDistribution(generator);
		}

		// Now we can prepare the equation.
		auto measure = _measures[measureDistribution(generator)];
		auto value = valueDistribution(generator);
		auto fromUnit = _units[fromIndex];
		auto toUnit = _units[toIndex];
		return Equation(measure, fromUnit, toUnit, value);
	}

	void Session::registerCorrectAnswer() {
		_correctAnswers++;
	}

#pragma endregion

#pragma region Getters & Setters

	std::vector<Unit> Session::units() {
		return _units;
	}

	int Session::equationsCount() {
		return _equationsCount;
	}

	int Session::correctAnswers() {
		return _correctAnswers;
	}

#pragma endregion

#pragma region Equation

	Equation::Equation(std::wstring measure, Unit from, Unit to, BigDecimal value) {
		auto standard = from.toStandard(value);
		_solution = to.fromStandard(standard);
		_value = value;
		_measure = measure;
		_fromUnit = from;
		_toUnit = to;
		resetInput();
	}

	void Equation::appendInput(const char& value) {
		_input += value;
	}

	void Equation::deleteLastInputChar() {
		if (_input.empty()) return;
		_input.pop_back();
	}

	void Equation::resetInput() {
		_inputCounter = 0;
		_input = "";
	}

	void Equation::incrementInput() {
		_inputCounter++;
	}

	bool Equation::isInputCorrect() {
		BigDecimal inputValue(_input);
		return inputValue == _solution;
	}

	std::string Equation::input() {
		return _input;
	}

	int Equation::inputCounter() {
		return _inputCounter;
	}

	std::wstring Equation::measure() {
		return _measure;
	}

	BigDecimal Equation::value() {
		return _value;
	}

	BigDecimal Equation::solution() {
		return _solution;
	}

	Unit Equation::fromUnit() {
		return _fromUnit;
	}

	Unit Equation::toUnit() {
		return _toUnit;
	}

#pragma endregion
}
