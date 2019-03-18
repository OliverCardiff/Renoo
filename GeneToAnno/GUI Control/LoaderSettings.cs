using System;
using Gtk;
using System.Collections.Generic;

namespace GeneToAnno
{
	public partial class LoaderSettings : Gtk.Window
	{
		private static List<OldFiltSettings> FiltSettings;
		protected List<SettingBinder> _settings;
		protected List<ElementFilterInstruction> instructions;
		protected List<FilterEntryBinder> filtEntBind;

		public LoaderSettings () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			BindSettings ();
			AssignSettings ();
			notebook1.CurrentPage = 0;
			MakeFilterWindow ();
		}

		protected void OnMainCancel (object sender, EventArgs e)
		{
			this.Destroy ();
		}

		protected void OnAccept (object sender, EventArgs e)
		{
			string msg;
			bool canClose = true;
			foreach (SettingBinder b in _settings) {
				if (!b.ReturnSettings (out msg)) {
					MainData.ShowMessageWindow (msg, false);
					canClose = false;
				}
			}
			if (canClose) {
				this.Destroy ();
			}
		}

		protected void MakeFilterWindow()
		{
			if (ProgramState.LoadedGFF3) {
				if (FiltSettings == null) {
					FiltSettings = new List<OldFiltSettings> ();
				}

				VBox Vboc = new VBox (false, 5);

				filtEntBind = new List<FilterEntryBinder> ();
				List<string> lstrs = MainData.TypeDict.GetAllTypes ();

				HBox hb = new HBox (true, 5);
				hb.PackStart (new Label ("Element Name"), false, true, 3);
				hb.PackStart (new Label ("Min Length"), false, true, 3);
				hb.PackStart (new Label ("Max Length"), false, true, 3);
				Vboc.PackStart(hb,false, true, 3);

				foreach (string st in lstrs) {

					int minEnt = 0;
					int maxEnt = 999999;
					bool active = false;

					foreach (OldFiltSettings fl in FiltSettings) {
						if (fl.name == st) {
							minEnt = fl.min;
							maxEnt = fl.max;
							active = true;
						}
					}

					HBox Hboc = new HBox (true, 3);
					CheckButton chb = new CheckButton (st);
					chb.Active = active;
					Entry minEntry = new Entry (minEnt.ToString());
					Entry maxEntry = new Entry (maxEnt.ToString());

					filtEntBind.Add (new FilterEntryBinder (chb, minEntry, maxEntry, st));
					Hboc.PackStart (chb, false, true, 3);
					Hboc.PackStart (minEntry, false, true, 3);
					Hboc.PackStart (maxEntry, false, true, 3);
					Vboc.PackEnd (Hboc, false, true, 3);
				}

				filterWindow.AddWithViewport (Vboc);
				filterWindow.ShowAll ();
				filterRunBut.Sensitive = true;
			}
		}

		protected void OnRunFilter (object sender, EventArgs e)
		{
			bool parseSuccess = true;
			string failfield = "(";
			instructions = new List<ElementFilterInstruction> ();
			FiltSettings.Clear ();

			foreach (FilterEntryBinder feb in filtEntBind) {
				if (feb.Check.Active) {
					int ma; int mi;
					if (!int.TryParse (feb.MaxEnt.Text, out ma)) {
						parseSuccess = false;
						failfield += feb.id + "min val, ";
					}
					if (!int.TryParse (feb.MinEnt.Text, out mi)) {
						parseSuccess = false;
						failfield += feb.id + "max val, ";
					}
					if (parseSuccess) {
						FiltSettings.Add (new OldFiltSettings (feb.id, mi, ma));
						ElementFilterInstruction efil = new ElementFilterInstruction (feb.id, mi, ma);
						instructions.Add (efil);
					}
				}
			}

			if (parseSuccess) {
				MainData.RunElementFilter (instructions);
				this.Destroy ();
			} else {
				MainData.ShowMessageWindow ("The following entries were in error: " + failfield, false);
			}
		}

		protected void AssignSettings()
		{
			foreach (SettingBinder b in _settings) {
				b.FillForm ();
				b.SensitizeDependents ();
			}
		}

