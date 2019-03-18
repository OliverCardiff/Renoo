using System;
using OxyPlot;
using Gtk;
using System.IO;

namespace GeneToAnno
{
	public partial class SavePdfWindow : Gtk.Window
	{
		PlotModel model;

		public SavePdfWindow (PlotModel mod) :
			base (Gtk.WindowType.Toplevel)
		{
			model = mod;
			this.Build ();
			xAxisEntry.Text = mod.Axes [0].Title;
			yAxisEntry.Text = mod.Axes [1].Title;
			titleEntry.Text = mod.Title;
		}

		protected void OnCancel (object sender, EventArgs e)
		{
			this.Destroy ();
		}

		public void SaveToPdf(string fileName, double x, double y)
		{
			using(var stream = File.Create(fileName))
			{
				PdfExporter.Export (model, stream, x, y);
			}
		}

		protected void OnSave (object sender, EventArgs e)
		{
			bool success = true;
			double x, y;

			model.Axes [0].Title = xAxisEntry.Text;
			model.Axes [1].Title = yAxisEntry.Text;
			model.Title = titleEntry.Text;

			if(!double.TryParse(xScaleEntry.Text, out x))
			{
				success = false;
			}
			if(!double.TryParse(yScaleEntry.Text, out y))
			{
				success = false;
			}
			if (fileEntry.Text.Length < 2) {
				success = false;
			}

			if (!success) {
				MainData.ShowMessageWindow ("Unable to verify field entries, x & y are numbers, and you need a file location!", false);
			} else {
				SaveToPdf (fileEntry.Text, x, y);
				MainData.UpdateLog ("Success!\nSaved Pdf to: " + fileEntry.Text, false);
				this.Destroy ();
			}
		}

		protected void OnSelectLocation (object sender, EventArgs e)
		{
			FileChooserDialog dlog = new FileChooserDialog("Find a place to save the pdf: ",
				this,
				FileChooserAction.Save,
				"Cancel", ResponseType.Cancel,
				"Open", ResponseType.Accept);

			//dlog.PreviewFilename = "output.pdf";
			dlog.DoOverwriteConfirmation = true;

			if (dlog.Run () == (int)ResponseType.Accept) {
				string st = dlog.Filename;
				if (!st.Contains (".pdf")) {
					st += ".pdf";
				}
				fileEntry.Text = st;
			}

			dlog.Destroy ();
		}
	}
}

