using System;
using System.Drawing;
using static System.ConsoleColor;

namespace Kids.Menu {

	/// <summary>
	/// Horizontal menu.
	/// </summary>
	class HorizontalMenu : Menu {

		#region Initialization & Disposal

		internal HorizontalMenu(MenuBuilder builder) : base(builder) {
			// No additional setup needed.
		}

		#endregion

		#region Overrides

		protected override Point OnDrawItem(Konsole.IConsole console, Point position, MenuItem item, bool selected) {
			var x = position.X;
			var y = position.Y;

			// Draw or clear selection area on the left side of the item.
			console.PrintAtColor(Black, x, y, " ", Background());
			x++;

			// Draw item title.
			console.PrintAtColor(Text(), x, y, item.Title, Background());
			x += item.Title.Length;

			// Draw or clear selection area on the right side of the item.
			console.PrintAtColor(Black, x, y, " ", Background());
			x++;

			ConsoleColor? Background() => selected ? White : (null as ConsoleColor?);
			ConsoleColor Text() => selected ? Black : White;

			return new Point(x + Math.Max(0, item.NextItemOffset), y);
		}

		protected override bool IsPreviousItemKey(ConsoleKeyInfo key) {
			return key.Key == ConsoleKey.LeftArrow;
		}

		protected override bool IsNextItemKey(ConsoleKeyInfo key) {
			return key.Key == ConsoleKey.RightArrow;
		}

		#endregion
	}
}
