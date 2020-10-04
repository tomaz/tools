using System;
using Konsole;
using static System.ConsoleColor;

namespace Kids.Common {
	static class KonsoleExtension {

		/// <summary>
		/// Opens a new box centered in the given <see cref="IConsole"/>.
		/// </summary>
		public static IConsole OpenBoxCentered(
			this IConsole console,
			string title,
			int width,
			int height,
			BoxStyle? style = null) {

			var parameters = new CenterParams() {
				Title = title,
				Width = width,
				Height = height
			};

			return OpenBoxCentered(console, parameters, style);
		}

		/// <summary>
		/// Opens a new box centered in the given <see cref="IConsole"/> by given offset.
		/// </summary>
		public static IConsole OpenBoxCentered(
			this IConsole console,
			CenterParams parameters,
			BoxStyle? style = null) {

			var x = (console.WindowWidth - parameters.Width) / 2 + parameters.DX;
			var y = (console.WindowHeight - parameters.Height) / 2 + parameters.DY;

			if (style != null) {
				return console.OpenBox(parameters.Title, x, y, parameters.Width, parameters.Height, style);
			} else {
				return console.OpenBox(parameters.Title, x, y, parameters.Width, parameters.Height);
			}
		}

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

	public class CenterParams {
		public string Title { get; set; } = "";
		public int DX { get; set; } = 0;
		public int DY { get; set; } = 0;
		public int Width { get; set; }
		public int Height { get; set; }
	}
}
