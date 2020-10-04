using Konsole;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Kids.Menu {

	/// <summary>
	/// Helper class for building menu items.
	/// </summary>
	public class MenuBuilder {
		internal IConsole Console { get; private set; }
		internal Point Position { get; private set; }
		internal List<MenuItem> Items { get; private set; }
		internal int InitialSelection { get; private set; }

		#region Initialization & Disposal

		public MenuBuilder(IConsole console) {
			Console = console;
			Position = new Point(0, 0);
			Items = new List<MenuItem>();
			InitialSelection = 0;
		}

		#endregion

		#region Builder

		/// <summary>
		/// Adds the given <see cref="MenuItem"/> to the end of current list.
		/// </summary>
		/// <param name="item">Menu item to add.</param>
		/// <returns>Returns <see cref="MenuBuilder"/> for fluent setup.</returns>
		public MenuBuilder Add(MenuItem item) {
			Items.Add(item);
			return this;
		}

		/// <summary>
		/// Convenience for creating simple menu items.
		/// </summary>
		/// <param name="title">Item title.</param>
		/// <param name="action">Action to be invoked when user chooses this item.</param>
		/// <returns>Returns <see cref="MenuBuilder"/> for fluent setup.</returns>
		public MenuBuilder Add(string title, MenuItem.Action action) {
			return Add(new MenuItem() {
				Title = title,
				OnActivated = action
			});
		}

		/// <summary>
		/// Adds all items from the given enumerable to the end of current items list.
		/// </summary>
		/// <param name="items">Items to be added.</param>
		/// <returns>Returns <see cref="MenuBuilder"/> for fluent setup.</returns>
		public MenuBuilder AddAll(IEnumerable<MenuItem> items) {
			Items.AddRange(items);
			return this;
		}

		/// <summary>
		/// Sets menu position relative to <see cref="IConsole"/>.
		/// </summary>
		/// <param name="x">X position.</param>
		/// <param name="y">Y position.</param>
		/// <returns>Returns <see cref="MenuBuilder"/> for fluent setup.</returns>
		public MenuBuilder SetPosition(int x, int y) {
			Position = new Point(x, y);
			return this;
		}

		/// <summary>
		/// Sets initial selection for menu.
		/// </summary>
		/// <param name="selection">Selection index.</param>
		/// <returns>Returns <see cref="MenuBuilder"/> for fluent setup.</returns>
		public MenuBuilder SetInitialSelection(int selection) {
			InitialSelection = selection;
			return this;
		}

		#endregion

		#region Creating

		/// <summary>
		/// Creates a vertical menu from registered data.
		/// </summary>
		/// <returns>Returns vertical menu instance.</returns>
		public Menu VerticalMenu() {
			return new VerticalMenu(this);
		}

		/// <summary>
		/// Creates a horizontal menu from registered data.
		/// </summary>
		/// <returns>Retruns horizontal menu instance.</returns>
		public Menu HorizontalMenu() {
			return new HorizontalMenu(this);
		}

		#endregion
	}
}
