/*
// This file has been generated by the GUI designer. Do not modify.
namespace GeneToAnno
{
	public partial class SetElementTypeDictWindow
	{
		private global::Gtk.VBox vbox2;
		
		private global::Gtk.ScrolledWindow labWindow;
		
		private global::Gtk.HBox hbox4;
		
		private global::Gtk.Button button2;
		
		private global::Gtk.HSeparator hseparator4;
		
		private global::Gtk.Button button1;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget GeneToAnno.SetElementTypeDictWindow
			this.Name = "GeneToAnno.SetElementTypeDictWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Select Annotation Elements");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child GeneToAnno.SetElementTypeDictWindow.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.labWindow = new global::Gtk.ScrolledWindow ();
			this.labWindow.CanFocus = true;
			this.labWindow.Name = "labWindow";
			this.labWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			this.vbox2.Add (this.labWindow);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.labWindow]));
			w1.Position = 0;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox4 = new global::Gtk.HBox ();
			this.hbox4.Name = "hbox4";
			this.hbox4.Spacing = 6;
			// Container child hbox4.Gtk.Box+BoxChild
			this.button2 = new global::Gtk.Button ();
			this.button2.CanFocus = true;
			this.button2.Name = "button2";
			this.button2.UseUnderline = true;
			this.button2.BorderWidth = ((uint)(5));
			this.button2.Label = global::Mono.Unix.Catalog.GetString ("Cancel");
			this.hbox4.Add (this.button2);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.button2]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			w2.Padding = ((uint)(10));
			// Container child hbox4.Gtk.Box+BoxChild
			this.hseparator4 = new global::Gtk.HSeparator ();
			this.hseparator4.Name = "hseparator4";
			this.hbox4.Add (this.hseparator4);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.hseparator4]));
			w3.Position = 1;
			// Container child hbox4.Gtk.Box+BoxChild
			this.button1 = new global::Gtk.Button ();
			this.button1.CanFocus = true;
			this.button1.Name = "button1";
			this.button1.UseUnderline = true;
			this.button1.BorderWidth = ((uint)(5));
			this.button1.Label = global::Mono.Unix.Catalog.GetString ("Accept");
			this.hbox4.Add (this.button1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.button1]));
			w4.Position = 2;
			w4.Expand = false;
			w4.Fill = false;
			w4.Padding = ((uint)(10));
			this.vbox2.Add (this.hbox4);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox4]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 300;
			this.Show ();
			this.button2.Clicked += new global::System.EventHandler (this.OnCancel);
			this.button1.Clicked += new global::System.EventHandler (this.OnAccept);
		}
	}
}
*/