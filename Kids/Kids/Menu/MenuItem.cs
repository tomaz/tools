using System;

namespace Kids.Menu {

	/// <summary>
	/// Represents a menu item.
	/// </summary>
	public class MenuItem {
		/// <summary>
		/// Title of the item.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Offset to next item in either lines or characters.
		/// </summary>
		public int NextItemOffset { get; set; }

		/// <summary>
		/// Optional data associated with this item.
		/// </summary>
		public Object? Data { get; set; }

		/// <summary>
		/// Called when this menu item becomes selected.
		/// </summary>
		public Action? OnSelected { get; set; }

		/// <summary>
		/// Called when this menu item becomes unselected (when changing selection, this gets called first on item that's just became unselected, before calling <see cref="OnSelected"/> on new item.
		/// </summary>
		public Action? OnUnselected { get; set; }

		/// <summary>
		/// Called when user presses enter on selected menu item.
		/// </summary>
		public Action? OnActivated { get; set; }

		/// <summary>
		/// Called when given key is pressed while this item is selected.
		/// </summary>
		public KeyAction? OnKeyPress { get; set; }

		#region Initialization & Disposal

		public MenuItem() {
			Title = "";
			NextItemOffset = -1;    // -1 is special value that will get replaced with default offset
			Data = null;
		}

		#endregion

		#region Declarations

		public delegate void Action(IMenu menu);
		public delegate void KeyAction(IMenu menu, ConsoleKeyInfo key);

		#endregion
	}
}
