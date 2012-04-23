FreeTherm.exe: FreeTherm.cs FreeTherm.glade.xml Makefile
	dmcs -target:winexe -resource:FreeTherm.glade.xml,FreeTherm.glade -resource:diy.php.png,diy.php.png -pkg:gtk-sharp-2.0,glade-sharp-2.0 -out:FreeTherm.exe FreeTherm.cs

clean:
	rm -f FreeTherm.exe
