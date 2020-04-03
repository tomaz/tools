#pragma once

#include <iostream>
#include <functional>
#include <pdcwin.h>

/// Lightweight PDCurses wrapper.
///
/// Mainly to simplify most repetitive tasks. To use create an instance and use helper functions on it.
class Screen {
public:
	Screen();
	~Screen();

public:
	/// Starts a new screen.
	void start();

	/// Ends current screen. If screen wasn't started, nothing happens, so safe to call multiple times.
	void end();

public:
	/// Enables or disables cursor display.
	void enableCursor(bool enable = true) const;

	/// Registers given color pair to given slot.
	void registerColors(short slot, short foreground, short background) const;

	/// Applies given attributes, calls given handler where caller can draw text, and removes applied attributes.
	void attributed(chtype attributes, std::function<void(const Screen &)> handler) const;

	/// Wraps drawing functions by enclosing them with refresh.
	void draw(std::function<void(const Screen &)> handler) const;

	/// Wraps drawing functions where cursor position is restored after given lamda returns.
	void restorePosition(std::function<void(const Screen &)> handler) const;

public:
	/// Returns current x coordinate.
	int currentX() const;

	/// Returns current y coordinate.
	int currentY() const;

	/// Returns width of the screen in columns.
	int width() const;

	/// Returns height of the screen in rows.
	int height() const;

private:
	int _width;
	int _height;
	bool _isStarted;
	WINDOW *_window;
};

#pragma region Helper functions

/// Prints the given string until end of line, repeating as many times as needed and cutting last occurence to fit.
void printeol(std::string s);

/// Prints std::string at the current coordinates.
void printstd(std::string s);

/// Prints std::wstring at the current coordinates.
void printstdw(std::wstring s);

/// Prints std::wstring at the given coordinates.
void mvprintstdw(int y, int x, std::wstring s);

/// Prints std::wstring centered on screen on the given y coordinate.
void mvprintstdwc(int y, std::wstring s);

/// Prints std::string at the given coordinates.
void mvprintstd(int y, int x, std::string s);

/// Prints std::string centered on screen on the given y coordinate.
void mvprintstdc(int y, std::string s);

/// Prints given C string centered on screen on the given y coordinate.
void mvprintc(int y, const char *s);

#pragma endregion
