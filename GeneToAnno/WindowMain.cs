using System;
using Gtk;
using System.Collections.Generic;
using OxyPlot.GtkSharp;
using OxyPlot;
using System.Threading;
using System.Drawing;

namespace GeneToAnno
{
	public enum GraphWindow {Genome, Gene, Meth, Global, Comparative, Cluster};
	public enum CurrentSavedRankData {Double, ListDouble};
	public partial class WindowMain: Gtk.Window
	{
		protected PlotView pView;
		protected RSplitContainer<double> LatestRSplitDOUBLE;
		protected RSplitContainer<List<double>> LatestRSplitLISTDUB;
		protected CurrentSavedRankData CurrentRSPContainerType;

		protected Dictionary<SensitiType, List<Widget>> SensiMap;
		protected Dictionary<GraphWindow, GraphWindowPair> gWindows;
		protected Dictionary<GraphWindow, DelegateComboMenu> dMenus;

		protected Dictionary<string, RankMethod> RankSplitMethods;
		protected Dictionary<string, OutputMethText> RankSplitTextOut;
		protected Dictionary<string, OutputMethGraph> RankSplitGraphOut;
		protected Dictionary<string, GraphType> RankSplitGraphDesc;
		protected Dictionary<OutputMethGraph, List<GraphType>> RankSplitGraphToTypes;
	
		protected Dictionary<string, BioSample> RankSplitSamples;
		protected ImgDisplay ImageDisplayPanel;

		protected int OldRSPSize;

		public WindowMain () : base (Gtk.WindowType.Toplevel)
		{
			Build ();
			MainData.MainWindow = this;
			AssignSensitivityControl ();
			graphNoteBook.CurrentPage = 0;
			loadNoteBook.CurrentPage = 0;
			statsNoteBook.CurrentPage = 0;
			MakeGraphPairDict ();
			MakeDelegateMenus ();
			ImageDisplayPanel = new ImgDisplay (imgDrawSpace);
		}

		public void InsertImage(List<Bitmap> bmps, List<string> ids)
		{
			Gtk.Application.Invoke (delegate {
				ImageDisplayPanel.RecieveImages (bmps, ids);

				ClearComboBox (imgSelectCombo);

				foreach (string st in ids) {
					imgSelectCombo.AppendText (st);
				}
				graphNoteBook.CurrentPage = 6;
			});

		}

		public void SetRSplitDoubleContainer(RSplitContainer<double> rsc)
		{
			LatestRSplitDOUBLE = rsc;
			CurrentRSPContainerType = CurrentSavedRankData.Double;
		}

		public void SetRSplitDoubleContainer(RSplitContainer<List<double>> rsc)
		{
			LatestRSplitLISTDUB = rsc;
			CurrentRSPContainerType = CurrentSavedRankData.ListDouble;
		}

		protected NumericalText RSPGetNumericalText()
		{
			if (CurrentRSPContainerType == CurrentSavedRankData.Double) {
				return LatestRSplitDOUBLE.GetData (RankSplit.OutputFromStart);
			} else {
				return LatestRSplitLISTDUB.GetData (RankSplit.OutputFromStart);
			}
		}

		protected List<string> RSPGetOldIDList(int st, int ed)
		{
			if (CurrentRSPContainerType == CurrentSavedRankData.Double) {
				return LatestRSplitDOUBLE.GetAllGeneIDs (st, ed);
			} else {
				return LatestRSplitLISTDUB.GetAllGeneIDs (st, ed);;
			}
		}

		protected void MakeGraphPairDict()
		{
			gWindows = new Dictionary<GraphWindow, GraphWindowPair> ();
			gWindows.Add (GraphWindow.Genome, new GraphWindowPair (genomeWindow, new PlotView ()));
			gWindows.Add (GraphWindow.Gene, new GraphWindowPair (geneWindow, new PlotView ()));
			gWindows.Add (GraphWindow.Meth, new GraphWindowPair (methWindow, new PlotView ()));
			gWindows.Add (GraphWindow.Global, new GraphWindowPair (globalWindow, new PlotView ()));
			gWindows.Add (GraphWindow.Comparative, new GraphWindowPair (comparativeWindow, new PlotView ()));
			gWindows.Add (GraphWindow.Cluster, new GraphWindowPair (clusterWindow, new PlotView ()));
		}

		protected void MakeDelegateMenus()
		{
			dMenus = new Dictionary<GraphWindow, DelegateComboMenu> ();

			dMenus.Add (GraphWindow.Genome, new DelegateComboMenu (genomeCombo));
			dMenus.Add (GraphWindow.Gene, new DelegateComboMenu (geneCombo));
			dMenus.Add (GraphWindow.Meth, new DelegateComboMenu (methCombo));

			dMenus [GraphWindow.Genome].AddAction (OnGenomeCalcBase, "Scan Fragment Base Ratios");
			dMenus [GraphWindow.Gene].AddAction (OnGeneLengthDistro, "Scan element lengths");
			dMenus [GraphWindow.Gene].AddAction (OnGeneBaseRatioScan, "Scan element base-ratios");
			dMenus [GraphWindow.Meth].AddAction (OnMethMakeCovRate, "Show element read-coverage");
			dMenus [GraphWindow.Meth].AddAction (OnMethMakeCovDepthRate, "Show ele. coverage-depth score");
			dMenus [GraphWindow.Meth].AddAction (OnMethMakeSpatialCov, "Show ele. spatial coverage rate");
			dMenus [GraphWindow.Meth].AddAction (OnMethMakeSpatialDepthCov, "Show ele. spatial depth-coverage rate");

		}

