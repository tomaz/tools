using System;

namespace CalendarCreator {

	/// <summary>
	/// Holds information about each event.
	/// </summary>
	public record Event {
		public String Title { get; init; }
		public String Location { get; init; }
		public DateTime From { get; init; }
		public DateTime To { get; init; }
	}
}