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

using Gtk;
using Gdk;
using Glade;
using System;
using System.IO;

namespace FreeTherm {

	/// <summary>
	///
	/// FreeTherm is a simple code generator that creates an include file with
	/// an array of temperature values indexable by the ADC input on the target.
	/// 
	/// It does this based on a number of inputs specifying its format and
	/// processing conditions and parameters to ensure maximum flexibility.
	/// 
	/// By Fred Cooke and Shameem Hameed.
	///
	/// See www.diyefi.org for more information on the FreeEMS project.
	///
	/// </summary>

	public class FreeTherm {

		/* Include all widgets here : */
		#region Glade Widgets
		
		[Glade.Widget]Gtk.Window MainWindow;
		[Glade.Widget]Gtk.AboutDialog AboutFreeThermDialog;

//		[Glade.Widget]Gtk.ImageMenuItem SaveMenuItem;
//		[Glade.Widget]Gtk.ImageMenuItem AboutMenuItem;
//		[Glade.Widget]Gtk.ImageMenuItem QuitMenuItem;
	
		[Glade.Widget]Gtk.Entry LowDegrees;
		[Glade.Widget]Gtk.Entry LowOhms;
        [Glade.Widget]Gtk.Entry MidDegrees;
        [Glade.Widget]Gtk.Entry MidOhms;
        [Glade.Widget]Gtk.Entry HighDegrees;
        [Glade.Widget]Gtk.Entry HighOhms;
        [Glade.Widget]Gtk.Entry BiasOhms;
        [Glade.Widget]Gtk.Entry Filename;
		[Glade.Widget]Gtk.Entry ArrayName;		

        [Glade.Widget]Gtk.RadioButton CelciusIn;
        [Glade.Widget]Gtk.RadioButton KelvinIn;
        [Glade.Widget]Gtk.RadioButton FahrenheitIn;
		
		#endregion

        #region Globals

		// Defaults for common NipponDenso sensor.
        double lowTemp = 0.0;
		double lowRes = 6540.0;
		
        double midTemp = 25.5;
		double midRes = 2040.0;
		
        double highTemp = 80.0;
		double highRes = 340.0;

        double biasRes = 2200.0;
		
		string defaultFilename = "IATTransferTable.c";
		
		// enum for these ?
        const int CELCIUS = 0;
        const int KELVIN=1;
        const int FAHRENHEIT=2;

		int outputFactor = 100;
        int inputTempScale = CELCIUS;
		int outputTempScale = KELVIN;

        double A = 0, B = 0, C = 0;

        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		public static void Main (string[] args)
		{
			new FreeTherm (args);
		}

		/* Fred Cooke 18 June 2008 */
		public FreeTherm (string[] args) {
			try {
				Application.Init();
			
				Glade.XML MainXML =  new Glade.XML(null, "FreeTherm.glade", "MainWindow", null);
				MainXML.Autoconnect (this);
			
				init(); // load all the fields with values from globals for now, but perhaps a user config later?
				
				Application.Run();
			} catch(Exception e) {
				Console.WriteLine("The application failed to start : " + e.Message + "\nThe StackTrace was : " + e.StackTrace);
			}
		}
	
        // Connect the Signals defined in Glade
        public void on_MainWindow_delete_event(object o, DeleteEventArgs args)
        {
            Application.Quit();
            args.RetVal = true;
        }
		
		private void init(){
			BiasOhms.Text = biasRes.ToString();
			
			LowOhms.Text = lowRes.ToString();
			LowDegrees.Text = lowTemp.ToString();

			MidOhms.Text = midRes.ToString();
			MidDegrees.Text = midTemp.ToString();

			HighOhms.Text = highRes.ToString();
			HighDegrees.Text = highTemp.ToString();

			Filename.Text = defaultFilename;

}
			
        #region Custom Functions

        public void calculateSHCoefficients()
        {
			// Convert input temps to output temp scale
			double lT,mT,hT;
			
			double KelvinOffset = 273.15;
			
			if (inputTempScale == outputTempScale) {
				lT = lowTemp;
				mT = midTemp;
				hT = highTemp;
			} else if (inputTempScale == CELCIUS && outputTempScale == KELVIN) {
				lT = lowTemp + KelvinOffset;
				mT = midTemp + KelvinOffset;
				hT = highTemp + KelvinOffset;
			} else if (inputTempScale == KELVIN && outputTempScale == CELCIUS) {
				lT = lowTemp - KelvinOffset;
				mT = midTemp - KelvinOffset;
				hT = highTemp - KelvinOffset;
			} else {
				Console.WriteLine("You are living in the past, write your own code :-)");
				return;
			}

            double x1 = 1 / (lT);
            double x2 = 1 / (mT);
            double x3 = 1 / (hT);
            double y1 = Math.Log(lowRes);
            double y2 = Math.Log(midRes);
            double y3 = Math.Log(highRes);
            double y1c = Math.Pow(y1, 3);
            double y2c = Math.Pow(y2, 3);
            double y3c = Math.Pow(y3, 3);

            C = (((y2 - y1) * (x3 - x1)) - ((y3 - y1) * (x2 - x1))) / (((y3c - y1c) * (y2 - y1)) - ((y2c - y1c) * (y3 - y1)));
            B = ((x2 - x1) - (C * (y2c - y1c))) / (y2 - y1);
            A = x1 - B * y1 - C * y1c;
            return;
        }

