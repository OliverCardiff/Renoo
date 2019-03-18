
// This file has been generated by the GUI designer. Do not modify.
namespace GeneToAnno
{
	public partial class SavePdfWindow
	{
		private global::Gtk.VBox vbox8;
		
		private global::Gtk.HBox hbox12;
		
		private global::Gtk.Button selectLocation;
		
		private global::Gtk.Entry fileEntry;
		
		private global::Gtk.HBox hbox11;
		
		private global::Gtk.VBox vbox9;
		
		private global::Gtk.VBox vbox11;
		
		private global::Gtk.HBox hbox13;
		
		private global::Gtk.Label label18;
		
		private global::Gtk.HSeparator hseparator6;
		
		private global::Gtk.Entry xScaleEntry;
		
		private global::Gtk.HBox hbox14;
		
		private global::Gtk.Label label19;
		
		private global::Gtk.HSeparator hseparator5;
		
		private global::Gtk.Entry yScaleEntry;
		
		private global::Gtk.VBox vbox1;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.HSeparator hseparator1;
		
		private global::Gtk.Entry titleEntry;
		
		private global::Gtk.HBox hbox2;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.HSeparator hseparator2;
		
		private global::Gtk.Entry xAxisEntry;
		
		private global::Gtk.HBox hbox3;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.HSeparator hseparator4;
		
		private global::Gtk.Entry yAxisEntry;
		
		private global::Gtk.HBox hbox10;
		
		private global::Gtk.Button cancelBut;
		
		private global::Gtk.HSeparator hseparator3;
		
