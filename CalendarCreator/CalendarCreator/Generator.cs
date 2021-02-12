using System;
using System.IO;
using System.Collections.Generic;

namespace CalendarCreator {

	/// <summary>
	/// Generates output from given list of <see cref="Event"/>s.
	/// </summary>
	public class Generator : BaseHandler {

		private List<String> lines;

		public Generator(Options options) : base(options) {
			this.lines = new List<string>();
		}

		public void Generate(List<Event> events) {
			Console.WriteLine($"Generating output for {events.Count} events ");

			// Write header.
			AddLines(
				"BEGIN:VCALENDAR",
				"PRODID:-//Tomaz Kragelj//Calendar//EN",
				"VERSION:2.0",
				"CALSCALE:GREGORIAN",
				"METHOD:PUBLISH",
				$"X-WR-CALNAME:{Options.CalendarName}",
				"X-WR-TIMEZONE:UTC",
				"X-WR-CALDESC:");

			// Write all events.
			var index = 1;
			foreach (var e in events) {
				Console.WriteLine($"{index,3}: {e.Title}");

				var id = Guid.NewGuid().ToString().Replace("-", "");

				AddLines(
					"BEGIN:VEVENT",
					$"DTSTART:{FormattedTime(e.From)}",
					$"DTEND:{FormattedTime(e.To)}",
					$"DTSTAMP:{FormattedTime(e.From)}",
					$"UID:{id}@gentlebytes.com",
					$"CREATED:{FormattedTime(DateTime.Now)}",
					$"DESCRIPTION:",
					$"LAST-MODIFIED:{FormattedTime(DateTime.Now)}",
					$"LOCATION:{e.Location}",
					"SEQUENCE:0",
					"STATUS:CONFIRMED",
					$"SUMMARY:{e.Title}",
					"TRANSP:TRANSPARENT",
					"END:VEVENT");

				index++;
			}

			// Write footer.
			AddLines("END:VCALENDAR");

			// Now finally generate the output file.
			WriteGeneratedLines();
		}

		private void WriteGeneratedLines() {
			if (lines.Count == 0) {
				Console.WriteLine("Nothing to generate...");
				return;
			}

			Console.WriteLine($"Writting {lines.Count} lines");
			File.WriteAllLines(Options.Output, lines);
		}

		private void AddLines(params String[] lines) {
			this.lines.AddRange(lines);
		}

		private String FormattedTime(DateTime time) {
			return time.ToString("yyyyMMddTHHmmssZ");
		}
	}
}
