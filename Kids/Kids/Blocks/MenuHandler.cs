using System;
using System.Collections.Generic;
using System.Drawing;
using Konsole;
using static System.ConsoleColor;

namespace Kids.Blocks {

	/// <summary>
	/// Reusable menu handling class.
	/// </summary>
	class MenuHandler : IMenu {
		
		/// <summary>
		/// Menu position within provided <see cref="IConsole"/>.
		/// </summary>
		public Point Position { get => _position; }

		/// <summary>
		/// Total menu height in lines.
		/// </summary>
		public int Height { get; private set; } = 0;

		/// <summary>
		/// Bottom coordinate of the menu within provided <see cref="IConsole"/>.
		/// </summary>
		public int Bottom { get => Height + _position.Y; }

		private readonly IConsole _console;

		private readonly List<Item> _items = new List<Item>();
		private Point _position = new Point(0, 0);

		private bool _running = false;
		private int _selectedItem = 0;

		#region Initialization & Disposal

		public MenuHandler(IConsole console) {
			this._console = console;
		}

		#endregion

		#region IMenu

		/// <summary>
		/// Selects the given item and redraws menu.
		/// </summary>
		/// <param name="index"Index of item to select.</param>
		public void SelectItem(int index) {
			ChangeSelection(() => _selectedItem = index);
		}

		/// <summary>
		/// Exits the menu.
		/// </summary>
		public void Exit() {
			_running = false;
		}

		#endregion

		#region Setup

		/// <summary>
		/// Adds the given menu <see cref="Item"/>.
		/// </summary>
		/// <param name="item">Menu item to add.</param>
		/// <returns>Returns <see cref="MenuHandler"/> for fluent setup.</returns>
		public MenuHandler Add(Item item) {
			_items.Add(item);
			return this;
		}

		/// <summary>
		/// Convenience for creating simple menu items.
		/// </summary>
		/// <param name="title">Item title.</param>
		/// <param name="action">Action to be invoked when user chooses this item.</param>
		/// <returns>Returns <see cref="MenuHandler"/> for fluent setup.</returns>
		public MenuHandler Add(string title, Action action) {
			return Add(new Item() {
				Title = title,
				OnActivated = action
			});
		}

		/// <summary>
		/// Sets menu position relative to <see cref="IConsole"/>.
		/// </summary>
		/// <param name="x">X position.</param>
		/// <param name="y">Y position.</param>
		/// <returns>Returns <see cref="MenuHandler"/> for fluent setup.</returns>
		public MenuHandler SetPosition(int x, int y) {
			_position.X = x;
			_position.Y = y;
			return this;
		}

		#endregion

		#region Public

		/// <summary>
		/// Draws the menu in its current state.
		/// </summary>
		public void Draw() {
			var x = _position.X;
			var y = _position.Y;
			var top = y;

			for (int i = 0; i < _items.Count; i++) {
				var option = _items[i];

				if (i == _selectedItem) {
					_console.PrintAtColor(Black, x, y, " › ", White);
				} else {
					_console.PrintAtColor(Black, x, y, "   ", null);
				}

				_console.PrintAtColor(White, x + 5, y, option.Title, null);

				Height = y - top + 1;

				y += Math.Max(1, option.NextItemOffset);
			}
		}

		/// <summary>
		/// Runs the menu.
		/// 
		/// This function is synchronous and blocks the thread until <see cref="Exit"/> is called. If no option is added, function exits immediately.
		/// </summary>
		public void Run() {
			var originalCursorVisible = Console.CursorVisible;
			Console.CursorVisible = false;

			_running = (_items.Count > 0);
			_selectedItem = 0;

			Draw();

			_items[0].OnSelected?.Invoke(this);

			while (_running) {
				Handle();
			}

			Console.CursorVisible = originalCursorVisible;
		}

		#endregion

		#region Helpers

		private void Handle() {
			var key = Console.ReadKey(true);

			switch (key.Key) {
				case ConsoleKey.UpArrow:
					if (_selectedItem == 0) break;
					ChangeSelection(() => _selectedItem--);
					break;

				case ConsoleKey.DownArrow:
					if (_selectedItem == _items.Count - 1) break;
					ChangeSelection(() => _selectedItem++);
					break;

				case ConsoleKey.Enter:
					_items[_selectedItem].OnActivated?.Invoke(this);
					break;
			}

			_items[_selectedItem].OnKeyPress?.Invoke(this, key);
		}

		private void ChangeSelection(Handler handler) {
			_items[_selectedItem].OnUnselected?.Invoke(this);
			handler();
			_items[_selectedItem].OnSelected?.Invoke(this);
			Draw();
		}

		#endregion

		#region Declarations

		public delegate void Action(IMenu menu);
		public delegate void KeyAction(IMenu menu, ConsoleKeyInfo key);
		private delegate void Handler();

		public class Item {
#nullable enable
			public string Title { get; set; }
			public int NextItemOffset { get; set; }
			public Action? OnSelected { get; set; }
			public Action? OnUnselected { get; set; }
			public Action? OnActivated { get; set; }
			public KeyAction? OnKeyPress { get; set; }
#nullable disable

			public Item() {
				NextItemOffset = 2;
			}
		}

		#endregion
	}

	#region Declarations

	public interface IMenu {
		void SelectItem(int index);
		void Exit();
	}

	#endregion
}
