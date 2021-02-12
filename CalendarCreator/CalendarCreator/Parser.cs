using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace CalendarCreator {
#nullable enable

	/// <summary>
	/// Parses the given data into list of <see cref="Event"/>s.
	/// </summary>
	public class Parser : BaseHandler {

		private static readonly int DataLinesCount = 4;
		private static readonly int EntryLinesCount = DataLinesCount + 1;

		public Parser(Options options) : base(options) { }

		public List<Event> Parse() {
			Console.WriteLine("Reading data...");
			var lines = File.ReadAllLines(Options.Input, Encoding.UTF8);

			Console.WriteLine($"Parsing {lines.Count()} lines...");

			// Group lines into arrays of 5 (4 data lines + empty delimiter).
			var entries = lines
				.Select((x, i) => new { Index = i, Value = x })
				.GroupBy(x => x.Index / EntryLinesCount)
				.Select(x => x.Select(v => v.Value).ToList())
				.ToList();

			// Parse each entry that has 4 lines.
			var index = 1;
			var result = new List<Event>();
			foreach (var entry in entries) {
				Console.Write($"{index,3}: ");

				// If entry doesn't contain enough lines, exit.
				if (entry.Count() < DataLinesCount) {
					Console.WriteLine($"Only {entry.Count()} lines found, 4 expected");
					continue;
				}

				// Parse date from line 1.
				var date = ParseDate(entry[0]);
				if (date == null) continue;

				// Parse from and to times from line 2.
				var fromTo = ParseTimes(entry[1]);
				if (fromTo == null) continue;

				// Get title and location.
				var title = entry[2];
				var location = entry[3];

				// Create event.
				var e = new Event { 
					Title = title, 
					Location = location,
					From = DateAndTime(date.Value, fromTo.From),
					To = DateAndTime(date.Value, fromTo.To)
				};
		
				Console.WriteLine($"{e.From:dd.MM.yyyy} / {e.From:HH:mm}-{e.To:HH:mm} / {title}");

				result.Add(e);
				index++;
			}

			return result;
		}

		private DateTime? ParseDate(String source) {
			try {
				return DateTime.ParseExact(source, "yyyyMMdd", CultureInfo.InvariantCulture);
			} catch (FormatException) {
				Console.WriteLine($"Failed parsing date from {source}");
				return null;
			}
		}

		private DateTime? ParseTime(string source) {
			try {
				return DateTime.ParseExact(source, "HHmm", null);
			} catch (FormatException) {
				Console.WriteLine($"Failed parsing time from {source}");
				return null;
			}
		}

		private FromTo? ParseTimes(String source) {
			var components = source.Split('-');

			if (components.Length != 2) {
				Console.WriteLine($"Expected 2 time components in {source}");
				return null;
			}

			var from = ParseTime(components[0]);
			if (from == null) return null;

			var to = ParseTime(components[1]);
			if (to == null) return null;

			return new FromTo { From = from.Value, To = to.Value };
		}

		private DateTime DateAndTime(DateTime date, DateTime time) {
			return TimeZoneInfo.ConvertTimeToUtc(date.AddHours(time.Hour).AddMinutes(time.Minute));
		}
	}

	record FromTo {
		public DateTime From { get; init; }
		public DateTime To { get; init; }
	}
}