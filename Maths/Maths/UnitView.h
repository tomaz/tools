#pragma once

#include <vector>

#include "UnitViewUnit.h"
#include "UnitViewSession.h"
#include "View.h"

namespace UnitConversion {

	namespace Values {

		enum class Option {
			ENTER_EQUATION,
			SHOW_RESULT,
			EXIT
		};

		enum class MenuResult {
			CONTINUE_WITH_EQUATION,
			RESULT_VALID,
			RESULT_WRONG,
			SHOW_RESULT,
			EXIT
		};
	}

	class UnitView : public View {
	public:
		using View::View;

	protected:
		void onDraw(const Screen &screen) override;
		bool onHandle() override;

	private:
		Values::MenuResult handleMenu(Equation &equation);
		bool handleResult(Values::MenuResult result, Equation &equation);

		void drawUnits(const Screen &screen);
		void drawMenu(const Screen &screen, Equation &equation);
		void drawEquation(const Screen &screen, Equation &equation);

	private:
		Values::Option _menuOption;
		Session _session;
		int _equationX;
		int _menuX;
		int _menuY;
	};

}

