#pragma once

#include "Screen.h"

/// Abstract base class for handling a view.
///
/// View represents one screen of the app, for example: menu etc and is associated with its own `Screen` instance for managing PDCurses and the rest of the rendering.
class View {
public:
	View();
	virtual ~View();

public:
	/// Runs the view until user exists it.
	void run();

protected:
	/// Called only once from `run`.
	///
	/// Note: this is called before screen is initialized.
	virtual void onInit();

	/// Instructs subclass to draw the view.
	///
	/// This function should render the screen and exit. It's immediately followed by a call to `onInput` which is where user input should be managed.
	virtual void onDraw(const Screen &screen) = 0;

	/// Handles user input.
	///
	/// This is called after `onDraw` to handle user input. It should manage input and return false when screen is completed and we can exit.
	///
	/// Default implementation simply returns false to exit render loop.
	virtual bool onHandle();

	/// Ad hoc drawing function.
	///
	/// The main purpose is to get hold of underlying `Screen` so we can render stuff on it.
	void draw(std::function<void(const Screen &)> handler);

	/// Convenience for Screen::restorePosition so that we can call it before initiating drawing on it.
	void restorePosition(std::function<void(const Screen &)> handler);

	/// Switches to a different view.
	///
	/// Create an instance and call `View::run` on it inside the given handler.
	void switchView(std::function<void(void)> handler);

private:
	void drawView();

private:
	Screen _screen;
};
