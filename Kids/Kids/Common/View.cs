using Kids.Common;
using Konsole;
using System;
using static System.ConsoleColor;

namespace Kids.Common {
	/// <summary>
	/// Helper class for managing a screen.
	/// 
	/// To use, instantiate, then access common screen parts via helper functions.
	/// </summary>
	public sealed class View {

		private readonly IConsole _console;

		#region Properties

		/// <summary>
		/// Provides direct access to title <see cref="IConsole"/>.
		/// </summary>
		public IConsole TitleConsole { get; }

		/// <summary>
		/// Provides direct access to main content <see cref="IConsole"/>.
		/// </summary>
		public IConsole ContentConsole { get; }

		/// <summary>
		/// Provides direct access to <see cref="Draw"/> for rendering in <see cref="TitleConsole"/>.
		/// </summary>
		public Draw TitleDraw { get; }

		/// <summary>
		/// Provides direct access to <see cref="Draw"/> for rendering in <see cref="ContentConsole"/>.
		/// </summary>
		public Draw ContentDraw { get; }

		#endregion

		#region Initialization & Disposal

		public View(IConsole? parent = null) {
			// Use existing parent console or create new window taking full available screen area.
			_console = parent ?? new Window();

			// Split it vertically into 2 consoles, top is 3 lines high, bottom takes remaining space.
			var areas = _console.SplitRows(
				new Split(3),
				new Split(0));

			// Prepare direct links to both consoles.
			TitleConsole = areas[0];
			ContentConsole = areas[1];

			// Prepare default colors for title console.
			TitleConsole.ForegroundColor = Black;
			TitleConsole.BackgroundColor = DarkYellow;

			// Prepares draws for both consoles.
			TitleDraw = new Draw(TitleConsole);
			ContentDraw = new Draw(ContentConsole);
		}

		#endregion

		#region Public

		/// <summary>
		/// Sets the title, optionally changing the color and underling style.
		/// </summary>
		/// <param name="title">Title to use</param>
		/// <param name="color">Color to use, or null to use default color.</param>
		public void SetTitle(string title, ConsoleColor? color = null) {
			var foreground = (color == null) ? Black : color;
			var background = (color == null) ? DarkYellow : (ConsoleColor?)null;
			TitleConsole.PrintAtCentered(1, title, foreground, background);
		}

		#endregion
	}
}