		private global::Gtk.Button saveBut;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget GeneToAnno.SavePdfWindow
			this.Name = "GeneToAnno.SavePdfWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Save PDF...");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child GeneToAnno.SavePdfWindow.Gtk.Container+ContainerChild
			this.vbox8 = new global::Gtk.VBox ();
			this.vbox8.Name = "vbox8";
			this.vbox8.Spacing = 6;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox12 = new global::Gtk.HBox ();
			this.hbox12.Name = "hbox12";
			this.hbox12.Spacing = 6;
			// Container child hbox12.Gtk.Box+BoxChild
			this.selectLocation = new global::Gtk.Button ();
			this.selectLocation.CanFocus = true;
			this.selectLocation.Name = "selectLocation";
			this.selectLocation.UseUnderline = true;
			this.selectLocation.BorderWidth = ((uint)(5));
			this.selectLocation.Label = global::Mono.Unix.Catalog.GetString ("Select Location");
			this.hbox12.Add (this.selectLocation);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox12 [this.selectLocation]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hbox12.Gtk.Box+BoxChild
			this.fileEntry = new global::Gtk.Entry ();
			this.fileEntry.CanFocus = true;
			this.fileEntry.Name = "fileEntry";
			this.fileEntry.IsEditable = false;
			this.fileEntry.InvisibleChar = '•';
			this.hbox12.Add (this.fileEntry);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox12 [this.fileEntry]));
			w2.Position = 1;
			w2.Padding = ((uint)(5));
			this.vbox8.Add (this.hbox12);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox12]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox11 = new global::Gtk.HBox ();
			this.hbox11.Name = "hbox11";
			this.hbox11.Spacing = 6;
			// Container child hbox11.Gtk.Box+BoxChild
			this.vbox9 = new global::Gtk.VBox ();
			this.vbox9.Name = "vbox9";
			this.vbox9.Spacing = 6;
			// Container child vbox9.Gtk.Box+BoxChild
			this.vbox11 = new global::Gtk.VBox ();
			this.vbox11.Name = "vbox11";
			this.vbox11.Spacing = 6;
			// Container child vbox11.Gtk.Box+BoxChild
			this.hbox13 = new global::Gtk.HBox ();
			this.hbox13.Name = "hbox13";
			this.hbox13.Spacing = 6;
			// Container child hbox13.Gtk.Box+BoxChild
			this.label18 = new global::Gtk.Label ();
			this.label18.Name = "label18";
			this.label18.Xpad = 5;
			this.label18.Ypad = 5;
			this.label18.LabelProp = global::Mono.Unix.Catalog.GetString ("X - Scale");
			this.hbox13.Add (this.label18);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox13 [this.label18]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child hbox13.Gtk.Box+BoxChild
			this.hseparator6 = new global::Gtk.HSeparator ();
			this.hseparator6.Name = "hseparator6";
			this.hbox13.Add (this.hseparator6);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox13 [this.hseparator6]));
			w5.Position = 1;
			// Container child hbox13.Gtk.Box+BoxChild
			this.xScaleEntry = new global::Gtk.Entry ();
			this.xScaleEntry.WidthRequest = 30;
			this.xScaleEntry.CanFocus = true;
			this.xScaleEntry.Name = "xScaleEntry";
			this.xScaleEntry.IsEditable = true;
			this.xScaleEntry.InvisibleChar = '•';
			this.hbox13.Add (this.xScaleEntry);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox13 [this.xScaleEntry]));
			w6.Position = 2;
			w6.Padding = ((uint)(5));
			this.vbox11.Add (this.hbox13);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox11 [this.hbox13]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox11.Gtk.Box+BoxChild
			this.hbox14 = new global::Gtk.HBox ();
			this.hbox14.Name = "hbox14";
			this.hbox14.Spacing = 6;
			// Container child hbox14.Gtk.Box+BoxChild
			this.label19 = new global::Gtk.Label ();
			this.label19.Name = "label19";
			this.label19.Xpad = 5;
			this.label19.Ypad = 5;
			this.label19.LabelProp = global::Mono.Unix.Catalog.GetString ("Y - Scale");
			this.hbox14.Add (this.label19);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox14 [this.label19]));
			w8.Position = 0;
			w8.Expand = false;
			w8.Fill = false;
			// Container child hbox14.Gtk.Box+BoxChild
			this.hseparator5 = new global::Gtk.HSeparator ();
			this.hseparator5.Name = "hseparator5";
			this.hbox14.Add (this.hseparator5);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox14 [this.hseparator5]));
			w9.Position = 1;
			// Container child hbox14.Gtk.Box+BoxChild
			this.yScaleEntry = new global::Gtk.Entry ();
			this.yScaleEntry.WidthRequest = 30;
			this.yScaleEntry.CanFocus = true;
			this.yScaleEntry.Name = "yScaleEntry";
			this.yScaleEntry.IsEditable = true;
			this.yScaleEntry.InvisibleChar = '•';
			this.hbox14.Add (this.yScaleEntry);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox14 [this.yScaleEntry]));
			w10.Position = 2;
			w10.Padding = ((uint)(5));
			this.vbox11.Add (this.hbox14);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox11 [this.hbox14]));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vbox11.Gtk.Box+BoxChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xpad = 5;
			this.label1.Ypad = 5;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Plot Title");
			this.hbox1.Add (this.label1);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.label1]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.hseparator1 = new global::Gtk.HSeparator ();
			this.hseparator1.Name = "hseparator1";
			this.hbox1.Add (this.hseparator1);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.hseparator1]));
			w13.Position = 1;
			// Container child hbox1.Gtk.Box+BoxChild
			this.titleEntry = new global::Gtk.Entry ();
			this.titleEntry.WidthRequest = 35;
			this.titleEntry.CanFocus = true;
			this.titleEntry.Name = "titleEntry";
			this.titleEntry.IsEditable = true;
			this.titleEntry.InvisibleChar = '•';
			this.hbox1.Add (this.titleEntry);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.titleEntry]));
			w14.Position = 2;
			this.vbox1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox1]));
			w15.Position = 0;
			w15.Expand = false;
			w15.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xpad = 5;
			this.label2.Ypad = 5;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("X-Axis Label");
			this.hbox2.Add (this.label2);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.label2]));
			w16.Position = 0;
			w16.Expand = false;
			w16.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.hseparator2 = new global::Gtk.HSeparator ();
			this.hseparator2.WidthRequest = 1;
			this.hseparator2.Name = "hseparator2";
			this.hbox2.Add (this.hseparator2);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.hseparator2]));
			w17.Position = 1;
			// Container child hbox2.Gtk.Box+BoxChild
			this.xAxisEntry = new global::Gtk.Entry ();
			this.xAxisEntry.WidthRequest = 55;
			this.xAxisEntry.CanFocus = true;
			this.xAxisEntry.Name = "xAxisEntry";
			this.xAxisEntry.IsEditable = true;
			this.xAxisEntry.InvisibleChar = '•';
			this.hbox2.Add (this.xAxisEntry);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.xAxisEntry]));
			w18.Position = 2;
			this.vbox1.Add (this.hbox2);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox2]));
			w19.Position = 1;
			w19.Expand = false;
			w19.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xpad = 5;
			this.label3.Ypad = 5;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Y-Axis Label");
			this.hbox3.Add (this.label3);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.label3]));
			w20.Position = 0;
			w20.Expand = false;
			w20.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.hseparator4 = new global::Gtk.HSeparator ();
			this.hseparator4.WidthRequest = 1;
			this.hseparator4.Name = "hseparator4";
			this.hbox3.Add (this.hseparator4);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.hseparator4]));
			w21.Position = 1;
			// Container child hbox3.Gtk.Box+BoxChild
			this.yAxisEntry = new global::Gtk.Entry ();
			this.yAxisEntry.WidthRequest = 55;
			this.yAxisEntry.CanFocus = true;
			this.yAxisEntry.Name = "yAxisEntry";
			this.yAxisEntry.IsEditable = true;
			this.yAxisEntry.InvisibleChar = '•';
			this.hbox3.Add (this.yAxisEntry);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.yAxisEntry]));
			w22.Position = 2;
			this.vbox1.Add (this.hbox3);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox3]));
			w23.Position = 2;
			w23.Expand = false;
			w23.Fill = false;
			this.vbox11.Add (this.vbox1);
			global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.vbox11 [this.vbox1]));
			w24.Position = 2;
			w24.Expand = false;
			w24.Fill = false;
			this.vbox9.Add (this.vbox11);
			global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.vbox9 [this.vbox11]));
			w25.Position = 0;
			w25.Expand = false;
			w25.Fill = false;
			this.hbox11.Add (this.vbox9);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.hbox11 [this.vbox9]));
			w26.Position = 0;
			this.vbox8.Add (this.hbox11);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox11]));
			w27.Position = 1;
			w27.Expand = false;
			w27.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox10 = new global::Gtk.HBox ();
			this.hbox10.Name = "hbox10";
			this.hbox10.Spacing = 6;
			// Container child hbox10.Gtk.Box+BoxChild
			this.cancelBut = new global::Gtk.Button ();
			this.cancelBut.CanFocus = true;
			this.cancelBut.Name = "cancelBut";
			this.cancelBut.UseUnderline = true;
			this.cancelBut.BorderWidth = ((uint)(5));
			this.cancelBut.Label = global::Mono.Unix.Catalog.GetString ("Cancel");
			this.hbox10.Add (this.cancelBut);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(this.hbox10 [this.cancelBut]));
			w28.Position = 0;
			w28.Expand = false;
			w28.Fill = false;
			// Container child hbox10.Gtk.Box+BoxChild
			this.hseparator3 = new global::Gtk.HSeparator ();
			this.hseparator3.Name = "hseparator3";
			this.hbox10.Add (this.hseparator3);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.hbox10 [this.hseparator3]));
			w29.Position = 1;
			// Container child hbox10.Gtk.Box+BoxChild
			this.saveBut = new global::Gtk.Button ();
			this.saveBut.WidthRequest = 30;
			this.saveBut.CanFocus = true;
			this.saveBut.Name = "saveBut";
			this.saveBut.UseUnderline = true;
			this.saveBut.BorderWidth = ((uint)(5));
			this.saveBut.Label = global::Mono.Unix.Catalog.GetString ("Save");
			this.hbox10.Add (this.saveBut);
			global::Gtk.Box.BoxChild w30 = ((global::Gtk.Box.BoxChild)(this.hbox10 [this.saveBut]));
			w30.Position = 2;
			this.vbox8.Add (this.hbox10);
			global::Gtk.Box.BoxChild w31 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox10]));
			w31.Position = 2;
			w31.Expand = false;
			w31.Fill = false;
			this.Add (this.vbox8);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 254;
			this.Show ();
			this.selectLocation.Clicked += new global::System.EventHandler (this.OnSelectLocation);
			this.cancelBut.Clicked += new global::System.EventHandler (this.OnCancel);
			this.saveBut.Clicked += new global::System.EventHandler (this.OnSave);
		}
	}
}
