using Kids.Menu;
using Kids.Common;
using Konsole;
using System;
using static System.ConsoleColor;
using System.Threading;

namespace Kids.Modules.Hangman {
	public class HangmanModule : BaseModule {

		private readonly GameOptionsModule.Options _newGameOptions;
		private readonly Menu.Menu _menu;
		private readonly IConsole _contentConsole;
		private readonly IConsole _gallowsConsole;
		private readonly IConsole _menuConsole;

		private Game? _game;

		#region Initialization & Disposal

		public HangmanModule() : base() {
			var splits = View.ContentConsole.SplitColumns(new Split(3), new Split(0), new Split(3));
			_contentConsole = splits[1];

			_menuConsole = new Window(_contentConsole, 0, 1, 40, 20);

			_gallowsConsole = _contentConsole.OpenBox("", 25, 0, 50, 16, new BoxStyle() {
				ThickNess = LineThickNess.Single
			});

			_newGameOptions = new GameOptionsModule.Options();
			_menu = CreateMenu();
		}

		#endregion

		#region Overrides

		protected override void OnDraw() {
			View.SetTitle(" B E S E D E ");

			_menu.Draw();
			_game?.Draw();
		}

		protected override void OnRun() {
			Initialize();

			_menu.Run();
		}

		#endregion

		#region Setup

		private Menu.Menu CreateMenu() {
			return new MenuBuilder(_menuConsole)
				.Add(new Menu.MenuItem() {
					Title = "Igraj",
					OnSelected = (_) => { UpdateInputStatus(true); },
					OnUnselected = (_) => { UpdateInputStatus(false); },
					OnKeyPress = (IMenu menu, ConsoleKeyInfo key) => { InterpretKey(key); },
					NextItemOffset = 10
				})
				.Add("Nova Igra", (_) => { StartNewGame(); })
				.Add("Izhod", (IMenu menu) => { menu.Exit(); })
				.VerticalMenu();
		}

		#endregion

		#region State

		/// <summary>
		/// Loads all resources and initializes the game. This should only be called once, when user starts this module.
		/// </summary>
		private void Initialize() {
			LoadWithProgressIndicator();
			StartNewGame();

			void LoadWithProgressIndicator() {
				_newGameOptions.Dictionary.Initialize((double progress) => {
					_contentConsole.PrintAtCentered($"Nalagam {(progress * 100):0}%");
				});

				_contentConsole.Clear();

				Redraw();
			}
		}

		/// <summary>
		/// Presents game options and then starts a new game.
		/// </summary>
		private void StartNewGame() {
			ShowOptions();
			CreateNewGame();
			Redraw();

			void ShowOptions() {
				// Deactivate our menu while showing options so that only one selection arrow is displayed at any given time.
				_menu.IsActive = false;

				var optionsConsole = View.ContentConsole.OpenBoxCentered("Nova igra", 60, 13, new BoxStyle() {
					ThickNess = LineThickNess.Double,
					Body = new Colors(White, DarkBlue),
					Line = new Colors(DarkGray, DarkBlue),
					Title = new Colors(White, DarkBlue)
				});

				var options = new GameOptionsModule(optionsConsole, _newGameOptions);
				options.Run();

				_menu.IsActive = true;
			}

			void CreateNewGame() {
				_game = new Game.Builder()
					.Console(_gallowsConsole, 2, 1)
					.Phrase(_newGameOptions.Group.RandomWord)
					.Difficulty(_newGameOptions.Difficulty)
					.Game();

				_game.Draw();
			}
		}

		/// <summary>
		/// Presents game win screen and then starts a new game.
		/// </summary>
		private void ShowGameWin() {
			var parameters = new CenterParams() {
				Title = "Konec igre",
				Width = 36,
				Height = 7
			};

			HandleGameOverDisplay(parameters, Green, (IConsole console) => {
				console.PrintAtCentered(1, "BRAVO!!!");
				console.PrintAtCentered(3, "Pritisni tipko za nadaljevanje", DarkGray);
			});
		}

		/// <summary>
		/// Presents game over screen and then starts a new game.
		/// </summary>
		private void ShowGameOver() {
			var parameters = new CenterParams() {
				Title = "Ni ti uspelo",
				Width = Math.Max(_game!.GuessingPhrase.Length + 6, 36),
				Height = 7
			};

			HandleGameOverDisplay(parameters, Red, (IConsole console) => {
				console.PrintAtColor(Yellow, 2, 1, _game.GuessingPhrase, null);
				console.PrintAtCentered(3, "Pritisni tipko za nadaljevanje", Gray);
			});
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Shows or hides cursor.
		/// </summary>
		private void UpdateInputStatus(bool active) {
			_gallowsConsole.CursorVisible = active;
			_game?.Draw();
		}

		/// <summary>
		/// Interprets given key for guessing the word.
		/// </summary>
		private void InterpretKey(ConsoleKeyInfo key) {
			if (!Dictionary.IsValidLetter(key.KeyChar)) return;

			_game?.AddGuess(key.KeyChar);
			_game?.Draw();

			switch (_game?.Status) {
				case Game.StatusType.Won:
					ShowGameWin();
					break;
				case Game.StatusType.Lost:
					ShowGameOver();
					break;
				case Game.StatusType.Playing:
					// Nothing to do here...
					break;
			}
		}

		/// <summary>
		/// Manages game over display with the given background color. Caller uses given handler to draw into the game over window.
		/// </summary>
		private void HandleGameOverDisplay(CenterParams parameters, ConsoleColor background, GameStatusHandler handler) {
			// Disable input.
			_menu.IsActive = false;
			UpdateInputStatus(false);

			// We want console to be displayed slightly downwards from center.
			parameters.DY = 4;

			// Prepare the console.
			var console = View.ContentConsole.OpenBoxCentered(parameters, new BoxStyle() {
				ThickNess = LineThickNess.Double,
				Body = new Colors(White, background),
				Line = new Colors(White, background),
				Title = new Colors(White, background)
			});

			// Let caller draw the desired text.
			handler(console);

			// Give some minimum time status box is open to prevent to quick close if user is holding key down.
			Thread.Sleep(3000);
			Console.Read();

			// Redraw the screen.
			_menu.IsActive = true;
			_gallowsConsole.Clear();
			Redraw();

			// Finally start a new game.
			StartNewGame();
		}

		#endregion

		#region Declarations

		private delegate void GameStatusHandler(IConsole console);

		#endregion
	}
}
