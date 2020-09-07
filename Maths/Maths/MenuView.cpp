#include "UnitView.h"
#include "HangmanView.h"

#include "MenuView.h"

namespace Menu {

	constexpr short TITLE_COLOR = 1;
	constexpr short MENU_COLOR = 2;
	constexpr auto MENU_LEFT = 25;

#pragma region Overrides

	void MenuView::onDraw(const Screen &screen) {
		screen.registerColors(TITLE_COLOR, COLOR_YELLOW, COLOR_BLACK);
		screen.registerColors(MENU_COLOR, COLOR_WHITE, COLOR_BLACK);

		screen.draw([this](const Screen &screen) {
			screen.attributed(A_REVERSE | COLOR_PAIR(TITLE_COLOR), [](const Screen &screen) {
				mvprintstdwc(2, L" ┌┐┌┐┌──┐─┬─┌──┌┐┌┐┌──┐─┬─ ┬ ┬┌  ┌──┐ ");
				mvprintstdwc(3, L" │└┘│├──┤ │ ├─ │└┘│├──┤ │  │ ├┘┐ ├──┤ ");
				mvprintstdwc(4, L" ┴  ┴┴  ┴ ┴ └──┴  ┴┴  ┴ ┴  ┴ ┴ ┴ ┴  ┴ ");
			});

			drawOption(screen, 10, "A", "Pretvorba enot");
			drawOption(screen, 12, "S", "Nekaj za sprostitev");
			drawOption(screen, 14, "X", "Izhod");
		});
	}

	bool MenuView::onHandle() {
		switch (getch()) {
			case 'a': case 'A':
				switchView([]() { UnitConversion::UnitView().run(); });
				break;

			case 's': case 'S':
				switchView([]() { Hangman::HangmanView().run();  });
				break;

			case 'x': case 'X':
				return false;
		}

		// Stay in the loop.
		return true;
	}

#pragma endregion

#pragma region Helpers

	void MenuView::drawOption(const Screen &screen, int line, string key, string description) const {
		screen.attributed(A_BOLD | A_REVERSE | COLOR_PAIR(MENU_COLOR), [&](const Screen &screen) {
			mvprintw(line, MENU_LEFT, " %s ", key.c_str());
		});

		mvprintstd(line, MENU_LEFT + 5, description);
	}

#pragma endregion
}
