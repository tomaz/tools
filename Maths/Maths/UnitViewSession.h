#pragma once

#include <iostream>
#include <vector>

#include "UnitViewUnit.h"

namespace UnitConversion {

	class Equation;

	class Session {
	public:
		Session();

	public:
		Equation prepareNewEquation();
		void registerCorrectAnswer();

	public:
		std::vector<Unit> units();
		int equationsCount();
		int correctAnswers();

	private:
		std::vector<Unit> _units;
		std::vector<std::wstring> _measures;
		int _equationsCount;
		int _correctAnswers;
	};

	class Equation {
	public:
		Equation(std::wstring measure, Unit from, Unit to, BigDecimal value);

	public:
		void appendInput(const char& value);
		void deleteLastInputChar();
		void resetInput();
		void incrementInput();
		bool isInputCorrect();
		
		std::string input();
		int inputCounter();

	public:
		std::wstring measure();
		BigDecimal value();
		BigDecimal solution();
		Unit fromUnit();
		Unit toUnit();

	private:
		std::string _input;
		std::wstring _measure;
		BigDecimal _value;
		BigDecimal _solution;
		Unit _fromUnit;
		Unit _toUnit;
		int _inputCounter;
	};

}
