FreeTherm.exe: FreeTherm.cs freetherm.glade
	mcs -resource:freetherm.glade -pkg:gtk-sharp -pkg:glade-sharp -out:FreeTherm.exe FreeTherm.cs

clean:
	rm -f *.bak *.exe
