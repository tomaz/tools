using Kids.Common;
using Konsole;

namespace Kids.Modules {
	
	/// <summary>
	/// Base class for all modules.
	/// 
	/// A module is typically associated with specific "screen" of the app.
	/// </summary>
	abstract public class BaseModule {

		protected IConsole? ParentConsole { get; private set; }
		protected View View { get; private set; }

		#region Initialization & Disposal

		public BaseModule(IConsole? console = null) {
			this.ParentConsole = console;
			this.View = new View(console);
		}

		#endregion

		#region Subclass

		/// <summary>
		/// Draws the module to <see cref="View"/>.
		/// </summary>
		protected abstract void OnDraw();

		/// <summary>
		/// Called when base class is ready to run; subclass should override and handle the session.
		/// </summary>
		protected abstract void OnRun();

		#endregion

		#region Public

		/// <summary>
		/// Runs the module blocking thread until module completes.
		/// </summary>
		public void Run() {
			Clear();
			OnDraw();
			OnRun();
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Pushes given module to "stack" and calls its <see cref="Run"/>.
		/// 
		/// When module is done, control is returned to previous module ensuring it redraws.
		/// </summary>
		/// <param name="module">Module to push.</param>
		protected void Push(BaseModule module) {
			// Run the given module. This will clear the screen and redraw, then run the module until its done.
			module.Run();

			// When returned, redraw this module.
			Redraw();
		}

		/// <summary>
		/// Clears and redraws the module.
		/// </summary>
		protected void Redraw() {
			Clear();
			OnDraw();
		}

		private void Clear() {
			View.TitleConsole.Clear();
			View.ContentConsole.Clear();
		}

		#endregion
	}
}