        public double numberVerify(Entry o, double number) {			
            if (o.Text != "") {
				try {
					return Double.Parse(o.Text);
				} catch {
					o.Text = number.ToString();
					return number;
				}
            } else {
				return 0.0;
			}
        }

        #endregion

        #region Text Entry handlers

        public void on_LowDegrees_changed(object o, EventArgs args)
        {
            lowTemp = numberVerify(LowDegrees, lowTemp);
            return;
        }

        public void on_MidDegrees_changed(object o, EventArgs args)
        {
            midTemp = numberVerify(MidDegrees, midTemp);
            return;
        }

        public void on_HighDegrees_changed(object o, EventArgs args)
        {
            highTemp = numberVerify(HighDegrees, highTemp);
            return;
        }


        public void on_LowOhms_changed(object o, EventArgs args)
        {
            lowRes = numberVerify(LowOhms, lowRes);
            return;
        }

        public void on_MidOhms_changed(object o, EventArgs args)
        {
            midRes = numberVerify(MidOhms, midRes);
            return;
        }

        public void on_HighOhms_changed(object o, EventArgs args)
        {
            highRes = numberVerify(HighOhms, highRes);
            return;
        }

        public void on_BiasOhms_changed(object o, EventArgs args)
        {
            biasRes = numberVerify(BiasOhms, biasRes);
            return;
        }
        
        #endregion

        #region Radio Button handlers

        public void on_CelciusIn_toggled(object o, EventArgs args)
        {
            if (CelciusIn.Active)
                inputTempScale = CELCIUS;
            return;
        }

        public void on_KelvinIn_toggled(object o, EventArgs args)
        {
            if (KelvinIn.Active)
                inputTempScale = KELVIN;
            return;
        }

        public void on_FahrenheitIn_toggled(object o, EventArgs args)
        {
            if (FahrenheitIn.Active)
                inputTempScale = FAHRENHEIT;
            return;
        }

        #endregion

        #region Menu item handlers

        public void on_SaveMenuItem_activate(object o, EventArgs args)
        {
			Gtk.FileChooserDialog FCD = new Gtk.FileChooserDialog("Save...", MainWindow, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			FCD.CurrentName = Filename.Text;
			FCD.DoOverwriteConfirmation = true;

			int Response = FCD.Run();
			FCD.Hide();

            if (Response == (int)ResponseType.Accept)
			{
				String filename = FCD.Filename;				
				FileStream file = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
				file.SetLength(0);
				file.Flush();
				StreamWriter writer = new StreamWriter(file);

				calculateSHCoefficients();

				int Elements = 1024;
				
				writer.WriteLine("const " + ArrayName.Text + "[" + Elements.ToString("0000") + "] {");
				
				for (int loop = 0; loop < Elements; loop++)
				{
					double temp;
					if (loop == 0) {
						temp = double.MaxValue;
					} else {
						double res = biasRes / ( ( 1.0 / ( (double) loop / (double) Elements) ) - 1.0 ); 
						double y = Math.Log(res);
						double yc = Math.Pow(y, 3);
						temp = 1 / (A + B * y + C * yc);
					}
					
					temp *= outputFactor;
					if (temp > 65535) {
						temp = 65535;
					}
					
					string comma = ",";
					if (loop == (Elements -1)) {
						comma = "";
					}

					writer.WriteLine(temp.ToString("00000") + comma);
				}

				writer.WriteLine("}\n");
				
				writer.Close();
			}
			FCD.Destroy();
			return;
		}
		
		public void on_QuitMenuItem_activate(object o, EventArgs args)
		{
			Application.Quit();
			return;
		}
		
		public void on_AboutMenuItem_activate(object o, EventArgs args)
		{
			try {
				/* Load the window from the embedded glade file. */
				Glade.XML AboutXML = new Glade.XML(null, "FreeTherm.glade", "AboutFreeThermDialog", null);
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
				Console.WriteLine("About dialog exception : " + e.Message + "/nAbout dialog stacktrace : " + e.StackTrace);
			}
        }

        #endregion

        // Common functions used by buttons and menu items
	}
}
