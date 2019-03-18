using System;
using Gtk;
using Gtk.DotNet;
using System.Collections.Generic;
using System.Drawing;

namespace GeneToAnno
{
	public class ImgDisplay
	{
		DrawingArea MainArea;
		Dictionary<string, Bitmap> images;

		public ImgDisplay (DrawingArea draw)
		{
			MainArea = draw;
			images = new Dictionary<string, Bitmap> ();
		}

		public void RecieveImages(List<Bitmap> bmps, List<string> names)
		{
			images.Clear ();

			int cnt = bmps.Count;

			for (int i = 0; i < cnt; i++) {
				images.Add (names [i], bmps [i]);
			}

		}

		public void SwitchImages()
		{
			MainArea.QueueDraw ();
		}

		public void DrawToSpace(string name, ExposeEventArgs args)
		{
			Bitmap bm = images [name];
			Gdk.Window window = args.Event.Window;
			int xs = 0; int ys = 0;
			args.Event.Window.GetSize (out xs, out ys);
			if (xs < bm.Width) {
				xs = bm.Width;
			}
			if (ys < bm.Height) {
				ys = bm.Height;
			}
			args.Event.Window.Resize (xs, ys);

			if (images.Count > 0) {
				Rectangle rect = new Rectangle (0, 0, bm.Width, bm.Height);

				using (System.Drawing.Graphics graphics = Gtk.DotNet.Graphics.FromDrawable (window)) {
					graphics.DrawImage (bm, rect);
				}
			}
		}

		public void SaveToFile(string name, Window w)
		{
			Bitmap bm = images [name];

			FileChooserDialog saveIt = new FileChooserDialog(
				"Where should the image be saved?..",
				w,
				FileChooserAction.Save,
				"Cancel", ResponseType.Cancel,
				"Save", ResponseType.Accept);

			if (saveIt.Run () == (int)ResponseType.Accept) {
				bm.Save (saveIt.Filename);
			}

			saveIt.Destroy ();
		}
	}
}