		protected void AssignSensitivityControl()
		{
			SensiMap = new Dictionary<SensitiType, List<Widget>> ();
			for (int i = 0; i < ProgramState.SENSITYPE_COUNT; i++) {
				SensiMap.Add ((SensitiType)i, new List<Widget> ());
			}
			SensiMap [SensitiType.Genome].Add (genomeCombo);
			SensiMap [SensitiType.Genome].Add (genomeGo);
			SensiMap [SensitiType.Genome].Add (gffLoadBut);

			SensiMap [SensitiType.GenomeGraph].Add (genomeSavePdf);
			SensiMap [SensitiType.GenomeGraph].Add (genomeSaveTxt);

			SensiMap [SensitiType.GFF3].Add (geneCombo);
			SensiMap [SensitiType.GFF3].Add (geneGo);
			SensiMap [SensitiType.GFF3].Add (outfmt6LoadBut);
			SensiMap [SensitiType.GFF3].Add (bamAddBut);
			SensiMap [SensitiType.GFF3].Add (bamClearBut);
			SensiMap [SensitiType.GFF3].Add (bamLoadBut);
			SensiMap [SensitiType.GFF3].Add (fkpmAddBut);
			SensiMap [SensitiType.GFF3].Add (fkpmLoadBut);
			SensiMap [SensitiType.GFF3].Add (fkpmClearBut);
			SensiMap [SensitiType.GFF3].Add (vcfLoadBut);
			SensiMap [SensitiType.GFF3].Add (vcfAddBut);
			SensiMap [SensitiType.GFF3].Add (vcfClearBut);

			SensiMap [SensitiType.GeneGraph].Add (geneSaveButTxt);
			SensiMap [SensitiType.GeneGraph].Add (geneSaveButPdf);

			SensiMap [SensitiType.BAM].Add (methCombo);
			SensiMap [SensitiType.BAM].Add (methGo);

			SensiMap [SensitiType.BAM].Add (rspToEndRadio);
			SensiMap [SensitiType.BAM].Add (rspFromStartRadio);
			SensiMap [SensitiType.BAM].Add (rspTextSaveGoBut);
			SensiMap [SensitiType.BAM].Add (rspTextOutputCombo);
			SensiMap [SensitiType.BAM].Add (rspSubDivideCheck);
			SensiMap [SensitiType.BAM].Add (rspStartDivEntry);
			SensiMap [SensitiType.BAM].Add (rspSequenceCombo);
			SensiMap [SensitiType.BAM].Add (rspRankDivEntry);
			SensiMap [SensitiType.BAM].Add (rspPickNormaliseRadio);
			SensiMap [SensitiType.BAM].Add (rspNoBaseEntry);
			SensiMap [SensitiType.BAM].Add (rspMethodCombo);
			SensiMap [SensitiType.BAM].Add (rspGraphicalProcessBut);
			SensiMap [SensitiType.BAM].Add (rspGraphicalOutputCombo);
			SensiMap [SensitiType.BAM].Add (rspEndDivEntry);
			SensiMap [SensitiType.BAM].Add (rspBaseCountRadio);

			SensiMap [SensitiType.BAM].Add (rspToEndRadio2);
			SensiMap [SensitiType.BAM].Add (rspFromStartRadio2);
			SensiMap [SensitiType.BAM].Add (rspSubDivideCheck2);
			SensiMap [SensitiType.BAM].Add (rspStartDivEntry2);
			SensiMap [SensitiType.BAM].Add (rspPickNormaliseRadio2);
			SensiMap [SensitiType.BAM].Add (rspNoBaseEntry2);
			SensiMap [SensitiType.BAM].Add (rspEndDivEntry2);
			SensiMap [SensitiType.BAM].Add (rspBaseCountRadio2);
			SensiMap [SensitiType.BAM].Add (rspDoSubDivCheck);
			SensiMap [SensitiType.BAM].Add (rspSubTakeGroupEntry);
			SensiMap [SensitiType.BAM].Add (rspSubAndSplitEntry);
			SensiMap [SensitiType.BAM].Add (rspSubDivNextRadio);
			SensiMap [SensitiType.BAM].Add (rspSequenceForCombo);
			SensiMap [SensitiType.BAM].Add (rspGraphTypeCombo);

			SensiMap [SensitiType.BAM].Add (rspSampleCombo);
			SensiMap [SensitiType.BAM].Add (rspChooseByIndexCheck);
			SensiMap [SensitiType.BAM].Add (rspChooseForIndexCheck);
			SensiMap [SensitiType.BAM].Add (rspForIndexEntry);
			SensiMap [SensitiType.BAM].Add (rspByIndexEntry);

			SensiMap [SensitiType.MethGraph].Add (methSavePdf);
			SensiMap [SensitiType.MethGraph].Add (methSaveTxt);

			SensiMap [SensitiType.GlobalGraph].Add (globalSaveText);
			SensiMap [SensitiType.GlobalGraph].Add (globalSavePdf);
			SensiMap [SensitiType.GlobalGraph].Add (rspSubDivOldRadio);
			SensiMap [SensitiType.GlobalGraph].Add (rspSubFromGroupEntry);
			SensiMap [SensitiType.GlobalGraph].Add (rspSubToGroupEntry);
			SensiMap [SensitiType.GlobalGraph].Add (rspGetCorrelation);
			SensiMap [SensitiType.GlobalGraph].Add (rspExportFullRank);
			SensiMap [SensitiType.GlobalGraph].Add (rspMergeGraphCheck);

			SensiMap [SensitiType.ComparativeGraph].Add (comparativeSaveText);
			SensiMap [SensitiType.ComparativeGraph].Add (comparativeSavePdf);
			SensiMap [SensitiType.ClusterGraph].Add (clusterSaveText);
			SensiMap [SensitiType.ClusterGraph].Add (clusterSavePdf);

			SensiMap [SensitiType.ImgAll].Add (imgSaveBut);
			SensiMap [SensitiType.ImgAll].Add (imgSelectCombo);
			SensiMap [SensitiType.ImgAll].Add (imgShowBut);

			SensiMap [SensitiType.BAMLoadLock].Add (bamLoadBut);
			SensiMap [SensitiType.BAMLoadLock].Add (bamAddBut);
			SensiMap [SensitiType.BAMLoadLock].Add (gffLoadBut);
			SensiMap [SensitiType.BAMLoadLock].Add (genomeLoadBut);
			SensiMap [SensitiType.BAMLoadLock].Add (outfmt6LoadBut);
			SensiMap [SensitiType.BAMLoadLock].Add (bamClearBut);
			SensiMap [SensitiType.BAMLoadLock].Add (vcfLoadBut);
			SensiMap [SensitiType.BAMLoadLock].Add (fkpmLoadBut);

			SensiMap [SensitiType.VariantLoadLock].Add (vcfLoadBut);
			SensiMap [SensitiType.VariantLoadLock].Add (vcfAddBut);
			SensiMap [SensitiType.VariantLoadLock].Add (vcfClearBut);
			SensiMap [SensitiType.VariantLoadLock].Add (genomeLoadBut);
			SensiMap [SensitiType.VariantLoadLock].Add (outfmt6LoadBut);
			SensiMap [SensitiType.VariantLoadLock].Add (gffLoadBut);
			SensiMap [SensitiType.VariantLoadLock].Add (bamLoadBut);
			SensiMap [SensitiType.VariantLoadLock].Add (fkpmLoadBut);

			SensiMap [SensitiType.GenLoadLock].Add (genomeLoadBut);
			SensiMap [SensitiType.GffLoadLock].Add (gffLoadBut);
			SensiMap [SensitiType.GffLoadLock].Add (genomeLoadBut);

			SensiMap [SensitiType.Outfmt6LoadLock].Add (outfmt6LoadBut);
			SensiMap [SensitiType.Outfmt6LoadLock].Add (gffLoadBut);
			SensiMap [SensitiType.Outfmt6LoadLock].Add (genomeLoadBut);
		}

