using System;
using System.Collections.Generic;
using System.Text;

namespace Kids.Modules.UnitConversion {
	public class Unit {
		public string Name { get; private set; }
		public decimal Scale { get; private set; }

		public Unit(string name, decimal scale) {
			this.Name = name;
			this.Scale = scale;
		}

		public decimal ToStandard(decimal value) {
			return value * Scale;
		}

		public decimal FromStandard(decimal value) {
			return value / Scale;
		}
	}
}
