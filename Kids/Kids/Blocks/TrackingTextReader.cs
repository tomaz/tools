using Konsole.Platform.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kids.Blocks {

	/// <summary>
	/// Provides information on current position in the stream as text is read from it.
	/// 
	/// Based on https://stackoverflow.com/a/2594374
	/// </summary>
	public class TrackingTextReader : TextReader {

		private TextReader _baseReader;

		#region Properties

		/// <summary>
		/// Current position.
		/// </summary>
		public int Position { get; private set; }

		#endregion

		#region Initialization & Disposal

		public TrackingTextReader(TextReader reader) {
			_baseReader = reader;
		}

		#endregion

		#region Overrides

		public override int Read() {
			Position++;
			return base.Read();
		}

		public override int Peek() {
			return base.Peek();
		}

		#endregion
	}
}
