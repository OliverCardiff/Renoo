using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;
using System.Drawing;
using System.Linq;

namespace GeneToAnno
{
	public static class MainData
	{
		public static Genome Genome;
		public static Dictionary<string, BioSample> Samples;

		public static string genomeFileName;
		public static string gff3FileName;
		public static string outfmt6FileName;
		public static List<string> FPKMFileNames;
		public static List<string> BAMFileNames;
		public static List<string> VariantFileNames;
		public static Dictionary<string, string> GeneToScaffDict;
		public static Dictionary<string, string> RegionToScaffDict;

		public static WindowMain MainWindow;
		public static ElementTypeDictionary TypeDict;
		public static int LoadedSeqs = 0;
		public static int LoadedVars = 0;
		public static int LoadedFKPM = 0;
		public static int LoadedBlasts = 0;

		public static List<Series> RSPToMergeSeries;

		public static List<string> VariantSamples { get { 
				List<string> lstr = new List<string> ();
				foreach (KeyValuePair<string, BioSample> kvp in Samples) {
					if (kvp.Value.HasVars) {
						lstr.Add (kvp.Key);
					}
				}
				return lstr;
			} }

		public static string ShortFileName(string str)
		{
			char [] chrs = {'/','\\'};
			string[] strs = str.Split (chrs, 50);
			int lastInd = strs.Length - 1;
			return strs [lastInd];
		}

		public static void Assemble()
		{
			Genome = new Genome ();
			ProgramState.Init ();
			AppSettings.Assemble ();
			RankSplit.Assemble ();
			FPKMFileNames = new List<string> ();
			BAMFileNames = new List<string> ();
			VariantFileNames = new List<string> ();
			TypeDict = new ElementTypeDictionary ();
			Samples = new Dictionary<string, BioSample> ();
			BioSample.ResetStaticLoader ();
			GeneToScaffDict = new Dictionary<string, string> ();
			RegionToScaffDict = new Dictionary<string, string> ();
			RSPToMergeSeries = new List<Series> ();
			StatsTester.Assemble ();
		}

		public static void UpdateLog(string _text, bool _threaded)
		{
			if (!_threaded) {
				MainWindow.UpdateLog (_text);
			} else {
				Gtk.Application.Invoke (delegate {
					MainWindow.UpdateLog (_text);
				});
			}
		}

		public static void ShowMessageWindow(string _text, bool _threaded)
		{
			if (!_threaded) {
				MainWindow.GenerateMessageWindow (_text);
			} else {
				Gtk.Application.Invoke (delegate {
					MainWindow.GenerateMessageWindow (_text);
				});
			}
		}

		public static void UnloadAll()
		{
			Genome = new Genome ();
			TypeDict = new ElementTypeDictionary ();
			FPKMFileNames = new List<string> ();
			BAMFileNames = new List<string> ();
			VariantFileNames = new List<string> ();
			ProgramState.ResetAll ();
			LoadedSeqs = 0;
			LoadedVars = 0;
		}

		public static void LoadGenome()
		{
			try{
				ProgramState.LoadedGenome = false;
				ProgramState.GenomeLoadLock = false;

				UpdateLog ("Loading Genome..", true);
				if(AppSettings.Loading.GEN_LINE_BREAK.Item) {
					Genome.LoadGenomeReturnSeq(genomeFileName);
				}
				else{
					Genome.LoadGenomeLongSeq(genomeFileName);
				}

				RankSplit.GetMethodsFromGenomeInstance();
				UpdateLog ("Loaded " + Genome.ShortFileName + " into memory..", true);

				ProgramState.LoadedGenome = true;
				ProgramState.GenomeLoadLock = true;
			}
			catch(Exception e) {
				MainWindow.GenerateMessageWindow (e.Message);
				ProgramState.GenomeLoadLock = true;
			}
		}

		public static void LoadGFF3()
		{
			try{
				ProgramState.LoadedGFF3 = false;
				ProgramState.GFF3LoadLock = false;

				GeneToScaffDict.Clear();
				RegionToScaffDict.Clear();

				UpdateLog ("Loading GFF3..", true);
				Genome.LoadGFF3 (gff3FileName);
				UpdateLog ("Loaded " + ShortFileName(gff3FileName) + " into memory..", true);
				TypeDict.FindAllEntries(Genome);
				UpdateLog ("-- Built Element Type Dictionary --", true);

				Genome.FillGeneToScaffDict(GeneToScaffDict);
				Genome.FillRegionToScaffDict(RegionToScaffDict);

				ProgramState.LoadedGFF3 = true;
				ProgramState.GFF3LoadLock = true;
			}
			catch(Exception e) {
				ShowMessageWindow(e.Message, true);
				ProgramState.GFF3LoadLock = true;
			}
		}

