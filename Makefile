FreeTherm.exe: FreeTherm.cs freetherm.glade Makefile
	mcs -resource:freetherm.glade,freetherm.glade -resource:diy.php.png,diy.php.png -pkg:gtk-sharp-2.0 -pkg:glade-sharp-2.0 -out:FreeTherm.exe FreeTherm.cs

clean:
	rm -f FreeTherm.exe
