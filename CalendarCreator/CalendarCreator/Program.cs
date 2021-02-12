using System;
using System.Collections.Generic;

namespace CalendarCreator {

	/// <summary>
	/// Converts simple line delimiter calendar format into ics file suitable for importing to calendar app.
	/// </summary> 
	class Program {
		static void Main(string[] args) {
			if (args.Length == 0) {
				Console.WriteLine("USAGE: CalendarCreator <source>");
				Console.WriteLine();
				Console.WriteLine("Source should contain list of events separated by empty line");
				Console.WriteLine();
				Console.WriteLine("Format for each event:");
				Console.WriteLine("YYYYMMDD");
				Console.WriteLine("HHMM-HHMM");
				Console.WriteLine("title wstring");
				Console.WriteLine("location wstring");
				Console.WriteLine();
				Console.WriteLine("Output: <source>.ics");
				return;
			}

			var options = new Options(args[0]);
			var events = new Parser(options).Parse();
			new Generator(options).Generate(events);
		}
	}
}
