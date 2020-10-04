using Konsole;
using System;
using System.Collections.Generic;
using System.Drawing;
using static System.ConsoleColor;

namespace Kids.Menu {

	/// <summary>
	/// Specifies requirements for a menu handler.
	/// </summary>
	public abstract class Menu : IMenu {

		/// <summary>
		/// Position within parent <see cref="IConsole"/>.
		/// </summary>
		public Point Position { get; private set; }

		/// <summary>
		/// Bottom right position of the menu within parent <see cref="IConsole"/>. Updated during <see cref="Draw"/>.
		/// </summary>
		public Point BottomRight { get; private set; }

		/// <summary>
		/// Currently selected item. Will throw exception if no item is assigned.
		/// </summary>
		public MenuItem SelectedItem => _items[SelectedItemIndex];

		/// <summary>
		/// Currently selected item index.
		/// </summary>
		public int SelectedItemIndex { get; private set; } = 0;

		/// <summary>
		/// Specifies whether the menu is currently active or not. This also invokes selection events on selected item when value changes.
		/// </summary>
		public bool IsActive {
			get { return _active; }
			set {
				if (value == _active) return;
				_active = value;
				
				if (_active) {
					SelectedItem.OnSelected?.Invoke(this);
				} else {
					SelectedItem.OnUnselected?.Invoke(this);
				}

				Draw();
			}
		}

		private readonly IConsole _console;
		private readonly List<MenuItem> _items;

		private bool _active = true;
		private bool _running = false;

		#region Initialization & Disposal

		protected Menu(MenuBuilder builder) {
			Position = builder.Position;
			BottomRight = Position;

			_active = true;
			SelectedItemIndex = builder.InitialSelection;

			_console = builder.Console;
			_items = builder.Items;

			_items.ForEach((MenuItem item) => {
				if (item.NextItemOffset < 0) {
					item.NextItemOffset = DefaultItemOffset();
				}
			});
		}

		#endregion

		#region IMenu

		public void SelectItem(int index) {
			ChangeSelection(() => SelectedItemIndex = index);
		}

		public void Exit() {
			_running = false;
		}

		#endregion

		#region Subclass

		/// <summary>
		/// Draws the given item and advances position for next item.
		/// </summary>
		/// <param name="console"><see cref="IConsole"/> to draw in.</param>
		/// <param name="position">Position to draw at.</param>
		/// <param name="item">Item to draw.</param>
		/// <param name="selected">Whether the item is selected or not.</param>
		/// <returns>New scren position.</returns>
		protected abstract Point OnDrawItem(IConsole console, Point position, MenuItem item, bool selected);

		/// <summary>
		/// Determines if the given key represents navigation to previous item.
		/// </summary>
		/// <param name="key">Key to check.</param>
		/// <returns>True if key is for previous item, false otherwise.</returns>
		protected abstract bool IsPreviousItemKey(ConsoleKeyInfo key);

		/// <summary>
		/// Determines if the given key represents navigation to next item.
		/// </summary>
		/// <param name="key">Key to check</param>
		/// <returns>True if key is for next item, false otherwise.</returns>
		protected abstract bool IsNextItemKey(ConsoleKeyInfo key);

		/// <summary>
		/// Specifies default item offset applied to all items that don't have their specific one.
		/// </summary>
		/// <returns>Returns item offset.</returns>
		protected virtual int DefaultItemOffset() {
			return 0;
		}

		#endregion

		#region Public

		/// <summary>
		/// Draws the menu.
		/// </summary>
		public void Draw() {
			var pos = new Point(Position.X, Position.Y);

			for (var i = 0; i < _items.Count; i++) {
				var item = _items[i];
				var selected = (i == SelectedItemIndex);
				pos = OnDrawItem(_console, pos, item, selected);
			}

			BottomRight = pos;
		}

		/// <summary>
		/// Runs the menu.
		/// 
		/// This function is synchronous and blocks the thread until <see cref="Exit"/> is called. If no option is added, function exits immediately. The function is suitable when menu is run as "standalone".
		/// </summary>
		public void Run() {
			if (_items.Count <= 0) return;

			// Remember original cursor visibility (so we can restore) and make it invisible.
			var originalCursorVisible = Console.CursorVisible;
			Console.CursorVisible = false;

			// Draw the menu.
			Draw();

			// This ensures selected action is called on initially selected item.
			_items[SelectedItemIndex].OnSelected?.Invoke(this);

			// Handle the run loop.
			_running = true;
			while (_running) {
				var key = Console.ReadKey(true);
				Handle(key);
			}

			// Restore cursor.
			Console.CursorVisible = originalCursorVisible;
		}

		/// <summary>
		/// Handles the given key.
		/// 
		/// This function is suitable when menu is used as submenu of a <see cref="MenuItem"/> for example. In such case you can implement <see cref="MenuItem.OnKeyPress"/> event and pass the key to this function.
		/// </summary>
		/// <param name="key">Key to handle</param>
		public void Handle(ConsoleKeyInfo key) {
			if (IsPreviousItemKey(key)) {
				// User wants to move to previous item.
				if (SelectedItemIndex == 0) return;
				ChangeSelection(() => SelectedItemIndex--);

			} else if (IsNextItemKey(key)) {
				// User wants to move to next item.
				if (SelectedItemIndex == _items.Count - 1) return;
				ChangeSelection(() => SelectedItemIndex++);

			} else if (key.Key == ConsoleKey.Enter) {
				// User selected currently selected item.
				_items[SelectedItemIndex].OnActivated?.Invoke(this);
			}

			// For all other keys, send them to currently selected item.
			_items[SelectedItemIndex].OnKeyPress?.Invoke(this, key);
		}

		/// <summary>
		/// Deactivates this menu at the start and re-activates it again after given handler exits.
		/// 
		/// This is just a convenience for managing <see cref="IsActive"/> while certain process is running.
		/// </summary>
		/// <param name="handler">Handler to execute while this menu will be inactive.</param>
		public void DeactivateWhile(Handler handler) {
			IsActive = false;

			handler();

			IsActive = true;
		}

		#endregion

		#region Helpers

		private void ChangeSelection(Handler handler) {
			// Notify current item it's being unselected.
			_items[SelectedItemIndex].OnUnselected?.Invoke(this);

			// Invoke handler that will change selection index.
			handler();

			// Notify new item it's become selected.
			_items[SelectedItemIndex].OnSelected?.Invoke(this);

			// Redraw the menu.
			Draw();
		}

		#endregion

		#region Declarations

		public delegate void Handler();

		#endregion
	}
}
