# What?

Various command line tools.

Projects are written in different languages as I wanted to explore various approaches. Initially went with Ruby to keep it as cross-platform as possible, but found maintaining gems messy. Then tried C++, my first OOP language I still keep in fond memory; used to program Windows and embedded apps in it for many years, so wanted to checkout various new features the language got since I used it. It went well, though after developing in higher level languages since ~2000, found it too complicated with no additional benefit, especially in light of sharing the code with other people. So I quickly followed my transition to C# from back in time... So at the moment tools are written in different languages, but plan is to convert them all to C# and .NET Core eventually. changes to existing ones.

Code is available under MIT license.

## Kids

Gamification tool for helping kids learn various subjects in school. Primarily maths related. This is growing list, I add modules as my kids learn different subjects and we run out of imagination to compile lists after lists of equations for them to solve :) At the moment it includes:

- Unit conversion: creates equations for converting measures from one unit to another
- Words: encoragues kids to learn new words through game of hangman

Note: while code is written in English, texts are Slovene. At the moment there is no support for different languages. Should be interesting excercise though - an idea for PR. Also open for PR with new tools. Either way: definitely contact me first for discussion before doing any work to avoid costly PR refactoring.

Language: C#

## CalendarCreator

Takes a list of events in simple text format and converts it to `.ics` file suitable for importing into calendar app as series of events. Examples of source files are included in `Sample` folder.

Language: C#

## Movies

SRT subtitles tweaking for changing timestamps etc.

Language: Ruby (will be converted to C# in the future)


# Contribution

Contribution is welcome! While reporting issues is fine, I encourage more active participation with pull requests. If you notice an issue, submit PR with fix; doesn't matter if it's just a single typo fix, if you noticed it, you're more than qualified to fix it! Taking responsibility is much more empowering than opening an issue and requesting someone else to fix, isn't it? It doesn't matter what your level of expertise is, this can be a nice learning tool even for beginners.

If you want to contribute but don't know where to start, converting existing projects to C# would be a good candidate. Also adding additional modules to `Kids` project, or implementing multi language support. If unsure, feel free to contact me (for example DM [@tomsbarks](https://twitter.com/tomsbarks)) for discussion up front.

I will require code formatting and style to follow existing conventions (feel free to import settings from included `project.editorconfig` file). I'm also super OCD about architecture; I do mu best to follow good practices even for these type of home projects. And I will require the same from PR. But don't feel intimidated, I am "extremely" cooperative and good manered (if I may say so myself :) so will discuss all issues I find in a constructive way!


# License

Copyright 2020, Tomaz Kragelj

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.