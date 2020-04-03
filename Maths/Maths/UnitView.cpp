#include "UnitView.h"

namespace UnitConversion {

	using namespace std;
	using namespace Values;

	enum ColorIndexes {
		TITLE_COLOR = 1,
		MENU_COLOR,
		INPUT_COLOR,
		CORRECT_RESULT_COLOR,
		WRONG_RESULT_COLOR,
		STATISTICS_COLOR,
	};

	constexpr auto LEGEND_TOP = 4;
	constexpr auto LEGEND_LEFT = 3;

#pragma region Overrides

	void UnitView::onDraw(const Screen &screen) {
		init_color(COLOR_MAGENTA, 700, 700, 700);

		screen.registerColors(TITLE_COLOR, COLOR_YELLOW, COLOR_BLACK);
		screen.registerColors(MENU_COLOR, COLOR_WHITE, COLOR_BLACK);
		screen.registerColors(INPUT_COLOR, COLOR_CYAN, COLOR_BLACK);
		screen.registerColors(CORRECT_RESULT_COLOR, COLOR_GREEN, COLOR_BLACK);
		screen.registerColors(WRONG_RESULT_COLOR, COLOR_RED, COLOR_BLACK);
		screen.registerColors(STATISTICS_COLOR, COLOR_MAGENTA, COLOR_BLACK);

		screen.draw([this](const Screen &screen) {
			screen.attributed(A_REVERSE | COLOR_PAIR(TITLE_COLOR), [](const Screen &screen) {
				mvprintstdc(2, " P R E T V O R B A   E N O T ");
			});

			drawUnits(screen);
		});
	}

	bool UnitView::onHandle() {
		bool result = true;

		auto shouldStayInLoop = [](MenuResult status) {
			switch (status) {
				case MenuResult::CONTINUE_WITH_EQUATION:
				case MenuResult::RESULT_WRONG:
					return true;
				default:
					return false;
			}
		};

		// We treat each iteration as one equation and we perform our own loop until user exits.
		auto equation = _session.prepareNewEquation();

		// Setup various PDCurses settings we need.
		cbreak();				// Don't wait for enter, pass everything to us.
		keypad(stdscr, true);	// Enable arrow and function keys.

		// Run equation solving loop. Each iteration waits for user input, validates and handles the answer.
		MenuResult status = MenuResult::CONTINUE_WITH_EQUATION;
		while (shouldStayInLoop(status)) {
			// Draw the menu.
			draw([&](const Screen &screen) { drawMenu(screen, equation); });

			// Handle user selection and input. If true is returned, we must stay in input loop, otherwise we should exit.
			status = handleMenu(equation);
			result = handleResult(status, equation);
		}

		// After we're done with equation, exit the loop notifying parent whether to conitnue or not.
		return result;
	}

#pragma endregion

#pragma region Helpers

	MenuResult UnitView::handleMenu(Equation &equation) {
#define ADD(delta) Option(static_cast<int>(_menuOption) + delta)

		MenuResult result = MenuResult::CONTINUE_WITH_EQUATION;

		auto ch = getch();

		switch (ch) {
			case KEY_UP:
				if (_menuOption > Option::ENTER_EQUATION) _menuOption = ADD(-1);
				break;

			case KEY_DOWN:
				if (_menuOption < Option::EXIT) _menuOption = ADD(1);
				break;

			case KEY_ENTER:
			case '\n':
				// When enter is pressed, we need to handle it based on which menu option is currently selected.
				equation.incrementInput();

				switch (_menuOption) {
					case Option::ENTER_EQUATION:
						if (equation.isInputCorrect()) {
							result = MenuResult::RESULT_VALID;
						} else {
							result = MenuResult::RESULT_WRONG;
						}
						break;

					case Option::SHOW_RESULT:
						result = MenuResult::SHOW_RESULT;
						break;

					case Option::EXIT:
						result = MenuResult::EXIT;
						break;
				}
				break;

			case KEY_BACKSPACE:
			case 127:
			case '\b':
				// https://stackoverflow.com/a/54303430/1076949
				if (_menuOption != Option::ENTER_EQUATION) break;
				equation.deleteLastInputChar();
				break;

			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				if (_menuOption != Option::ENTER_EQUATION) break;
				equation.appendInput(ch);
				break;

			case '.':
			case ',':
				if (_menuOption != Option::ENTER_EQUATION) break;
				equation.appendInput('.');
				break;
		}

		return result;
	}