		public static void LoadBlastOutfmt6()
		{
			try{
				ProgramState.LoadedBlast = false;
				ProgramState.OutFmt6LoadLock = false;

				UpdateLog("Loading Blast Results..", true);
				Genome.LoadOutFmt6(outfmt6FileName);
				UpdateLog("Loaded " + ShortFileName(outfmt6FileName) + " into memory..", true);
				UpdateLog("Assigned " + MainData.LoadedBlasts + " hits to elements!", true);

				ProgramState.OutFmt6LoadLock = true;
				ProgramState.LoadedBlast = true;
			}
			catch(Exception e) {
				ShowMessageWindow (e.Message + "\n Check you are using the right format!", true);
				ProgramState.OutFmt6LoadLock = true;
			}
		}

		public static void MakeGenomeBaseRatio()
		{
			try
			{
				UpdateLog ("Fragmenting " + Genome.ShortFileName + " Into 200bp Fragments & calculcating base ratios..", true);
				List<double> ratios = Genome.FindAllBaseRatios ();
				int bcount = Math.Min(ratios.Count / 300, 40);
				DoublesHisto dHisto = new DoublesHisto (ratios, bcount, "CG Ratio", false, ModelDisplayType.Bar);

				PlotModel model = new PlotModel { Title = "For 200bp fragments: Base Ratio: CG / (AT + CG)" };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, 1, "CG Ratio");
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, dHisto.GetYPercentLimit(1), "Density");
				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;

				model.Axes.Add(axX);
				model.Axes.Add(axY);

				dHisto.AddToModel(model);
				model.IsLegendVisible = true;

				MainWindow.InsertGraph(GraphWindow.Genome, model, dHisto);
				UpdateLog ("Made a base ratio graph in the Genome tab.", true);

