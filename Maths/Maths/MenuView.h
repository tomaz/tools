#pragma once

#include <iostream>
#include "View.h"

namespace Menu {

	using namespace std;

	/// Manages menu view.
	class MenuView : public View {
	public:
		using View::View;

	protected:
		void onDraw(const Screen &screen) override;
		bool onHandle() override;

	private:
		void drawOption(const Screen &screen, int line, string key, string description) const;
	};

}
