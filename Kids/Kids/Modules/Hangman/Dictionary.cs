using Kids.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kids.Modules.Hangman {

	/// <summary>
	/// Handles the words for hangman.
	/// </summary>
	class Dictionary {

		/// <summary>
		/// List of all <see cref="Group"/> in order based on word length. Only available after <see cref="Initialize(ProgressDelegate?)"/>!
		/// </summary>
		public List<Group> Groups { get; private set; } = new List<Group>();

		/// <summary>
		/// Number of all words in the dictionary. Only available after <see cref="Initialize(ProgressDelegate?)"/>!
		/// </summary>
		public int AllWordsCount { get; private set; }

		private static readonly string ValidLowerCaseLetters = "abcčdefghijklmnoprsštuvzž";  // At the moment only Slovenian alphabet is supported.
		private static readonly string ValidUpperCaseLetters = ValidLowerCaseLetters.ToUpper();

		private static readonly double ParsingProgressRatio = 0.8;  // Parsing will take 80% of initialization.
		private static readonly double GroupingProgressRatio = 1 - ParsingProgressRatio;
		private static readonly int MaxGroups = 5;

		#region Public

		/// <summary>
		/// Determines if the given char is a valid letter or not.
		/// </summary>
		/// <param name="ch">Char to validate.</param>
		/// <returns>True if char is valid letter, false otherwise.</returns>
		static public bool IsValidLetter(Char ch) {
			if (ValidLowerCaseLetters.Contains(ch)) return true;
			if (ValidUpperCaseLetters.Contains(ch)) return true;
			return false;
		}

		/// <summary>
		/// Initializes all strings.
		/// </summary>
		/// <param name="progress">Delegate that will be informed of initialization progress.</param>
		public void Initialize(ProgressDelegate? progress = null) {
			progress?.Invoke(0.0);

			var wordsByLengths = ParseWords(progress);
			Groups = ParseGroups(wordsByLengths, progress);
			AllWordsCount = Groups.Aggregate(0, (acc, group) => acc + group.Words.Count);

			progress?.Invoke(1.0);
		}

		#endregion

		#region Parsing

		private Dictionary<int, List<string>> ParseWords(ProgressDelegate? progress) {
			var result = new Dictionary<int, List<string>>();

			var text = Properties.Resources.sskj;
			using var stringReader = new StringReader(text);
			using var textReader = new TrackingTextReader(stringReader);

			string? line;
			var previousProgress = 0.0;
			while ((line = stringReader.ReadLine()) != null) {
				// Add word to list of words of this size; create list if needed.
				if (!result.ContainsKey(line.Length)) {
					result.Add(line.Length, new List<string>());
				}
				result[line.Length].Add(line);

				// Report progress.
				var progressRatio = ((double)textReader.Position / (double)text.Length) * ParsingProgressRatio;
				if (progressRatio != previousProgress) {
					previousProgress = progressRatio;
					progress?.Invoke(progressRatio);
				}
			}

			progress?.Invoke(ParsingProgressRatio);

			return result;
		}

		private List<Group> ParseGroups(Dictionary<int, List<string>> wordsByLengths, ProgressDelegate? progress) {
			var result = new List<Group>();

			// Sort all lengths first.
			var lengths = wordsByLengths.Keys.Select(i => i).ToList();
			lengths.Sort();

			// Determine how many lengths will fit each "group".
			var lengthsPerGroup = lengths.Count / MaxGroups;

			// Loop over all lengths and prepare the groups.
			Group? group = null;
			var n = 0;  // i will iterate from 0 to lengthsPerGroup and then wrap
			var previousProgress = GroupingProgressRatio;
			for (var i = 0; i  < lengths.Count; i++) {
				var length = lengths[i];
				var words = wordsByLengths[length];

				// When remainder of n / lengthsPerGroup is 0, it indicates we must create new group. But don't create more groups than required - just attach words to the end of last group in such case.
				if (result.Count < MaxGroups && n % lengthsPerGroup == 0) {
					group = new Group() { MinLength = length };
					result.Add(group);
				}

				// Add current strings to group and update max length.
				group!.MaxLength = length;
				group!.Words.AddRange(words);

				// Report progress.
				var progressRatio = GroupingProgressRatio + (double)i / (double)lengths.Count;
				if (progressRatio != previousProgress) {
					previousProgress = progressRatio;
					progress?.Invoke(progressRatio);
				}

				// Increment our group counter.
				n++;
			}

			return result;
		}

		#endregion

		#region Declarations

		public class Group {
			/// <summary>
			/// Minimum word length.
			/// </summary>
			public int MinLength { get; set; }

			/// <summary>
			/// Maximum word length.
			/// </summary>
			public int MaxLength { get; set; }

			/// <summary>
			/// List of all words.
			/// </summary>
			public List<string> Words { get; set; }

			/// <summary>
			/// Prepares group title.
			/// </summary>
			public string Title {
				get {
					if (MaxLength == MinLength) return MinLength.ToString();
					return $"{MinLength}-{MaxLength}";
				}
			}

			/// <summary>
			/// Returns a random word from this group.
			/// </summary>
			public string RandomWord {
				get {
					return Words[new Random().Next(Words.Count)];
				}
			}

			internal Group() {
				MinLength = 0;
				MaxLength = 0;
				Words = new List<string>();
			}
		}

		/// <summary>
		/// Delegate that's being informed about progress.
		/// </summary>
		/// <param name="progress">Progress in the range of 0 to 1.</param>
		public delegate void ProgressDelegate(double progress);

		#endregion
	}
}
