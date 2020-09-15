﻿using System;
using Kids.Blocks;
using Kids.Common;
using Kids.Modules;
using Kids.Modules.Hangman;
using Kids.Modules.UnitConversion;
using static System.ConsoleColor;

namespace Kids.MainMenu {

	/// <summary>
	/// Runs the main menu.
	/// </summary>
	class MainMenuModule : BaseModule {

		private MenuHandler _menu;

		#region Initialization & Disposal

		public MainMenuModule() : base() {
			_menu = CreateMenu();
		}

		#endregion

		#region Overrides

		protected override void OnDraw() {
			View.TitleConsole.PrintAtCentered(0, " ┌┐┌┐┌──┐─┬─┌──┌┐┌┐┌──┐─┬─ ┬ ┬┌  ┌──┐ ", Black, DarkYellow);
			View.TitleConsole.PrintAtCentered(1, " │└┘│├──┤ │ ├─ │└┘│├──┤ │  │ ├┘┐ ├──┤ ", Black, DarkYellow);
			View.TitleConsole.PrintAtCentered(2, " ┴  ┴┴  ┴ ┴ └──┴  ┴┴  ┴ ┴  ┴ ┴ ┴ ┴  ┴ ", Black, DarkYellow);

			_menu.Draw();
		}

		protected override void OnRun() {
			_menu.Run();
		}

		#endregion

		#region Setup

		private MenuHandler CreateMenu() {
			return new MenuHandler(View.ContentConsole)
				.SetPosition(10, 5)
				.Add("Pretvorba enot", (IMenu menu) => { Push(new UnitConversionModule()); })
				.Add("Vislice", (IMenu menu) => { Push(new HangmanModule());  })
				.Add("Izhod", (IMenu menu) => { menu.Exit(); });
		}

		#endregion
	}
}