using Kids.Blocks;
using Konsole;
using System;
using System.Collections.Generic;
using System.Drawing;
using static System.ConsoleColor;

namespace Kids.Modules.UnitConversion {
	class UnitConversionModule : BaseModule {

		private readonly List<Unit> _units;
		private readonly List<string> _measures;
		private readonly MenuHandler _menu;
		private readonly IConsole _contentConsole;
		private readonly IConsole _legendConsole;

		private Equation _equation;
		private int _attempts = 0;
		private int _guesses = 0;

		#region Initialization & Disposal

		public UnitConversionModule() : base() {
			_units = CreateUnits();
			_measures = CreateMeasures();

			var splits = View.ContentConsole.SplitColumns(new Split(3), new Split(0), new Split(30), new Split(3));
			_contentConsole = splits[1];
			_legendConsole = splits[2];

			_menu = CreateMenu();
			PrepareNextEquation(false);
		}

		#endregion

		#region Overrides

		protected override void OnDraw() {
			View.SetTitle(" P R E T V O R B A   E N O T ");

			_contentConsole.Clear();
			_legendConsole.Clear();

			_menu.Draw();

			DrawEquation();
			DrawStatistics();
			DrawLegend();
		}

		protected override void OnRun() {
			_menu.Run();
		}

		#endregion

		#region Setup

		private MenuHandler CreateMenu() {
			return new MenuHandler(_contentConsole)
				.SetPosition(0, 1)
				.Add(new MenuHandler.Item() {
					Title = "",
					OnSelected = (IMenu menu) => { UpdateInputStatus(true); },
					OnUnselected = (IMenu menu) => { UpdateInputStatus(false); },
					OnActivated = (IMenu menu) => { ValidateSolution(); },
					OnKeyPress = (IMenu menu, ConsoleKeyInfo key) => { InterpretKey(key); }
				})
				.Add("Prikazi odgovor", (IMenu menu) => {
					DrawEquation(true);
					DrawMessage(" Pritisni tipko za nadaljevanje ");
					Console.ReadKey();
					PrepareNextEquation();
				})
				.Add("Preskoci enacbo", (IMenu menu) => {
					menu.SelectItem(0);
					PrepareNextEquation();
				})
				.Add("Izhod", (IMenu menu) => { 
					menu.Exit();
				});
		}

		private List<Unit> CreateUnits() {
			return new List<Unit>(new Unit[] {
				new Unit("M", 1000000m),
				new Unit("k", 1000m),
				new Unit("h", 100m),
				new Unit("da", 10m),
				new Unit("", 1m),
				new Unit("d", 0.1m),
				new Unit("c", 0.01m),
				new Unit("m", 0.001m),
				new Unit("µ", 0.000001m),
			});
		}

		private List<string> CreateMeasures() {
			return new List<string>(new string[] { "m", "l", "g" });
		}

		#endregion

		#region Drawing

		private void DrawLegend() {
			_legendConsole.ForegroundColor = DarkGray;

			// Draw all units.
			var position = new Point(3, 1);
			_units.ForEach((Unit unit) => {
				if (unit.Name.Length == 0) return;
				_legendConsole.PrintAtColor(Gray, position.X, position.Y, unit.Name, null);
				_legendConsole.PrintAt(position.X + 5, position.Y, unit.Scale.ToString());
				position.Y++;
			});

			// Draw a box in dark gray.
			new Draw(_legendConsole).Box(0, 0, _legendConsole.WindowWidth - 1, _units.Count);
		}

		private void DrawEquation(bool drawSolution = false) {
			// First set cursor position for write.
			SetCursorPosition(CursorPosition.Equation);

			// Write from value followed by unit and measure.
			View.ContentConsole.Write(White, _equation.FromValue.ToString());
			View.ContentConsole.Write(DarkGray, _equation.FromUnit.Name);
			View.ContentConsole.Write(DarkGray, _equation.Measure);

			// Write equal sign.
			View.ContentConsole.Write(DarkGray, " = ");

			// If requested so, draw the solution, otherwise setup for user input.
			if (drawSolution) {
				View.ContentConsole.Write(White, _equation.ToValue.ToString());
				View.ContentConsole.Write(DarkGray, _equation.ToUnit.Name);
				View.ContentConsole.Write(DarkGray, _equation.Measure);
			} else {
				// Write current user input.
				View.ContentConsole.Write(White, _equation.Input);

				// We should show cursor at current coordinate, so update its position.
				Console.CursorLeft = View.ContentConsole.CursorLeft;
				Console.CursorTop = View.ContentConsole.CursorTop + View.ContentConsole.AbsoluteY;

				// We should insert an empty space when user is typing to accomodate cursor.
				if (Console.CursorVisible) View.ContentConsole.Write(" ");

				// At the end we should write to unit and measure.
				View.ContentConsole.Write(DarkGray, _equation.ToUnit.Name);
				View.ContentConsole.Write(DarkGray, _equation.Measure);
			}

			// This part takes care of clearing when deleting chars.
			View.ContentConsole.Write("                    ");
		}

