namespace Kids.Menu {

	/// <summary>
	/// Defines common menu actions that are allowed to be called from menu item actions.
	/// </summary>
	public interface IMenu {
		/// <summary
		/// Selects menu item at the given index and redraws menu.
		/// </summary>
		/// <param name="index">Index of the menu to select.</param>
		void SelectItem(int index);

		/// <summary>
		/// Exits menu run loop.
		/// </summary>
		void Exit();
	}
}
