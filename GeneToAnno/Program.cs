using System;
using Gtk;

namespace GeneToAnno
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			MainData.Assemble ();
			Application.Init ();
			WindowMain win = new WindowMain ();
			win.Show ();
			Application.Run ();
		}
	}
}
