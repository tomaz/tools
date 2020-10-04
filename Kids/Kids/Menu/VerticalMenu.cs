using System;
using System.Drawing;
using Konsole;
using static System.ConsoleColor;

namespace Kids.Menu {

	/// <summary>
	/// Vertical menu.
	/// </summary>
	class VerticalMenu : Menu {

		#region Initialization & Disposal

		internal VerticalMenu(MenuBuilder builder) : base(builder) {
			// No additional setup needed.
		}

		#endregion

		#region Overrides

		protected override Point OnDrawItem(IConsole console, Point position, MenuItem item, bool selected) {
			// Draw or clear selection indicator.
			if (selected && IsActive) {
				console.PrintAtColor(Black, position.X, position.Y, " › ", White);
			} else {
				console.PrintAtColor(Black, position.X, position.Y, "   ", null);
			}

			// Draw actual item.
			console.PrintAtColor(White, position.X + 5, position.Y, item.Title, null);

			// Return position for next item.
			return new Point(position.X, position.Y + Math.Max(1, item.NextItemOffset));
		}

		protected override bool IsPreviousItemKey(ConsoleKeyInfo key) {
			return key.Key == ConsoleKey.UpArrow;
		}

		protected override bool IsNextItemKey(ConsoleKeyInfo key) {
			return key.Key == ConsoleKey.DownArrow;
		}

		protected override int DefaultItemOffset() {
			return 2;
		}

		#endregion
	}
}
