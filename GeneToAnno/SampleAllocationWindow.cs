using System;
using Gtk;
using System.Collections.Generic;

namespace GeneToAnno
{
	public partial class SampleAllocationWindow : Gtk.Window
	{
		private List<PreSampleDefinition> preSDL;
		private List<string> availableIDs;
		private List<EntryDefPair> entries;

		public SampleAllocationWindow (List<PreSampleDefinition> pres) :
			base (Gtk.WindowType.Toplevel)
		{
			preSDL = pres;
			availableIDs = MainData.GetAllCurrentSampleIDs ();
			this.Build ();
			GenerateForm ();
		}

		private void GenerateForm()
		{
			entries = new List<EntryDefPair> ();
			VBox Vboc = new VBox (false, 5);

			Vboc.PackStart (new Label ("Create and allocate sample IDs for the incoming file(s): "), false, true, 10);

			foreach(PreSampleDefinition pre in preSDL)
			{
				HBox HBoc = new HBox (true, 5);
				ComboBoxEntry ent = new ComboBoxEntry (availableIDs.ToArray ());

				Label lab = new Label (MainData.MainWindow.MaxLenString(pre.ToString (), 20));
				HBoc.PackStart (lab, false, false, 5);
				HBoc.PackStart (new HSeparator (), true, true, 0);
				HBoc.PackStart (ent, false, false, 5);
				Vboc.PackStart (HBoc, false, false, 3);

				entries.Add (new EntryDefPair (ent.Entry, pre));
			}

			formMainWindow.AddWithViewport (Vboc);
		}

		protected void OnAccept (object sender, EventArgs e)
		{
			bool success = true;
			Dictionary<string, PreSampleDefinition> defsToSend = new Dictionary<string, PreSampleDefinition> ();

			foreach (EntryDefPair edp in entries) {
				if (edp.Ent.Text.Length > 0) {
					if (!defsToSend.ContainsKey (edp.Ent.Text)) {
						defsToSend.Add (edp.Ent.Text, new PreSampleDefinition (edp.Pres, edp.Ent.Text));
					} else {
						defsToSend [edp.Ent.Text].MergeSamp (edp.Pres);
					}
				} else {
					success = false;
				}
			}
			if (success) {
				this.Destroy ();
				Gtk.Application.Invoke (delegate {
					MainData.SendSampleDefsToSamples (new List<PreSampleDefinition> (defsToSend.Values));
				}
				);
			} else {
				MainData.ShowMessageWindow ("You need a sample ID for each set of information!", false);
			}
		}

		protected void OnCancel (object sender, EventArgs e)
		{
			this.Destroy ();
		}

		public class EntryDefPair
		{
			public Entry Ent;
			public PreSampleDefinition Pres;

			public EntryDefPair(Entry e, PreSampleDefinition p)
			{
				Ent = e;
				Pres = p;
			}
		}
	}
}

