using System;
using System.IO;

namespace CalendarCreator {

	/// <summary>
	/// Options for this session.
	/// </summary>
	public class Options {
		/// <summary>
		/// Input filename and path.
		/// </summary>
		public String Input { get; init; }

		/// <summary>
		/// Output filename and path.
		/// </summary>
		public String Output => Path.ChangeExtension(Input, "ics");

		/// <summary>
		/// Name of the generated calendar.
		/// </summary>
		public String CalendarName => "\\(arguments.calendarName)";

		public Options(String input) => Input = input;
	}
}