		protected void BindSettings()
		{
			_settings = new List<SettingBinder> ();

			SettingBinder<bool> ch1 = new SettingBinder<bool> (AppSettings.Loading.GEN_REMOVE_SUFF, genomeTrimSuffCheck);
			_settings.Add (ch1);
			ch1.AddDependent (genomeTrimSuffEntry);

			SettingBinder<bool> ch2 = new SettingBinder<bool> (AppSettings.Loading.GEN_SPLIT_ID, genomeSplitBaseIDCheck);
			_settings.Add (ch2);
			ch2.AddDependent (genomeSplitCharsEntry);
			ch2.AddDependent (genomeSplitSubsEntry);

			_settings.Add (new SettingBinder<char[]> (AppSettings.Loading.GEN_ID_SPLIT_CHARS, genomeSplitCharsEntry));
			_settings.Add (new SettingBinder<bool> (AppSettings.Loading.GEN_LITE_LOAD, genomeLiteLoadBut));
			_settings.Add (new SettingBinder<List<string>> (AppSettings.Loading.GEN_PRESUFFIX_TO_TRIM, genomeTrimSuffEntry));
			_settings.Add (new SettingBinder<int> (AppSettings.Loading.GEN_SPLIT_SUBSTR, genomeSplitSubsEntry));
			_settings.Add (new SettingBinder<bool> (AppSettings.Loading.GEN_LINE_BREAK, genomeLineBreakBut));

			SettingBinder<bool> ch3 = new SettingBinder<bool> (AppSettings.Loading.GFF3_REMOVE_SUFF, gff3TrimSuffCheck);
			_settings.Add (ch3);
			ch3.AddDependent (gff3TrimSuffEntry);

			SettingBinder<bool> ch4 = new SettingBinder<bool> (AppSettings.Loading.GFF3_SPLIT_ID, gff3SplitIDCheck);
			_settings.Add(ch4);
			ch4.AddDependent (gff3SplitCharEntry);
			ch4.AddDependent (gff3SplitSubsEntry);

			_settings.Add (new SettingBinder<List<string>> (AppSettings.Loading.GFF3_PRESUFFIX_TO_TRIM, gff3TrimSuffEntry));
			_settings.Add (new SettingBinder<int> (AppSettings.Loading.GFF3_SPLIT_SUBSTR, gff3SplitSubsEntry));
			_settings.Add (new SettingBinder<char[]> (AppSettings.Loading.GFF3_ID_SPLIT_CHARS, gff3SplitCharEntry));
			_settings.Add (new SettingBinder<string> (AppSettings.Loading.GFF3_ID_TAG, gff3MainIDTag));

			SettingBinder<bool> ch5 = new SettingBinder<bool> (AppSettings.Genes.GENERATE_ANY_PROMO, gPostMakePromoCheck);
			_settings.Add (ch5);
			ch5.AddDependent (gPostPro1Check);
			ch5.AddDependent (gPostPro2Check);
			ch5.AddDependent (gPostPro3Check);

			SettingBinder<bool> ch6 = new SettingBinder<bool> (AppSettings.Genes.GENERATE_PROMO_1, gPostPro1Check);
			_settings.Add (ch6);
			ch6.AddDependent (gPostPro1Entry);

			SettingBinder<bool> ch7 = new SettingBinder<bool> (AppSettings.Genes.GENERATE_PROMO_2, gPostPro2Check);
			_settings.Add (ch7);
			ch7.AddDependent (gPostPro2Entry);

			SettingBinder<bool> ch8 = new SettingBinder<bool> (AppSettings.Genes.GENERATE_PROMO_3, gPostPro3Check);
			_settings.Add (ch8);
			ch8.AddDependent (gPostPro3Entry);

			_settings.Add (new SettingBinder<int> (AppSettings.Genes.PROMO_1_SIZE, gPostPro1Entry));
			_settings.Add (new SettingBinder<int> (AppSettings.Genes.PROMO_2_SIZE, gPostPro2Entry));
			_settings.Add (new SettingBinder<int> (AppSettings.Genes.PROMO_3_SIZE, gPostPro3Entry));

			SettingBinder<bool> ch9 = new SettingBinder<bool> (AppSettings.Genes.GENERATE_FLANK3, gPostMake3FlankCheck);
			_settings.Add (ch9);
			ch9.AddDependent (gPost3FlankEntry);

			_settings.Add (new SettingBinder<int> (AppSettings.Genes.FLANK3_SIZE, gPost3FlankEntry));
			_settings.Add (new SettingBinder<bool> (AppSettings.Genes.GENERATE_INTRONS, gPostIntronCheck));

			SettingBinder<bool> ch10 = new SettingBinder<bool> (AppSettings.Processing.USE_THREADS, procUseThreads);
			_settings.Add (ch10);
			ch10.AddDependent (procThreadsEntry);
			_settings.Add (new SettingBinder<int> (AppSettings.Processing.MAX_THREADS, procThreadsEntry));

			_settings.Add (new SettingBinder<int> (AppSettings.Loading.FKPM_ID_COLUMN, fkpmIDColEntry));
			SettingBinder<bool> ch11 = new SettingBinder<bool> (AppSettings.Loading.FKPM_USE_OTHER_THAN_DIFFOUT, fkpmUseCuffDiffCheck);
			ch11.AddDependent (fkpmColDelimEntry);
			ch11.AddDependent (fkpmHasHILOCheck);
			ch11.AddDependent (fkpmHasTestCheck);
			ch11.AddDependent (fkpmScoreColEntry);

			_settings.Add (ch11);

			_settings.Add (new SettingBinder<int> (AppSettings.Loading.FKPM_SCORE_COLUMN, fkpmScoreColEntry));
			_settings.Add (new SettingBinder<int> (AppSettings.Loading.FKPM_TESTHI_COLUMN, fkpmHIColEntry));
			_settings.Add (new SettingBinder<int> (AppSettings.Loading.FKPM_TESTLO_COLUMN, fkpmLOColEntry));
			_settings.Add (new SettingBinder<int> (AppSettings.Loading.FKPM_TESTOK_COLUMN, fkpmTestColEntry));
			_settings.Add (new SettingBinder<char[]> (AppSettings.Loading.FKPM_FILE_DELIM, fkpmColDelimEntry));
			_settings.Add (new SettingBinder<string> (AppSettings.Loading.FKPM_TESTOK_TEXT, fkpmTestSuccessEntry));

			SettingBinder<bool> ch12 = new SettingBinder<bool> (AppSettings.Loading.FKPM_FILE_HAS_HILO, fkpmHasHILOCheck);
			ch12.AddDependent (fkpmHIColEntry);
			ch12.AddDependent (fkpmLOColEntry);
			_settings.Add (ch12);

			SettingBinder<bool> ch13 = new SettingBinder<bool> (AppSettings.Loading.FKPM_FILE_HAS_TESTOK, fkpmHasTestCheck);
			ch13.AddDependent (fkpmTestColEntry);
			ch13.AddDependent (fkpmTestSuccessEntry);
			_settings.Add (ch13);

			_settings.Add (new SettingBinder<bool> (AppSettings.Output.OUTPUT_PROCESSED, graphOutProcessCheck));
			_settings.Add (new SettingBinder<bool> (AppSettings.Statistic.NORMALISE_VARIANTS, statsNormVarCheck));
			_settings.Add (new SettingBinder<bool> (AppSettings.Loading.LOAD_SEQ_NAMES, samLoadReadNames));
			_settings.Add (new SettingBinder<bool> (AppSettings.Output.DEFAULT_IMG_FROMEND, graphFromEndCheck));
			_settings.Add (new SettingBinder<int> (AppSettings.Output.DEFAULT_IMG_BASES, graphBasesEntry));
		}

		protected void OnActivity (object o, Gtk.WidgetEventArgs args)
		{
			foreach (SettingBinder b in _settings) {
				b.SensitizeDependents ();
			}
		}

		public class FilterEntryBinder
		{
			public Entry MinEnt;
			public Entry MaxEnt;
			public CheckButton Check;
			public string id;

			public FilterEntryBinder(CheckButton chb, Entry min, Entry max, string _id)
			{
				Check = chb;
				MaxEnt = max;
				MinEnt = min;
				id = _id;
			}
		}

		public class OldFiltSettings
		{
			public string name;
			public int min;
			public int max;

			public OldFiltSettings(string _name, int _min, int _max)
			{
				name = _name;
				min = _min;
				max = _max;
			}
		}
	}
}

