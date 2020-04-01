//
// Created by Tomaz on 31. 03. 20.
//

#include "ScreenUnitConversion.h"
#include "ScreenMenu.h"

#pragma region Screen handling

bool ScreenMenu::onRun(Console &console) {
	int option = console.screen()
		.title(" M A T E M A T I K A ")
		.option("A", "Pretvarjanje enot")
		.option("X", "Izhod")
		.wait();

	bool result = true;

	switch (option) {
		case 0:
			ScreenUnitConversion(console).run();
			break;

		default:
			result = false;
			break;
	}

	return result;
}

#pragma endregion