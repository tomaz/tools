using Konsole;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Mail;
using System.Text;

namespace Kids.Modules.Hangman {
	abstract class HangmanDrawing {

		protected IConsole Console { get; private set; }
		protected Point Position { get; private set; }

		protected HangmanDrawing(IConsole console, int x, int y) {
			Console = console;
			Position = new Point(x, y);
		}

		protected void DrawRope() { DrawRelative(1, -1, "╤"); DrawRelative(1, 0, "│"); }

		protected void DrawHead() => DrawRelative(0, 1, "(-)");

		protected void DrawLeftShoulder() => DrawRelative(0, 2, "┌");
		protected void DrawNeck() => DrawRelative(1, 2, "█");
		protected void DrawRightShoulder() => DrawRelative(2, 2, "┐");

		protected void DrawLeftArm() => DrawRelative(0, 3, "│");
		protected void DrawBody() => DrawRelative(1, 3, "║");
		protected void DrawRightArm() => DrawRelative(2, 3, "│");

		protected void DrawLeftHip() => DrawRelative(0, 4, "┌");
		protected void DrawCrotch() => DrawRelative(1, 4, "╨");
		protected void DrawRightHip() => DrawRelative(2, 4, "┐");

		protected void DrawLeftLeg() => DrawRelative(0, 5, "┘");
		protected void DrawRightLeg() => DrawRelative(2, 5, "└");

		private void DrawRelative(int x, int y, string text) => Console.PrintAt(Position.X + x, Position.Y + y, text);

		abstract public void Draw(int level);
		abstract public bool IsGameOver(int level);
	}

	class EasyHangmanDrawing : HangmanDrawing {

		public EasyHangmanDrawing(IConsole console, int x, int y) : base(console, x, y) { }

		public override void Draw(int step) {
			if (step > 0) { DrawRope(); DrawHead(); }
			if (step > 1) { DrawLeftShoulder(); DrawNeck(); DrawRightShoulder(); }
			if (step > 2) DrawLeftArm();
			if (step > 3) DrawBody();
			if (step > 4) DrawRightArm();
			if (step > 5) DrawCrotch();
			if (step > 6) DrawLeftHip();
			if (step > 7) DrawRightHip();
			if (step > 8) DrawLeftLeg();
			if (step > 9) DrawRightLeg();
		}

		public override bool IsGameOver(int step) {
			return step >= 10;
		}
	}

	class MediumHangmanDrawing : HangmanDrawing {

		public MediumHangmanDrawing(IConsole console, int x, int y) : base(console, x, y) { }

		public override void Draw(int step) {
			if (step > 0) { DrawRope(); DrawHead(); }
			if (step > 1) { DrawLeftShoulder(); DrawNeck(); DrawRightShoulder(); }
			if (step > 2) { DrawLeftArm(); DrawBody(); DrawRightArm(); }
			if (step > 3) DrawCrotch(); 
			if (step > 4) { DrawLeftHip(); DrawRightHip();  }
			if (step > 5) DrawLeftLeg();
			if (step > 6) DrawRightLeg();
		}

		public override bool IsGameOver(int step) {
			return step >= 7;
		}
	}

	class HardHangmanDrawing : HangmanDrawing {

		public HardHangmanDrawing(IConsole console, int x, int y) : base(console, x, y) { }

		public override void Draw(int step) {
			if (step > 0) { DrawRope(); DrawHead(); }
			if (step > 1) { DrawLeftShoulder(); DrawNeck(); DrawRightShoulder(); }
			if (step > 2) { DrawLeftArm(); DrawBody(); DrawRightArm(); }
			if (step > 3) { DrawLeftHip(); DrawCrotch(); DrawRightHip(); }
			if (step > 4) { DrawLeftLeg(); DrawRightLeg(); }
		}

		public override bool IsGameOver(int step) {
			return step >= 5;
		}
	}
}
