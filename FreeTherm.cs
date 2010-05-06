/*	FreeTherm.cs

	Copyright 2008 Fred Cooke

	This file is part of the FreeEMS project.

	FreeEMS software is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	FreeEMS software is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with any FreeEMS software.  If not, see <http://www.gnu.org/licenses/>.

	We ask that if you make any changes to this file you send them upstream to us at admin@diyefi.org

	Thank you for choosing FreeEMS to run your engine! */

using System;
using Gtk;
using Gdk;
using Glade;

public class FreeTherm {
	/* Include all widgets here : */

	[Glade.Widget] Gtk.Window MainWindow;
	[Glade.Widget] Gtk.AboutDialog AboutFreeThermDialog;

	[Glade.Widget] Gtk.ImageMenuItem SaveMenuItem;
	[Glade.Widget] Gtk.ImageMenuItem AboutMenuItem;
	[Glade.Widget] Gtk.ImageMenuItem QuitMenuItem;
	
	[Glade.Widget] Gtk.Entry LowDegrees;
	[Glade.Widget] Gtk.Entry LowOhms;

	/* Fred Cooke 18 June 2008 */
	public static void Main (string[] args)
	{
		new FreeTherm (args);
	}

	/* Fred Cooke 18 June 2008 */
	public FreeTherm (string[] args) {
		try {
			Application.Init();
			
			Glade.XML MainXML =  new Glade.XML(null, "freetherm.glade", "MainWindow", null);
			MainXML.Autoconnect (this);
			
			/* Setup handlers here : */
			SaveMenuItem.Activated += SaveFile;	// Generate file output when user chooses "save".
			AboutMenuItem.Activated += About;	// Open about dialog when user chooses "about".
			QuitMenuItem.Activated += Quit;		// Exit when user chooses "quit".
			MainWindow.Hidden += Quit;			// Exit when the main window is closed.

			Application.Run();
		} catch(Exception e) {
			Console.WriteLine("The application failed to start : ", e);
		}
	}

	/* Put event handlers here : */

	public void SaveFileAs(object o, EventArgs args){}
	public void Cut(object o, EventArgs args){}
	public void Copy(object o, EventArgs args){}
	public void Paste(object o, EventArgs args){}
	public void Delete(object o, EventArgs args){}

	public void SaveFile(object o, EventArgs args)
	{
		Console.WriteLine("Low Ohms = " + LowOhms.Text + " and Low Temp = " + LowDegrees.Text);
	}

	/* Fred Cooke 18 June 2008 */
	public void Quit(object o, EventArgs args)
	{
		Environment.Exit(0); // Shut the program down without preserving any state.
	}

	/* Fred Cooke 18 June 2008 */
	public void About(object o, EventArgs args)	{
		try {
			/* Load the window from the embedded glade file. */
			Glade.XML AboutXML = new Glade.XML(null, "freetherm.glade", "AboutFreeThermDialog", null);
			AboutXML.Autoconnect(this);
		
			if(AboutFreeThermDialog == null) {
				Console.WriteLine("Why is AboutFreeThermDialog null?");
			} else {
				AboutFreeThermDialog.Logo = new Gdk.Pixbuf(null, "diy.php.png");	// Load the embedded resource into the dialog.
				AboutFreeThermDialog.TransientFor = MainWindow;
				AboutFreeThermDialog.Run();
				AboutFreeThermDialog.Destroy();
			}
		} catch(Exception e) {
			Console.WriteLine("Exception :", e);
		}
	}
}