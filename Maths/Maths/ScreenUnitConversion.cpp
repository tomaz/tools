//
// Created by Tomaz on 31. 03. 20.
//

#include <conio.h>
#include <iomanip>
#include "ScreenUnitConversion.h"

#pragma region Overrides

bool ScreenUnitConversion::onRun(Console &console) {
	// Prepare for new session.
	_session.prepareNewConversion();

	while (true) {
		console.screen().title(" P R E T V O R B E   E N O T ");

		// Print out the legend.
		for (auto unit : _session.units()) {
			if (unit.name().empty()) continue;
			printUnit(console, unit);
		}

		// Print out the question.
		_session.printQuestion(console);

		// Ask for solution.
		Result result = _session.askForSolution(console);
		switch (result) {
			case Result::exit: {
				// If user bails out, print correct one, then return false to exit the run loop for this screen.
				_session.printSolution(console);
				cout << " Pritisni katerokoli tipko za izhod";
				_getch();
				return false;
			}

			case Result::wrong: {
				// Answer was wrong. Let user know, and ask again.
				cout << endl << " " << attr(" NAPACNO! ").color(216) << " (Pritisni katerokoli tipko za ponoven poizkus)" << endl;
				_getch();
				break;
			}

			case Result::match: {
				// Answer was correct; prepare new session and proceed.
				cout << endl << " " << attr(" BRAVO! ").color(152) << " Ugotovil si v " << _session.attempts();
				
				switch (_session.attempts()) {
					case 1: cout << " poizkusu!" << endl; break;
					default: cout << " poizkusih!" << endl; break;
				}
				
				cout << " (Pritisni katerokoli tipko za novo enacbo)" << endl;
				
				_session.prepareNewConversion();
				_getch();
				break;
			}
		}
	}

	return false;
}

#pragma endregion

#pragma region Helpers

void ScreenUnitConversion::printUnit(Console &console, Unit &unit) {
	wcout << " " << left << setw(5) << unit.name();
	cout << " " << attr(unit.scale().toString()).dimmed() << endl;
}

#pragma endregion