				ProgramState.MadeGenomeGraph = true;
			}
			catch (Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeGeneBaseRatio()
		{
			try 
			{
				UpdateLog("Scanning base ratios for each included genomic annotation/element..", true);

				Dictionary<string, List<double>> tdict = Genome.FindAllElementBaseRatios();

				UpdateLog("Making Distributions..", true);
				MultiHisto mHist = new MultiHisto(tdict, 50, ModelDisplayType.Line);

				PlotModel model = new PlotModel { Title = "Annotation Element Base Ratios: CG / (CG + AT)" };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, 1, "Ratio");
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, mHist.GetYPercentLimit(1), "Density");
				//LogarithmicAxis
				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;
				model.Axes.Add(axX);
				model.Axes.Add(axY);

				mHist.AddToModel(model);

				model.IsLegendVisible = true;
				model.LegendItemSpacing = 10;

				MainWindow.InsertGraph(GraphWindow.Gene, model, mHist);
				UpdateLog("Made a base ratio graph in the Gene tab,", true);

				ProgramState.MadeGeneGraph = true;

			} catch (Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeMethySpatialCov()
		{
			try 
			{
				Dictionary<string, List<List<double>>> tdict = Genome.GetAllSingleSpatialCoverages();
				UpdateLog("Making Distributions..", true);

				MultiBayes mBayes = new MultiBayes(tdict, ModelDisplayType.Line, MultiBayesLayout.Stacked);

				PlotModel model = new PlotModel { Title = "Global spatial distribution of read-coverage across element(s)" };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, mBayes.GetXPercentLimit(1) + 1, "Normalised division of element");
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, mBayes.GetYPercentLimit(1), "Coverage-rate odds ratio");
				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;
				model.Axes.Add(axX);
				model.Axes.Add(axY);

				mBayes.AddToModel(model);

				model.IsLegendVisible = true;

				MainWindow.InsertGraph(GraphWindow.Meth, model, mBayes);
				UpdateLog("Made a spatial-coverage graph in the Methylation tab.", true);

				ProgramState.MadeMethGraph = true;

			}
			catch(Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeMethySpatialDepthCov()
		{
			try 
			{
				Dictionary<string, List<List<double>>> tdict = Genome.GetAllSingleSpatialDepthCoverages();
				UpdateLog("Making Distributions..", true);

				MultiBayes mBayes = new MultiBayes(tdict, ModelDisplayType.Line, MultiBayesLayout.Stacked);

				PlotModel model = new PlotModel { Title = "Global spatial distribution of read-depth across element(s)" };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, mBayes.GetXPercentLimit(1) + 1, "Normalised division of element");
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, mBayes.GetYPercentLimit(1), "Odds ratio of read coverage-depth for element vs background");
				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;
				model.Axes.Add(axX);
				model.Axes.Add(axY);

				mBayes.AddToModel(model);

				model.IsLegendVisible = true;

				MainWindow.InsertGraph(GraphWindow.Meth, model, mBayes);
				UpdateLog("Made a spatial-depth-coverage graph in the Methylation tab.", true);

				ProgramState.MadeMethGraph = true;

			}
			catch(Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeMethyCoverageDistro()
		{
			try
			{
				UpdateLog("Calculating annotation element read coverages..", true);

				Dictionary<string, List<double>> tdict = Genome.GetAllElementCoverages();
				UpdateLog("Making Distributions..", true);

				MultiHisto mHist = new MultiHisto(tdict, 50, ModelDisplayType.Bar);

				PlotModel model = new PlotModel { Title = "Annotation Element Coverage Rate 0-1" };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, mHist.GetXPercentLimit(1), "Read coverage of element");
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, mHist.GetYPercentLimit(1), "Density");
				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;
				model.Axes.Add(axX);
				model.Axes.Add(axY);

				mHist.AddToModel(model);

				model.IsLegendVisible = true;

				MainWindow.InsertGraph(GraphWindow.Meth, model, mHist);
				UpdateLog("Made a coverage-rate graph in the Methylation tab.", true);

				ProgramState.MadeMethGraph = true;
			}
			catch (Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeRegionChangeMap(Dictionary<string, List<List<double>>> data, RSplitContainer<List<double>> rsc, bool mergeOld)
		{
			try{
				UpdateLog("Making Images..", true);
				RegionChangeMap rcm = new RegionChangeMap (data, ColourRank.Sequential, RankSplit.OutputBaseCount, 20, RankSplit.OutputFromStart);
				List<Bitmap> bml = rcm.GetBmps ();
				List<string> stl = rcm.GetStrs();

				MainWindow.InsertImage(bml, stl);
				MainWindow.SetRSplitDoubleContainer(rsc);
				ProgramState.ImageHasBitmap = true;
				UpdateLog("Images sent to Image tab!", true);
			}
			catch (Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeMultiBayesRankGraph(Dictionary<string, List<List<double>>> data, GraphWindow grWind, 
			string xLab, string yLab, string title, RSplitContainer<List<double>> rsc, bool mergeOld)
		{
			try
			{
				UpdateLog("Making Odds ratios..", true);
				MultiBayes.FlipFullDictionary(data);
				MultiBayes mBayes = new MultiBayes(data, ModelDisplayType.Line, MultiBayesLayout.Stacked);
				PlotModel model = new PlotModel { Title = title };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, mBayes.GetXPercentLimit(1) + 1, xLab);
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, mBayes.GetYPercentLimit(1), yLab);

				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;
				model.Axes.Add(axX);
				model.Axes.Add(axY);

				mBayes.AddToModel(model);

				if(mergeOld && RSPToMergeSeries != null)
				{
					foreach(Series sr in RSPToMergeSeries)
					{
						model.Series.Add(sr);
					}
				}
				else
				{
					if(RSPToMergeSeries != null)
					{
						RSPToMergeSeries.Clear();
					}
				}

				RSPToMergeSeries.AddRange(mBayes.GetNextSeries(title.Split(' ')[0]));

				model.IsLegendVisible = true;

				MainWindow.InsertGraph(grWind, model, mBayes);
				MainWindow.SetRSplitDoubleContainer(rsc);
				UpdateLog("Made a Spatial Rank/Split graph...", true);

				ProgramState.MadeGlobalGraph = true;
			}
			catch (Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeMultiBayesBaseRankGraph(Dictionary<string, List<List<double>>> data, GraphWindow grWind, 
			string xLab, string yLab, string title, bool fromStart, RSplitContainer<List<double>> rsc, bool mergeOld)
		{
			try
			{
				UpdateLog("Making Odds ratios..", true);
				MultiBayesPerBase mBayes = new MultiBayesPerBase(data, fromStart);
				PlotModel model = new PlotModel { Title = title };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, mBayes.GetXPercentLimit(1) + 1, xLab);
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, mBayes.GetYPercentLimit(1), yLab);

				//model.titlefontsize!
				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;
				model.Axes.Add(axX);
				model.Axes.Add(axY);

				mBayes.AddToModel(model);

				if(mergeOld && RSPToMergeSeries != null)
				{
					foreach(Series sr in RSPToMergeSeries)
					{
						model.Series.Add(sr);
					}
				}
				else
				{
					if(RSPToMergeSeries != null)
					{
						RSPToMergeSeries.Clear();
					}
				}

				RSPToMergeSeries.AddRange(mBayes.GetNextSeries(title.Split(' ')[0]));
				model.IsLegendVisible = true;

				MainWindow.InsertGraph(grWind, model, mBayes);
				MainWindow.SetRSplitDoubleContainer(rsc);
				UpdateLog("Made a Spatial Rank/Split graph...", true);

				ProgramState.MadeGlobalGraph = true;
			}
			catch (Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeRankLineGraph(Dictionary<string, List<double>> data, GraphWindow grWind, 
			string xLab, string yLab, string title, RSplitContainer<double> rsc, bool mergeOld)
		{
			try
			{
				UpdateLog("Summarising groups..", true);
				RankLine rLine = new RankLine(data, "Rank-Groups by Mean", ModelDisplayType.Line);
				PlotModel model = new PlotModel { Title = title };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, rLine.GetXPercentLimit(1) + 1, xLab);
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, rLine.GetYPercentLimit(1), yLab);

				//model.titlefontsize!
				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;
				model.Axes.Add(axX);
				model.Axes.Add(axY);

				rLine.AddToModel(model);

				if(mergeOld && RSPToMergeSeries != null)
				{
					foreach(Series sr in RSPToMergeSeries)
					{
						model.Series.Add(sr);
					}
				}
				else
				{
					if(RSPToMergeSeries != null)
					{
						RSPToMergeSeries.Clear();
					}
				}
				RSPToMergeSeries.AddRange(rLine.GetNextSeries(title.Split(' ')[0]));

				model.IsLegendVisible = true;

				MainWindow.InsertGraph(grWind, model, rLine);
				MainWindow.SetRSplitDoubleContainer(rsc);
				UpdateLog("Made a Mean-Error Rank/Split Bar graph...", true);

				ProgramState.MadeGlobalGraph = true;
			}
			catch (Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeMultiHistoRankGraph(Dictionary<string, List<double>> data, GraphWindow grWind, 
			string xLab, string yLab, string title, RSplitContainer<double> rsc, bool mergeOld)
		{
			try
			{
				UpdateLog("Making Odds ratios..", true);

				MultiHisto mHist = new MultiHisto(data, 100, ModelDisplayType.Line);
				PlotModel model = new PlotModel { Title = title };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, mHist.GetXPercentLimit(1) + 1, xLab);
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, mHist.GetYPercentLimit(1), yLab);

				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;
				model.Axes.Add(axX);
				model.Axes.Add(axY);

				mHist.AddToModel(model);

				if(mergeOld && RSPToMergeSeries != null)
				{
					foreach(Series sr in RSPToMergeSeries)
					{
						model.Series.Add(sr);
					}
				}
				else
				{
					if(RSPToMergeSeries != null)
					{
						RSPToMergeSeries.Clear();
					}
				}
				RSPToMergeSeries.AddRange(mHist.GetNextSeries(title.Split(' ')[0]));

				model.IsLegendVisible = true;

				MainWindow.InsertGraph(grWind, model, mHist);
				MainWindow.SetRSplitDoubleContainer(rsc);
				UpdateLog("Made a density-based Rank/Split graph...", true);

				ProgramState.MadeGlobalGraph = true;
			}
			catch (Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeMethyCovDepthScoreDistro()
		{
			try
			{
				UpdateLog("Calculating annotation element read depth-coverage scores..", true);

				Dictionary<string, List<double>> tdict = Genome.GetAllElementDepthScores();

				UpdateLog("Making Distributions..", true);
				MultiHisto mHist = new MultiHisto(tdict, 100, ModelDisplayType.Line);

				PlotModel model = new PlotModel { Title = "Annotation Element per-base read coverage depth" };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, mHist.GetXPercentLimit(0.9), "Read depth-coverage of element");
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, mHist.GetYPercentLimit(1), "Density");
				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;
				model.Axes.Add(axX);
				model.Axes.Add(axY);

				mHist.AddToModel(model);

				model.IsLegendVisible = true;

				MainWindow.InsertGraph(GraphWindow.Meth, model, mHist);
				UpdateLog("Made a depth-coverage score graph in the SAM-read tab.", true);

				ProgramState.MadeMethGraph = true;
			}
			catch (Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static void MakeGeneLengthDistro()
		{
			try
			{
				UpdateLog("Scanning annotation element lengths..", true);

				Dictionary<string, List<double>> tdict = Genome.GetAllElementLengths();

				UpdateLog("Making Distributions..", true);
				MultiHisto mHist = new MultiHisto(tdict, 50, ModelDisplayType.Bar);

				PlotModel model = new PlotModel { Title = "Annotation Element Lengths in Base-Pairs" };
				LinearAxis axX = new LinearAxis(AxisPosition.Bottom, 0, mHist.GetXPercentLimit(0.90), "Length (BP)");
				LinearAxis axY = new LinearAxis(AxisPosition.Left, 0, mHist.GetYPercentLimit(0.95), "Density");
				model.TitlePadding = 10;
				axX.AxisTitleDistance = 10;
				axY.AxisTitleDistance = 10;
				model.Axes.Add(axX);
				model.Axes.Add(axY);

				mHist.AddToModel(model);

				model.IsLegendVisible = true;

				MainWindow.InsertGraph(GraphWindow.Gene, model, mHist);
				UpdateLog("Made a length distribution graph in the Gene tab.", true);

				ProgramState.MadeGeneGraph = true;
			}
			catch (Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static long ScanFileForModulo(string fileName, int division)
		{
			int count = 0;
			MainData.UpdateLog ("Scanning " + ShortFileName(fileName) + "...", true);
			using (StreamReader sr = new StreamReader (fileName)) {
				while ((sr.ReadLine ()) != null) {
					count++;
				}
			}
			return count / division;
		}

		public static void SendSampleDefsToSamples(List<PreSampleDefinition> prel)
		{
			try
			{				
				UpdateLog ("Assigned Data to Sample IDs, loading data now..", false);
				Thread o = new Thread (new ParameterizedThreadStart (RunSampleAllocationLoading));
				o.IsBackground = true;
				o.Start ((object)prel);
			}
			catch(Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		private static void RunSampleAllocationLoading(object ob)
		{
			try
			{
				BioSample.ResetStaticLoader ();
				List<PreSampleDefinition> prel = (List<PreSampleDefinition>)ob;

				foreach (PreSampleDefinition pre in prel) {
					if (!Samples.ContainsKey (pre.ID)) {
						Samples.Add (pre.ID, new BioSample (pre.ID));
					}
					Samples [pre.ID].RecieveDefinition (pre);
				}
			
				BioSample.LoadAllStatic();
			}
			catch(Exception e) {
				ShowMessageWindow (e.Message, true);
			}
		}

		public static List<string> GetAllCurrentSampleIDs()
		{
			return new List<string> (Samples.Keys);
		}

		public static void RunElementFilter(List<ElementFilterInstruction> ins)
		{
			UpdateLog ("Applying length-based filters to GFF3 elements..", false);
			Thread t = new Thread (new ParameterizedThreadStart (RunElementFilterObjCast));
			t.Start ((object)ins);
		}

		private static void RunElementFilterObjCast(object ob)
		{
			List<ElementFilterInstruction> ins = (List<ElementFilterInstruction>)ob;
			Genome.RunElementFilter (ins);
			UpdateLog ("GFF3 filtering complete!", true);
		}

		public static void TestElementCoverage()
		{
			Gene g = new Gene ("abc", "gene", 250, 500, Sense.Sense);
			g.AddFileElement ("exon", 250, 350);
			g.AddFileElement ("exon", 400, 500);
			g.AddFileElement ("mRNA", 250, 500);

			g.SendRead (new SeqRead (null, 250, 275));
			g.SendRead (new SeqRead (null, 260, 300));
			g.SendRead (new SeqRead (null, 275, 300));
			g.SendRead (new SeqRead (null, 300, 325));
			g.SendRead (new SeqRead (null, 350, 375));
			g.SendRead (new SeqRead (null, 375, 400));
			g.SendRead (new SeqRead (null, 400, 425));
			g.SendRead (new SeqRead (null, 425, 450));

			List<double> rec;

			foreach (KeyValuePair<string, List<GeneElement>> kvp in g.Elements) {
				foreach (GeneElement ge in kvp.Value) {
					ge.TryGetCoverageDepthByDivision (20, out rec);
				}
			}

			g.TryGetCoverageDepthByDivision (20, out rec);
		
		}
	}
}

