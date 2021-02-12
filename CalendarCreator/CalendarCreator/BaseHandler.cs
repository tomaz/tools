using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCreator {

	/// <summary>
	/// Common implementation for all "handlers" - aka objects that participate in handling input and output :)
	/// </summary>
	public abstract class BaseHandler {

		protected Options Options { get; init; }

		protected BaseHandler(Options options) => Options = options;
	}
}
