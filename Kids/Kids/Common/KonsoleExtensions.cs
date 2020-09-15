using System;
using Konsole;
using static System.ConsoleColor;

namespace Kids.Common {
	static class KonsoleExtension {

		/// <summary>
		/// Prints the given text on the given coordinates of the given <see cref="IConsole"/> and updates current cursor position.
		/// </summary>
		public static void Print(
			this IConsole console, 
			int x, 
			int y, 
			string text, 
			ConsoleColor? foreground = null, 
			ConsoleColor? background = null) {

			if (foreground.HasValue) {
				console.PrintAtColor(foreground.Value, x, y, text, background);
			} else {
				console.PrintAt(x, y, text);
			}

			console.CursorLeft = x + text.Length;
			console.CursorTop = y;
		}

		/// <summary>
		/// Prints given text centered on the given line of the given <see cref="IConsole"/> and updates current cursor position.
		/// </summary>
		public static void PrintAtCentered(
			this IConsole console, 
			int y, 
			string text, 
			ConsoleColor? foreground = null, 
			ConsoleColor? background = null) {

			var x = (console.WindowWidth - text.Length) / 2;

			if (foreground.HasValue) {
				console.PrintAtColor(foreground.Value, x, y, text, background);
			} else {
				console.PrintAt(x, y, text);
			}

			console.CursorLeft = x + text.Length;
			console.CursorTop = y;
		}

		/// <summary>
		/// Prints given text centerd horizontally and vertically in the given <see cref="IConsole"/> and updates current cursor position.
		/// </summary>
		public static void PrintAtCentered(
			this IConsole console,
			string text,
			ConsoleColor? foreground = null,
			ConsoleColor? background = null) {

			var x = (console.WindowWidth - text.Length) / 2;
			var y = console.WindowHeight / 2;

			if (foreground.HasValue) {
				console.PrintAtColor(foreground.Value, x, y, text, background);
			} else {
				console.PrintAt(x, y, text);
			}

			console.CursorLeft = x + text.Length;
			console.CursorTop = y;
		}

		/// <summary>
		/// Draws a line centered on the given line of the given <see cref="IConsole"/>
		/// </summary>
		public static void DrawLine(
			this IConsole console, 
			int y, 
			int width, 
			LineThickNess? thickness = null, 
			ConsoleColor? color = null) {

			var currentColor = console.ForegroundColor;
			var x = (console.WindowWidth - width) / 2 - 1;

			if (color.HasValue) {
				console.ForegroundColor = color.Value;
			}

			new Draw(console).Line(x, y, x + width, y, thickness);

			if (color != null) {
				console.ForegroundColor = currentColor;
			}
		}
	}
}