	bool UnitView::handleResult(MenuResult result, Equation &equation) {
		// The main purpose of this function is to reduce complexity in `onHandle`; we may perform quite a bit of juggling on some of the options.
		switch (result) {
			case MenuResult::CONTINUE_WITH_EQUATION:
				// Nothing to do, we should just stay in menu loop and wait next input.
				break;

			case MenuResult::RESULT_VALID:
				// Increase correct answers count.
				_session.registerCorrectAnswer();

				// User has entered correct result, congratulate them. Note that we can simply use current cursor position as this can only occur when user presses ented on input line.
				restorePosition([&](const Screen &screen) {
					screen.enableCursor(false);

					printstd(" ");

					screen.attributed(A_BOLD | A_REVERSE | COLOR_PAIR(CORRECT_RESULT_COLOR), [&](const Screen &screen) {
						printstd(" BRAVO! ");
					});

					if (equation.inputCounter() == 1) {
						printw(" Ugotovil si v prvem poizkusu!");
					} else {
						printw(" Ugotovil si v %d poizkusih", equation.inputCounter());
					}
				});

				// Wait key, then erase printed text.
				getch();
				printeol(" ");

				break;

			case MenuResult::RESULT_WRONG:
				// User has entered wrong result, congratulate them. Note that we can simply use current cursor position as this can only occur when user presses ented on input line.
				restorePosition([&](const Screen &screen) {
					screen.enableCursor(false);

					printstd(" ");

					screen.attributed(A_BOLD | A_REVERSE | COLOR_PAIR(WRONG_RESULT_COLOR), [&](const Screen &screen) {
						printstd(" Napaka! ");
					});
				});

				// Wait key, then erase printed text.
				getch();
				printeol(" ");

				break;

			case MenuResult::SHOW_RESULT:
				// If user wants to display result, assume they want to quit; show the result and continue with a different equation.
				restorePosition([&](const Screen &screen) {
					screen.attributed(A_REVERSE | COLOR_PAIR(INPUT_COLOR), [&](const Screen &screen) {
						printstd(equation.solution().toString());
						printstd(" ");
						printstdw(equation.toUnit().name());
						printstdw(equation.measure());
					});
				});

				// Wait key, then erase printed text.
				getch();
				printeol(" ");

				// Also, as convenience, auto select equation.
				_menuOption = Option::ENTER_EQUATION;

				break;

			case MenuResult::EXIT:
				// On exit we should return false so we exit the menu loop.
				return false;
		}

		// In all other cases return true to stay in the loop.
		return true;
	}

	void UnitView::drawUnits(const Screen &screen) {
		int x = LEGEND_LEFT, y = LEGEND_TOP;

		mvprintstdw(y, x, L"┌────────────────────────┐");
		y++;

		for (auto unit : _session.units()) {
			if (unit.isStandard()) continue;

			mvprintstdw(y, x, L"│                        │");

			screen.attributed(A_BOLD, [&](const Screen &screen) {
				mvprintstdw(y, x + 3, unit.name());
			});

			mvprintstd(y, x + 7, unit.scale().toString());

			y++;
		}

		mvprintstdw(y, x, L"└────────────────────────┘");

		// Prepare coordinates for printing out equation.
		_menuX = LEGEND_LEFT;
		_menuY = screen.currentY() + 3;
	}

	void UnitView::drawMenu(const Screen &screen, Equation &equation) {
		int x = _menuX + 5;
		int y = _menuY;

		auto optionY = [&](Option option) {
			switch (option) {
				case Option::ENTER_EQUATION: return _menuY + 4;
				case Option::SHOW_RESULT: return _menuY + 6;
				case Option::EXIT: return _menuY + 7;
			}
			return 0;
		};

		// Draw the title.
		mvprintstdw(y++, _menuX, L"↓↑ Premikanje med moznostmi");
		y++;

		mvprintstd(y++, _menuX, "--------------------------");
		y++;

		// Draw the equation.
		move(y++, x);
		drawEquation(screen, equation);
		y++;

		// Draw menu options.
		mvprintstd(y++, x, "Prikazi odgovor");
		mvprintstd(y++, x, "Izhod");

		// Clear menu selection to avoid artifacts.
		mvprintstd(optionY(Option::ENTER_EQUATION), _menuX, "   ");
		mvprintstd(optionY(Option::SHOW_RESULT), _menuX, "   ");
		mvprintstd(optionY(Option::EXIT), _menuX, "   ");

		// Draw selected option.
		screen.attributed(A_BOLD | A_REVERSE, [&](const Screen &screen) {
			mvprintstdw(optionY(_menuOption), _menuX, L" → ");
		});

		// Draw statistics.
		screen.attributed(COLOR_PAIR(STATISTICS_COLOR), [&](const Screen &screen) {
			int equations = _session.equationsCount() - 1;
			int correct = _session.correctAnswers();
			int y = optionY(Option::EXIT) + 1;

			mvprintstd(y + 1, _menuX, "---------------------------");

			mvprintstd(y + 3, _menuX, "Enacb:     ");
			printw("%d", equations);

			mvprintstd(y + 4, _menuX, "Pravilnih: ");
			printw("%d", correct);
			if (equations > 0) {
				printw(" (%d%%)", (int)(100.0 * (double)correct / (double)equations));
			}
		});

		// Update cursor based on selected option. For equation move to the end, otherwise to fixed position after the option text.
		if (_menuOption == Option::ENTER_EQUATION) {
			move(optionY(Option::ENTER_EQUATION), _equationX);
			screen.enableCursor();
		} else {
			move(optionY(_menuOption), 24);
			screen.enableCursor(false);
		}
	}

	void UnitView::drawEquation(const Screen &screen, Equation &equation) {
		// Print the number to convert emphasized.
		screen.attributed(A_BOLD, [&](const Screen &screen) {
			printstd(equation.value().toString());
		});

		// Followed by unit and measure (Mg).
		printstd(" ");
		printstdw(equation.fromUnit().name());
		printstdw(equation.measure());

		// Then conversion sign.
		printstdw(L" → ");

		// And unit to convert to and measure.
		printstdw(equation.toUnit().name());
		printstdw(equation.measure());

		// Also an indication for user to enter the value.
		printstd(" = ");

		// And finally the actual value user has entered so far.
		screen.attributed(A_REVERSE | COLOR_PAIR(INPUT_COLOR), [&](const Screen &screen) {
			printstd(equation.input());
		});

		// Remember current equation X coordinate so we can position cursor to it's place.
		_equationX = screen.currentX();

		// Now clear the rest of the space after current equation to prevent artifacts.
		printeol(" ");
	}

#pragma endregion
}
