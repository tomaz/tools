using Kids.Common;
using Konsole;
using System;
using System.Drawing;
using System.Text;
using static System.ConsoleColor;

namespace Kids.Modules.Hangman {
	class Game {
		
		/// <summary>
		/// Phrase the user is currently guessing.
		/// </summary>
		public string GuessingPhrase { get; private set; }

		/// <summary>
		/// Current game <see cref="Difficulty"/>.
		/// </summary>
		public DifficultyType Difficulty { get; private set; }

		/// <summary>
		/// Current game <see cref="Status"/>.
		/// </summary>
		public StatusType Status { get; private set; }

		private string _guessedChars;
		private string _guessedPhrase;

		private int _playerCurrentStep;
		private readonly IConsole _console;
		private readonly HangmanDrawing _hangmanDrawing;
		private Point _position;

		#region Initialization & Disposal

		Game(string word, int x, int y, IConsole console, DifficultyType difficulty) {
			GuessingPhrase = word.ToUpper();
			Difficulty = difficulty;
			Status = StatusType.Playing;

			_guessedChars = "";
			_guessedPhrase = "";
			_position = new Point(x, y);
			_console = console;

			_hangmanDrawing = PrepareHangmanDrawing(difficulty);
			_playerCurrentStep = 0;

			UpdateGuessedPhrase();
		}

		#endregion

		#region Public

		public void AddGuess(char guess) {
			var upper = guess.ToString().ToUpper();

			if (_guessedChars.Contains(upper)) return;

			_guessedChars += upper;

			UpdateGuessedPhrase();
			UpdateStatus();
		}

		public void Draw() {
			var x = _position.X;
			var y = _position.Y;

			_console.ForegroundColor = DarkGray;
			_console.PrintAt(x, y++, "═╦═════════");
			_console.PrintAt(x, y++, " ║");
			_console.PrintAt(x, y++, " ║");
			_console.PrintAt(x, y++, " ║");
			_console.PrintAt(x, y++, " ║");
			_console.PrintAt(x, y++, " ║");
			_console.PrintAt(x, y++, " ║");
			_console.PrintAt(x, y++, "═╩═════════");

			DrawHangman();

			_console.ForegroundColor = DarkGray;
			y++; _console.Print(x, y++, _guessedPhrase, Gray);
			y++; _console.Print(x, y++, _guessedChars, White);

			Console.CursorLeft = _console.AbsoluteX + _console.CursorLeft + 3;
			Console.CursorTop = _console.AbsoluteY + _console.CursorTop + 3;
		}

		#endregion

		#region Helpers

		private void DrawHangman() {
			_console.ForegroundColor = Color();
			_hangmanDrawing.Draw(_playerCurrentStep);

			ConsoleColor Color() {
				return Status switch
				{
					StatusType.Playing => White,
					StatusType.Lost => Red,
					StatusType.Won => Green,
					_ => throw new NotImplementedException(),
				};
			}
		}

		private HangmanDrawing PrepareHangmanDrawing(DifficultyType difficulty) {
			var x = _position.X + 5;
			var y = _position.Y + 1;

			return difficulty switch {
				DifficultyType.Easy => new EasyHangmanDrawing(_console, x, y),
				DifficultyType.Medium => new MediumHangmanDrawing(_console, x, y),
				DifficultyType.Hard => new HardHangmanDrawing(_console, x, y),
				_ => throw new NotImplementedException(),
			};
		}

		private void UpdateGuessedPhrase() {
			var builder = new StringBuilder();

			foreach (var ch in GuessingPhrase) {
				if (_guessedChars.Contains(ch)) {
					builder.Append(ch);
				} else {
					builder.Append("_");
				}
			}

			var newPhrase = builder.ToString();
			if (newPhrase == _guessedPhrase) {
				_playerCurrentStep++;
			}

			_guessedPhrase = newPhrase;
		}

		private void UpdateStatus() {
			if (!_guessedPhrase.Contains("_")) {
				Status = StatusType.Won;
			} else if (_hangmanDrawing.IsGameOver(_playerCurrentStep)) {
				Status = StatusType.Lost;
			} else {
				Status = StatusType.Playing;
			}
		}

		#endregion

		#region Declarations

		public enum StatusType {
			Playing,
			Lost,
			Won
		}

		public enum DifficultyType {
			Easy,
			Medium,
			Hard
		}

		public class Builder {

			private string? _phrase = null;
			private DifficultyType _difficulty = DifficultyType.Easy;

			private IConsole? _console = null;
			private int _x = 0;
			private int _y = 0;

			public Builder Phrase(string value) {
				_phrase = value;
				return this;
			}

			public Builder Difficulty(DifficultyType value) {
				_difficulty = value;
				return this;
			}

			public Builder Difficulty(int value) {
				return Difficulty(value switch {
					0 => DifficultyType.Easy,
					1 => DifficultyType.Medium,
					_ => DifficultyType.Hard
				});
			}

			public Builder Console(IConsole value, int? x = null, int? y = null) {
				_console = value;
				if (x.HasValue) X(x.Value);
				if (y.HasValue) Y(y.Value);
				return this;
			}

			public Builder X(int value) {
				_x = value;
				return this;
			}

			public Builder Y(int value) {
				_y = value;
				return this;
			}

			public Builder Position(int x, int y) {
				return X(x).Y(y);
			}

			public Game Game() {
				return new Game(_phrase!, _x, _y, _console!, _difficulty);
			}
		}

		#endregion
	}
}