		private void DrawStatistics() {
			var attempsStrings = new string[] { "enacba", "enacbi", "enacbe", "enacb" };
			var guessesStrings = new string[] { "pravilna", "pravilni", "pravilne", "pravilnih" };

			SetCursorPosition(CursorPosition.Statistics);

			View.ContentConsole.Write(White, _attempts.ToString());
			View.ContentConsole.Write(" ");
			View.ContentConsole.Write(DarkGray, Plural(_attempts, attempsStrings));

			View.ContentConsole.Write(DarkGray, ", ");

			View.ContentConsole.Write(White, _guesses.ToString());
			View.ContentConsole.Write(" ");
			View.ContentConsole.Write(DarkGray, Plural(_guesses, guessesStrings));

			if (_attempts > 0) {
				var percent = _guesses * 100 / _attempts;
				View.ContentConsole.Write(DarkGray, " (");
				View.ContentConsole.Write(White, $"{percent}%");
				View.ContentConsole.Write(DarkGray, ")");
			}

			View.ContentConsole.Write("                              ");

			static string Plural(int count, string[] strings) => count switch
			{
				0 => strings[3],
				1 => strings[0],
				2 => strings[1],
				3 => strings[2],
				4 => strings[2],
				_ => strings[3]
			};
		}

		private void DrawError() {
			SetCursorPosition(CursorPosition.Statistics);

			View.ContentConsole.BackgroundColor = Red;
			View.ContentConsole.Write(Black, " NAPAKA... ");
			View.ContentConsole.BackgroundColor = Black;

			// This is to make sure the whole statistics line is deleted.
			View.ContentConsole.Write("                              ");
		}

		private void DrawSuccess() {
			SetCursorPosition(CursorPosition.Statistics);

			View.ContentConsole.BackgroundColor = Green;
			View.ContentConsole.Write(Black, " BRAVO ");

			View.ContentConsole.BackgroundColor = Black;
			View.ContentConsole.Write(" ");
			View.ContentConsole.Write(Green, "Uganil si v ");

			switch (_equation.Attempts) {
				case 1:
					View.ContentConsole.Write(Green, "prvem poizkusu!");
					break;
				default:
					View.ContentConsole.Write(Green, $"{_equation.Attempts} poizkusih.");
					break;
			}

			// This is to make sure the whole statistics line is deleted.
			View.ContentConsole.Write("                    ");
		}

		private void DrawMessage(string message) {
			SetCursorPosition(CursorPosition.Statistics);

			View.ContentConsole.BackgroundColor = Red;
			View.ContentConsole.Write(Black, message);

			View.ContentConsole.BackgroundColor = Black;

			// This is to make sure the whole statistics line is deleted.
			View.ContentConsole.Write("                     ");
		}

		private void SetCursorPosition(CursorPosition position) {
			switch (position) {
				case CursorPosition.Equation:
					View.ContentConsole.CursorLeft = _menu.Position.X + 8;
					View.ContentConsole.CursorTop = _menu.Position.Y;
					break;

				case CursorPosition.Statistics:
					View.ContentConsole.CursorLeft = _menu.Position.X + 8;
					View.ContentConsole.CursorTop = _menu.Bottom + 3;
					break;
			}
		}

		#endregion

		#region Helpers

		private void ValidateSolution() {
			if (_equation.Input.Length == 0) return;

			var value = decimal.Parse(_equation.Input);

			// On each validation we increate attempt counter for this equation (purely for statistical purposes).
			_equation.Attempts++;

			// If solution was wrong, show error, wait for key, then stay in "guess" loop of the same equation.
			if (value != _equation.ToValue) {
				Console.Beep();
				DrawError();
				Console.ReadKey();
				DrawEquation();
				DrawStatistics();
				return;
			}

			// If all was fine, increase correct guesses counter, draw success message, wait for key, then prepare next equation.
			_guesses++;
			DrawSuccess();
			Console.ReadKey();
			PrepareNextEquation();
		}

		private void PrepareNextEquation(bool incrementAttempts = true) {
			if (incrementAttempts) _attempts++;

			_equation = new Equation(_measures, _units);

			_menu.Draw();
			DrawEquation();
			DrawStatistics();
		}

		private void UpdateInputStatus(bool active) {
			Console.CursorVisible = active;
			DrawEquation();
		}

		private void InterpretKey(ConsoleKeyInfo key) {
			switch (key.Key) {
				case ConsoleKey.D0:
				case ConsoleKey.D1:
				case ConsoleKey.D2:
				case ConsoleKey.D3:
				case ConsoleKey.D4:
				case ConsoleKey.D5:
				case ConsoleKey.D6:
				case ConsoleKey.D7:
				case ConsoleKey.D8:
				case ConsoleKey.D9:
					_equation.Input += key.KeyChar;
					DrawEquation();
					break;

				case ConsoleKey.OemComma:
				case ConsoleKey.OemPeriod:
					if (_equation.Input.Contains(',')) break;
					_equation.Input += ",";
					DrawEquation();
					break;

				case ConsoleKey.Backspace:
				case ConsoleKey.Delete:
					if (_equation.Input.Length == 0) break;
					_equation.Input = _equation.Input.Remove(_equation.Input.Length - 1);
					DrawEquation();
					break;
			}
		}

		#endregion

		#region Declarations

		private enum CursorPosition {
			Equation,
			Statistics
		}

		private class Equation {
			public string Measure { get; private set; }

			public Unit FromUnit { get; private set; }
			public Unit ToUnit { get; private set; }

			public decimal FromValue { get; private set; }
			public decimal ToValue { get; private set; }

			public string Input { get; set; }

			public int Attempts { get; set; }

			public Equation(List<string> measures, List<Unit> units) {
				var random = new Random();

				var fromIndex = random.Next(0, units.Count - 1);
				var toIndex = fromIndex;
				while (toIndex == fromIndex) {
					toIndex = random.Next(0, units.Count - 1);
				}

				Measure = measures[random.Next(0, measures.Count - 1)];

				FromUnit = units[fromIndex];
				FromValue = random.Next(1, 1000);

				ToUnit = units[toIndex];
				ToValue = ToUnit.FromStandard(FromUnit.ToStandard(FromValue));

				Input = "";
				Attempts = 0;
			}
		}

		#endregion
	}
}