		public void InsertGraph(GraphWindow wind, PlotModel mod, ProcessingClass proc)
		{
			Gtk.Application.Invoke (delegate {
				gWindows [wind].AssignAndShow (mod, proc);
				graphNoteBook.CurrentPage = (int)wind;
			});
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected void OnGenomeLoad (object sender, EventArgs e)
		{
			Thread othread;
			FileChooserDialog chooser = new FileChooserDialog (
				"Select a Fasta File For the Genome..",
				this,
				FileChooserAction.Open,
				"Cancel", ResponseType.Cancel,
				"Open", ResponseType.Accept);

			if (chooser.Run () == (int)ResponseType.Accept) {

				if (chooser.Filename.Contains (".fa")) {
					othread = new Thread (new ThreadStart (MainData.LoadGenome));
					MainData.genomeFileName = chooser.Filename;
					this.genomeLabel.Text = MaxLenString(chooser.Filename, 43);
					othread.Start ();
				} else {
					GenerateMessageWindow ("This doesn't look like a fasta file!");
				}
			}

			chooser.Destroy ();
		}

		protected void OnOutFmt6Load(object sender, EventArgs e)
		{
			Thread othread;
			FileChooserDialog chooser = new FileChooserDialog (
				"Select a Blast Output File to load..",
				this,
				FileChooserAction.Open,
				"Cancel", ResponseType.Cancel,
				"Open", ResponseType.Accept);

			if (chooser.Run () == (int)ResponseType.Accept) {

				othread = new Thread (new ThreadStart (MainData.LoadBlastOutfmt6));
				MainData.outfmt6FileName = chooser.Filename;
				this.outfmt6Label.Text = MaxLenString(chooser.Filename, 43);
				othread.Start ();
			}

			chooser.Destroy ();
		}

		public void UpdateLog(string _text)
		{
			processLog.Buffer.Text = _text + "\n" + processLog.Buffer.Text;
		}

		public void GenerateMessageWindow(string _text)
		{
			MessageDialog md = new MessageDialog (null, 
				DialogFlags.Modal, 
				MessageType.Info, 
				ButtonsType.Ok, 
				_text);
			md.Run ();
			md.Destroy ();
		}

		public string MaxLenString(string str, int maxLen)
		{
			if (str.Length > maxLen) {
				return "..." + str.Substring ((str.Length - 1) - (maxLen - 3));
			} else
				return str;
		}

		public void Sensitize(SensitiType stype, bool _activate)
		{
			foreach (Widget wid in SensiMap[stype]) {
				wid.Sensitive = _activate;
			}
		}

		public void UpdateRankSplit()
		{
			if (ProgramState.LoadedSamples) {
				ClearComboBox (rspTextOutputCombo);
				ClearComboBox (rspSequenceCombo);
				ClearComboBox (rspMethodCombo);
				ClearComboBox (rspGraphicalOutputCombo);
				ClearComboBox (rspSampleCombo);
				ClearComboBox (rspSequenceForCombo);

				RankSplitMethods = RankSplit.GetRelevantMethods ();
				RankSplitGraphOut = RankSplit.GetRelevantOutputGraph ();
				RankSplitTextOut = RankSplit.GetRelevantOutputText ();
				RankSplitSamples = RankSplit.GetRelevantSamples ();
				RankSplitGraphToTypes = RankSplit.GetGraphTypes ();
				RankSplitGraphDesc = RankSplit.GetRelevantGraphTypes ();

				List<string> seqFet = RankSplit.GetRelevantFeatures ();

				foreach (KeyValuePair<string, BioSample> kvp in RankSplitSamples) {
					rspSampleCombo.AppendText (kvp.Key);
				}
				foreach (KeyValuePair<string, RankMethod> kvp in RankSplitMethods) {
					rspMethodCombo.AppendText (kvp.Key);
				}
				foreach (KeyValuePair<string, OutputMethGraph> kvp in RankSplitGraphOut) {
					rspGraphicalOutputCombo.AppendText (kvp.Key);
				}
				foreach (KeyValuePair<string, OutputMethText> kvp in RankSplitTextOut) {
					rspTextOutputCombo.AppendText (kvp.Key);
				}
				foreach (string st in seqFet) {
					rspSequenceCombo.AppendText (st);
				}
				foreach (string st in seqFet) {
					rspSequenceForCombo.AppendText (st);
				}
			}
		}

		protected void ClearComboBox(ComboBox cb)
		{
			ListStore clist = new ListStore (typeof(string), typeof(string));
			cb.Model = clist;
		}

		protected void OnGFF3Load (object sender, EventArgs e)
		{
			Thread othread;
			FileChooserDialog chooser = new FileChooserDialog (
				"Select a GFF3 File For the Genome..",
				this,
				FileChooserAction.Open,
				"Cancel", ResponseType.Cancel,
				"Open", ResponseType.Accept);

			if (chooser.Run () == (int)ResponseType.Accept) {

				if (chooser.Filename.Contains (".gff3") || chooser.Filename.Contains (".GFF3")) {
					othread = new Thread (new ThreadStart (MainData.LoadGFF3));
					MainData.gff3FileName = chooser.Filename;
					this.gff3Label.Text = MaxLenString (chooser.Filename, 43);
					othread.Start ();
				} else {
					GenerateMessageWindow ("This doesn't look like a GFF3 file!");
				}
			}

			chooser.Destroy ();
		}

		protected void OnOpenSettings(object sender, EventArgs e)
		{
			LoaderSettings ls = new LoaderSettings ();
			ls.Modal = true;
			ls.Show ();
		}

		protected void OnUnloadAll (object sender, EventArgs e)
		{
			MainData.UnloadAll ();
			genomeLabel.Text = " -- Genome -- ";
			gff3Label.Text = " -- Gff3 -- ";
			outfmt6Label.Text = " -- Blast.outfmt6 -- ";
			foreach (KeyValuePair<GraphWindow, GraphWindowPair> kvp in gWindows) {
				kvp.Value.DetachAndHide ();
			}
			ProgramState.ResetAll ();

			UpdateLog ("Data unloaded, program reset.\n.\n.\n.\n");
		}

		protected void OnSavePdf (object sender, EventArgs e)
		{
			GraphWindow win = (GraphWindow)graphNoteBook.CurrentPage;

			PlotModel currentModel = gWindows [win].GetModel ();

			if(currentModel != null)
			{
				SavePdfWindow pdwin = new SavePdfWindow (currentModel);
				pdwin.Show ();
			}
		}

		protected void OnSaveText (object sender, EventArgs e)
		{
			GraphWindow win = (GraphWindow)graphNoteBook.CurrentPage;

			NumericalText currentData = gWindows [win].GetData ();

			if (currentData != null) {
				FileChooserDialog saveIt = new FileChooserDialog(
					"Where should the data be saved?..",
					this,
					FileChooserAction.Save,
					"Cancel", ResponseType.Cancel,
					"Open", ResponseType.Accept);

				if (saveIt.Run () == (int)ResponseType.Accept) {
					ProcessingClass.WriteDataFile (saveIt.Filename, currentData);
				}

				saveIt.Destroy ();
			}
		}

		protected void OnSaveRSPContainer (object sender, EventArgs e)
		{
			NumericalText currentData = RSPGetNumericalText();

			if (currentData != null) {
				FileChooserDialog saveIt = new FileChooserDialog(
					"Where should the Rank/Split Output be saved?..",
					this,
					FileChooserAction.Save,
					"Cancel", ResponseType.Cancel,
					"Open", ResponseType.Accept);

				if (saveIt.Run () == (int)ResponseType.Accept) {
					ProcessingClass.WriteDataFile (saveIt.Filename, currentData);
				}

				saveIt.Destroy ();
			}
		}

		protected void OnPingu (object sender, EventArgs e)
		{
			//MainData.TestElementCoverage ();
		}
		
		protected void OnMethMakeSpatialCov()
		{
			if (ProgramState.LoadedSamples == true) {
				if (MainData.TypeDict.RunSetTypeDictCaseWindow (TypeDictCase.Methylation) == (int)ResponseType.Accept) {
					UpdateLog ("Updated annotation element selection for: " + TypeDictCase.Methylation.ToString("G"));

					Thread o = new Thread (new ThreadStart (MainData.MakeMethySpatialCov));
					o.Start ();
				}
			}
		}

		protected void OnMethMakeSpatialDepthCov()
		{
			if (ProgramState.LoadedSamples == true) {
				if (MainData.TypeDict.RunSetTypeDictCaseWindow (TypeDictCase.Methylation) == (int)ResponseType.Accept) {
					UpdateLog ("Updated annotation element selection for: " + TypeDictCase.Methylation.ToString("G"));

					Thread o = new Thread (new ThreadStart (MainData.MakeMethySpatialDepthCov));
					o.Start ();
				}
			}
		}

		protected void OnMethMakeCovRate ()
		{
			if (ProgramState.LoadedSamples == true) {
				if (MainData.TypeDict.RunSetTypeDictCaseWindow (TypeDictCase.Methylation) == (int)ResponseType.Accept) {
					UpdateLog ("Updated annotation element selection for: " + TypeDictCase.Methylation.ToString("G"));

					Thread o = new Thread (new ThreadStart (MainData.MakeMethyCoverageDistro));
					o.Start ();
				}
			}
		}

		protected void OnMethMakeCovDepthRate ()
		{
			if (ProgramState.LoadedSamples == true) {
				if (MainData.TypeDict.RunSetTypeDictCaseWindow (TypeDictCase.Methylation) == (int)ResponseType.Accept) {
					UpdateLog ("Updated annotation element selection for: " + TypeDictCase.Methylation.ToString("G"));

					Thread o = new Thread (new ThreadStart (MainData.MakeMethyCovDepthScoreDistro));
					o.Start ();
				}
			}
		}

		protected void OnGeneLengthDistro ()
		{
			if (ProgramState.LoadedGFF3 == true) {
				if (MainData.TypeDict.RunSetTypeDictCaseWindow (TypeDictCase.Genes) == (int)ResponseType.Accept) {
					UpdateLog ("Updated annotation element selection.");

					Thread o = new Thread (new ThreadStart (MainData.MakeGeneLengthDistro));
					o.Start ();
				}
			}
		}

		protected void OnGeneBaseRatioScan ()
		{
			if (ProgramState.LoadedGFF3 == true) {
				if (MainData.TypeDict.RunSetTypeDictCaseWindow (TypeDictCase.Genes) == (int)ResponseType.Accept) {
					UpdateLog ("Updated annotation element selection.");

					Thread o = new Thread (new ThreadStart (MainData.MakeGeneBaseRatio));
					o.Start ();
				}
			}
		}

		protected void OnGenomeCalcBase ()
		{
			if (ProgramState.LoadedGenome == true) {
				Thread othread = new Thread (new ThreadStart (MainData.MakeGenomeBaseRatio));
				othread.Start ();
			}
		}

		protected void OnPressGo(object sender, EventArgs e)
		{
			dMenus [(GraphWindow)graphNoteBook.CurrentPage].InvokeCurrent ();
		}

		protected void OnBamAddFile (object sender, EventArgs e)
		{
			FileChooserDialog chooser = new FileChooserDialog (
				"Select one or more BAM files..",
				this,
				FileChooserAction.Open,
				"Cancel", ResponseType.Cancel,
				"Open", ResponseType.Accept);

			chooser.SelectMultiple = true;

			if (chooser.Run () == (int)ResponseType.Accept) {

				if (chooser.Filename.Contains (".sam") || chooser.Filename.Contains (".SAM")) {
					foreach (string st in chooser.Filenames) {
						bamTextView.Buffer.Text = bamTextView.Buffer.Text + st + "\n";
					}
					MainData.BAMFileNames.AddRange (chooser.Filenames);
				} else {
					GenerateMessageWindow ("This doesn't look like a SAM file!");
				}
			}

			chooser.Destroy ();
		}

		protected void OnBamLoad (object sender, EventArgs e)
		{
			if (MainData.BAMFileNames.Count > 0) {
				List<PreSampleDefinition> prels = new List<PreSampleDefinition> ();

				foreach (string st in MainData.BAMFileNames) {
					PreSampleDefinition pef = new PreSampleDefinition (SampleType.BAM);
					pef.singleFName = st;
					prels.Add (pef);
				}

				SampleAllocationWindow swind = new SampleAllocationWindow (prels);

				swind.ShowAll ();
			} else {
				MainData.ShowMessageWindow ("You need to add some files to load first!", false);
			}
		}

		protected void OnBamClear (object sender, EventArgs e)
		{
			MainData.BAMFileNames.Clear ();
			bamTextView.Buffer.Text = "";
		}

		protected void OnVcfAddFile (object sender, EventArgs e)
		{
			FileChooserDialog chooser = new FileChooserDialog (
				"Select one or more Vcf files..",
				this,
				FileChooserAction.Open,
				"Cancel", ResponseType.Cancel,
				"Open", ResponseType.Accept);

			chooser.SelectMultiple = true;

			if (chooser.Run () == (int)ResponseType.Accept) {

				if (chooser.Filename.Contains (".vcf") || chooser.Filename.Contains (".VCF")) {
					foreach (string st in chooser.Filenames) {
						vcfTextView.Buffer.Text = vcfTextView.Buffer.Text + st + "\n";
					}
					MainData.VariantFileNames.AddRange (chooser.Filenames);
				} else {
					GenerateMessageWindow ("This doesn't look like a Vcf file!");
				}
			}

			chooser.Destroy ();
		}

		protected void OnVcfLoad (object sender, EventArgs e)
		{
			if (MainData.VariantFileNames.Count > 0) {
				List<PreSampleDefinition> prels = new List<PreSampleDefinition> ();

				foreach (string st in MainData.VariantFileNames) {
					List<string> cols = null;

					if (Variant.IsGenotyped (st, ref cols)) {
						int cnt = 0;
						foreach (string ss in cols) {
							PreSampleDefinition pef = new PreSampleDefinition (SampleType.Vcf);
							pef.singleFName = st;
							pef.singleColID = ss;
							pef.singleColIndex = cnt + 9;
							pef.IsGenotyped = true;
							pef.TotalColumns = cols.Count;
							prels.Add (pef);
							cnt++;
						}
					} else {
						PreSampleDefinition pef = new PreSampleDefinition (SampleType.Vcf);
						pef.singleFName = st;
						pef.IsGenotyped = false;
						pef.TotalColumns = 0;
						prels.Add (pef);
					}
				}

				SampleAllocationWindow swind = new SampleAllocationWindow (prels);

				swind.ShowAll ();
			} else {
				MainData.ShowMessageWindow ("You need to add some files to load first!", false);
			}
		}

		protected void OnVcfClear (object sender, EventArgs e)
		{
			MainData.VariantFileNames.Clear ();
			vcfTextView.Buffer.Text = "";
		}

		protected void OnFkpmAddFile (object sender, EventArgs e)
		{
			FileChooserDialog chooser = new FileChooserDialog (
				"Select one or more Fkpm files..",
				this,
				FileChooserAction.Open,
				"Cancel", ResponseType.Cancel,
				"Open", ResponseType.Accept);

			chooser.SelectMultiple = true;

			if (chooser.Run () == (int)ResponseType.Accept) {

				//if (chooser.Filename.Contains (".fkpm") || chooser.Filename.Contains (".FKPM")) {
					foreach (string st in chooser.Filenames) {
						fkpmTextView.Buffer.Text = fkpmTextView.Buffer.Text + st + "\n";
					}
					MainData.FPKMFileNames.AddRange (chooser.Filenames);
				//} else {
				//	GenerateMessageWindow ("This doesn't look like an FKPM file!");
				//}
			}

			chooser.Destroy ();
		}

		protected void OnFkpmLoad (object sender, EventArgs e)
		{
			if (MainData.FPKMFileNames.Count > 0) {
				List<PreSampleDefinition> prels = new List<PreSampleDefinition> ();

				foreach (string st in MainData.FPKMFileNames) {
					List<string> cols = null;
					FKPM.GetSampleNames (st, out cols);
					int cnt = 0;
					foreach (string ss in cols) {
						PreSampleDefinition pef = new PreSampleDefinition (SampleType.Expr);
						pef.singleFName = st;
						pef.singleColID = ss;
						pef.singleColIndex = cnt + 9;
						pef.IsGenotyped = false;
						pef.TotalColumns = cols.Count;
						prels.Add (pef);
						cnt += 4;
					}
				}

				SampleAllocationWindow swind = new SampleAllocationWindow (prels);

				swind.ShowAll ();
			} else {
				MainData.ShowMessageWindow ("You need to add some files to load first!", false);
			}
		}

		protected void OnFkpmClear (object sender, EventArgs e)
		{
			MainData.FPKMFileNames.Clear ();
			fkpmTextView.Buffer.Text = "";
		}

		protected void OnRankSplitSaveGo (object sender, EventArgs e)
		{
			try{
				ParameterizedThreadStart prams = new ParameterizedThreadStart(RankSplit.ProcessRequest);
				Thread o = new Thread(prams);
				RankSplitInfo rsi = GatherRankSplitInfo (false);
				if(RankSplit.PickFileName(out RankSplit.LastFileName))
				{
					o.Start((object)rsi);
				}
			}
			catch(Exception ex)
			{
				MainData.ShowMessageWindow(ex.Message, false);
			}
		}

		protected void OnRankSplitProcess (object sender, EventArgs e)
		{
			try{
				ParameterizedThreadStart prams = new ParameterizedThreadStart(RankSplit.ProcessRequest);
				Thread o = new Thread(prams);
				RankSplitInfo rsi = GatherRankSplitInfo (true);
				o.Start((object)rsi);
			}
			catch(Exception ex)
			{
				MainData.ShowMessageWindow(ex.Message, false);
			}
		}

		protected RankSplitInfo GatherRankSplitInfo(bool isGraph)
		{
			RankMethod m;
			OutputMethGraph omg;
			OutputMethText omt;
			GraphType gt;

			string feature;
			string feature2;
			try{
				m = RankSplitMethods [rspMethodCombo.ActiveText];
				omg = OutputMethGraph.Expr;
				omt = OutputMethText.BaseSeq;
				gt = RankSplitGraphDesc[rspGraphTypeCombo.ActiveText];

				if (isGraph) {
					omg = RankSplitGraphOut [rspGraphicalOutputCombo.ActiveText];
				} else {
					omt = RankSplitTextOut [rspTextOutputCombo.ActiveText];
				}
			}
			catch(Exception e) {
				throw new Exception ("You need to select an option from the combo menu!");
			}

			feature = rspSequenceCombo.ActiveText;
			feature2 = rspSequenceForCombo.ActiveText;
			bool seqFeaturePass = false;
			bool seqFeature2Pass = false;
			foreach (string st in RankSplit.GetRelevantFeatures()) {

				if (st == feature) {
					seqFeaturePass = true;
				}

				if (st == feature2)
					seqFeature2Pass = true;
			}

			if (!seqFeaturePass || !seqFeature2Pass) {
				throw new Exception ("You need to choose two features to look at\n they can be the same!");
			}

			RankSplitInfo rsi;

			if (!rspSubDivideCheck.Active) {
				if (isGraph) {
					rsi = new RankSplitInfo (m, omg, feature);
				} else {
					rsi = new RankSplitInfo (m, omt, feature);
				}
			} else {
				if (rspBaseCountRadio.Active) {
					int bases;
					if (!int.TryParse (rspNoBaseEntry.Text, out bases)) {
						throw new Exception ("Input -> Unable To Parse Base Count!");
					}
					if (bases > 5000) {
						throw new Exception ("Input -> Maximum Number of bases is 5000!");
					}
					bool doStart = true;
					if (rspToEndRadio.Active) {
						doStart = false;
					}

					if (isGraph) {
						rsi = new RankSplitInfo (m, omg, feature, bases, doStart);
					} else {
						rsi = new RankSplitInfo (m, omt, feature, bases, doStart);
					}
				} else {
					int stDiv; int edDiv;

					if (!int.TryParse (rspStartDivEntry.Text, out stDiv)) {
						throw new Exception ("Input -> Unable to Parse Start Division!");
					}
					if (!int.TryParse (rspEndDivEntry.Text, out edDiv)) {
						throw new Exception ("Input -> Unable to Parse End Division!");
					}

					if (stDiv >= edDiv)
						throw new Exception ("Input -> Start division must be smaller than end!");
					if (stDiv > 20 || edDiv > 20)
						throw new Exception ("Input -> Division limit is 20.");

					if (isGraph) {
						rsi = new RankSplitInfo (m, omg, feature, stDiv, edDiv);
					} else {
						rsi = new RankSplitInfo (m, omt, feature, stDiv, edDiv);
					}
				}
			}

			rsi.SeqFeature2 = feature2;
			rsi.GraphingType = gt;

			if (rspSubDivideCheck2.Active) {
				rsi.doSubSeqOut = true;

				if (rspPickNormaliseRadio2.Active) {
					rsi.SeqDivOut = SubSequenceDivision.Normalised;

					int stDiv2;
					int edDiv2;

					if (!int.TryParse (rspStartDivEntry2.Text, out stDiv2)) {
						throw new Exception ("Output -> Unable to Parse Start Division!");
					}
					if (!int.TryParse (rspEndDivEntry2.Text, out edDiv2)) {
						throw new Exception ("Output -> Unable to Parse End Division!");
					}

					if (stDiv2 >= edDiv2)
						throw new Exception ("Output -> Start division must be smaller than end!");
					if (stDiv2 > 20 || edDiv2 > 20)
						throw new Exception ("Output -> Division limit is 20.");

					rsi.divStartOut = stDiv2;
					rsi.divEndOut = edDiv2;
				} else {
					rsi.SeqDivOut = SubSequenceDivision.AbsoluteBases;

					int bases2;
					if (!int.TryParse (rspNoBaseEntry2.Text, out bases2)) {
						throw new Exception ("Output -> Unable To Parse Base Count!");
					}
					if (bases2 > 5000) {
						throw new Exception ("Output -> Maximum Number of bases is 5000!");
					}
					bool doStart2 = true;
					if (rspToEndRadio2.Active) {
						doStart2 = false;
					}

					rsi.baseCountOut = bases2;
					rsi.fromStartOut = doStart2;
				}
			}
			int splitInt;

			if (!int.TryParse (rspRankDivEntry.Text, out splitInt)) {
				throw new Exception ("Rank Division - please enter a positive integer.");
			}
			rsi.SplitCount = splitInt;

			if (rspDoSubDivCheck.Active) {
				rsi.doSubSet = true;
				if (rspSubDivNextRadio.Active) {
					rsi.SubSetType = SubSetMethod.Current;
					int mainRank; int splitRank;

					if (!int.TryParse (rspSubTakeGroupEntry.Text, out mainRank)) {
						throw new Exception ("Unable to parse Sub-Set 'take group' entry, please enter a positive integer.");
					}
					if (!int.TryParse (rspSubAndSplitEntry.Text, out splitRank)) {
						throw new Exception ("Unable to parse Sub-Set 'and split' entry, please enter a positive integer.");
					}
					if (mainRank > rsi.SplitCount) {
						throw new Exception ("'Take Group' value must be lower than or equal to the split count.");
					} else if (mainRank < 1) {
						throw new Exception ("'Take Group' value too low, please enter a positive integer.");
					}
					if (splitRank < 1) {
						throw new Exception ("'And Split' value too low, please enter a positive integer.");
					}
					rsi.subCurMainRank = mainRank;
					rsi.subCurSplitCount = splitRank;

				} else {
					rsi.SubSetType = SubSetMethod.Previous;
					int fromGroup; int toGroup;

					if (!int.TryParse (rspSubFromGroupEntry.Text, out fromGroup)) {
						throw new Exception ("Unable to parse Sub-Set 'from group' entry, please enter a positive integer.");
					}
					if (!int.TryParse (rspSubToGroupEntry.Text, out toGroup)) {
						throw new Exception ("Unable to parse Sub-Set 'to group' entry, please enter a positive integer.");
					}
					if (fromGroup > OldRSPSize) {
						throw new Exception ("'From Group' must be lower than or equal to the previous Split-Count");
					}
					if (toGroup > OldRSPSize) {
						throw new Exception ("'To Group' must be lower than or equal to the previous Split-Count");
					}
					if (toGroup < fromGroup) {
						throw new Exception ("'To Group' should be higher than or equal to 'From Group'");
					}
					rsi.subOldEndRank = toGroup;
					rsi.subOldStartRank = fromGroup;

					rsi.oldIDlist = RSPGetOldIDList (fromGroup, toGroup);
				}
			} else {
				rsi.doSubSet = false;
				rsi.SubSetType = SubSetMethod.None;
			}

			if (rspMergeGraphCheck.Active) {
				rsi.MergeGraphs = true;
			} else
				rsi.MergeGraphs = false;

			try {
				rsi.sample = rspSampleCombo.ActiveText;
			}
			catch(Exception e) {
				throw new Exception ("You need to select one or all samples from the drop-down menu!");
			}

			OldRSPSize = rsi.SplitCount;
			rspMergeGraphCheck.Active = false;
			return rsi;
		}

		protected void OnExit (object sender, EventArgs e)
		{
			Application.Quit ();
		}

		protected void OnRSPCovTest (object sender, EventArgs e)
		{
			StatsOutputWindow statWind;
			if (CurrentRSPContainerType == CurrentSavedRankData.Double) {
				statWind = new StatsOutputWindow (LatestRSplitDOUBLE as RSplitContainer);
			} else {
				statWind = new StatsOutputWindow (LatestRSplitLISTDUB as RSplitContainer);
			}

			statWind.ShowAll ();
		}	

		protected void RSPOnGraphComboChange (object sender, EventArgs e)
		{
			if (rspGraphicalOutputCombo.ActiveText.Length > 3) {

				ClearComboBox (rspGraphTypeCombo);

				OutputMethGraph omg = RankSplitGraphOut [rspGraphicalOutputCombo.ActiveText];

				List<GraphType> tys = RankSplitGraphToTypes [omg];
				List<string> descs = RankSplit.GraphTypesToStrings (tys);

				foreach (string gt in descs) {
					rspGraphTypeCombo.AppendText (gt);
				}
			}
		}

		protected void OnShowImage (object sender, EventArgs e)
		{
			ImageDisplayPanel.SwitchImages ();
		}

		protected void OnSaveBitMap (object sender, EventArgs e)
		{
			string id = imgSelectCombo.ActiveText;

			if (id.Length > 2) {
				ImageDisplayPanel.SaveToFile (id, this);
			}
		}	

		protected void OnImageDraw (object o, ExposeEventArgs args)
		{
			string id = imgSelectCombo.ActiveText;

			if (id != null) {
				ImageDisplayPanel.DrawToSpace (id, args);
			}
		}
			

		public class DelegateComboMenu
		{
			ComboBox cbox;
			List<System.Action> actions;

			public DelegateComboMenu(ComboBox cb)
			{
				cbox = cb;
				actions = new List<System.Action>();
			}
			public void AddAction(System.Action a, string name)
			{
				cbox.AppendText (name);
				actions.Add (a);
			}

			public void InvokeCurrent()
			{
				if (actions.Count > 0) {
					int index = cbox.Active;
					actions [index].Invoke ();
				}
			}

		}
	}
}