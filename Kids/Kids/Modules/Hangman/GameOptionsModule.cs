using Kids.Common;
using Kids.Menu;
using Konsole;
using System;
using System.Linq;
using static System.ConsoleColor;

namespace Kids.Modules.Hangman {
	class GameOptionsModule : BaseModule {

		private static readonly int HorizontalMenuX = 23;

		private readonly Options _options;
		private readonly Menu.Menu _menu;
		private readonly Menu.Menu _lengthMenu;
		private readonly Menu.Menu _difficultyMenu;

		#region Initialization & Disposal

		public GameOptionsModule(IConsole console, Options options) : base(console) {
			_options = options;
			_menu = CreateMenu(console);
			_lengthMenu = CreateLengthMenu(console);
			_difficultyMenu = CreateDifficultyMenu(console);
		}

		#endregion

		#region Overrides

		protected override void OnDraw() {
			_menu.Draw();
			_lengthMenu.Draw();
			_difficultyMenu.Draw();
			PrintStatistics();
		}

		protected override void OnRun() {
			_menu.Run();
		}

		#endregion

		#region Setup

		private Menu.Menu CreateMenu(IConsole console) {
			return new MenuBuilder(console)
				.SetPosition(2, 1)
				.Add(new Menu.MenuItem() {
					Title = "Dolžina besede",
					OnKeyPress = (IMenu menu, ConsoleKeyInfo key) => {
						_lengthMenu.Handle(key);
						_options.GroupIndex = _lengthMenu.SelectedItemIndex;
						PrintStatistics();
					}
				})
				.Add(new Menu.MenuItem() {
					Title = "Težavnost",
					OnKeyPress = (IMenu menu, ConsoleKeyInfo key) => {
						_difficultyMenu.Handle(key);
						_options.Difficulty = _difficultyMenu.SelectedItemIndex;
					}
				})
				.Add("Začni", (IMenu menu) => { menu.Exit(); })
				.VerticalMenu();
		}

		private Menu.Menu CreateLengthMenu(IConsole console) {
			var items = _options.Dictionary.Groups.Select(group => new Menu.MenuItem() {
				Title = group.Title,
				Data = group
			});

			return new MenuBuilder(console)
				.SetPosition(HorizontalMenuX, 1)
				.SetInitialSelection(_options.GroupIndex)
				.AddAll(items)
				.HorizontalMenu();
		}

		private Menu.Menu CreateDifficultyMenu(IConsole console) {
			return new MenuBuilder(console)
				.SetPosition(HorizontalMenuX, 3)
				.SetInitialSelection(_options.Difficulty)
				.Add(new Menu.MenuItem() { Title = "Lahka", Data = Game.DifficultyType.Easy })
				.Add(new Menu.MenuItem() { Title = "Srednja", Data = Game.DifficultyType.Medium })
				.Add(new Menu.MenuItem() { Title = "Težka", Data = Game.DifficultyType.Hard })
				.HorizontalMenu();
		}

		#endregion

		#region Helpers

		private void PrintStatistics() {
			var y = _menu.BottomRight.Y + 1;
			ParentConsole?.PrintAtCentered(y++, $"     {_options.Group.Words.Count:N0} besed (od {_options.Dictionary.AllWordsCount:N0} skupaj)      ", DarkGray);
			ParentConsole?.PrintAtCentered(y++, "Vir: http://bos.zrc-sazu.si/besede.html", DarkGray);
		}

		#endregion

		#region Declarations

		public class Options {
			public Dictionary Dictionary { get; }
			public int GroupIndex { get; set; }
			public int Difficulty { get; set; }
			public Dictionary.Group Group => Dictionary.Groups[GroupIndex];

			public Options() {
				Dictionary = new Dictionary();
				Difficulty = 0;
				GroupIndex = 0;
			}
		}

		#endregion
	}
}