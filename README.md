# FreeTherm

FreeTherm is a simple utility to generate thermistor curves written in C#. It
is developed under Debian Linux using the mono implementation of the language.

This project uses GTK# for the GUI toolkit, therefore you'll need both to run it.

### Debian

For Debian and Ubuntu you need to install the following libraries to run this
application.

	apt-get install libmono-corlib2.0-cil libgtk2.0-cil libglade2.0-cil

For development, which is done on Debian, you just add -dev to each package.

	apt-get install libmono-corlib2.0-cil-dev libgtk2.0-cil-dev libglade2.0-cil-dev mono-mcs

These instructions may be incomplete, however it's a good start.

### Windows

To run the executables on Windows you need to install the GTK libraries. You
can get them from the following links:

 - http://sourceforge.net/projects/gtk-win/
 - http://www.go-mono.com/mono-downloads/download.html

With GTK# and GTK 2.0 runtime libs installed you should be able to run the app.

