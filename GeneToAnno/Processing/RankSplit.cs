using System;
using System.Collections.Generic;
using OxyPlot;
using Gtk;

namespace GeneToAnno
{
	public enum RankMethod{ ExprHiLo, ExprTot, ExprNorm, BlastE, BlastID, VarCount, VarWithin, VarBetween, MethyCov, MethyDepthCov };
	public enum OutputMethText{ BaseSeq, BlastID, FastaBase, SeqNames };
	public enum OutputMethGraph { MethyCov, MethyDepthCov, Expr, ExprVar, ExprNorm, Variant, BlastE, BlastID };
	public enum SubSequenceDivision { AbsoluteBases, Normalised, None };
	public enum SubSetMethod {Current, Previous, None};

	public enum GraphType{ Spatial, Means, Histo, Image };

	public class RankSplitInfo
	{
		public RankMethod RMethod;
		public OutputMethGraph Graph;
		public GraphType GraphingType;
		public OutputMethText Text;
		public string SeqFeature;
		public string SeqFeature2;
		public bool doGraph;
		public bool doSubSeq;
		public SubSequenceDivision SeqDiv;
		public int divStart;
		public int divEnd;
		public bool fromStart;
		public int baseCount;
		public string sample;

		public bool doSubSeqOut;
		public SubSequenceDivision SeqDivOut;
		public int divStartOut;
		public int divEndOut;
		public bool fromStartOut;
		public int baseCountOut;

		public int SplitCount;

		public bool doSubSet;
		public SubSetMethod SubSetType;
		public int subCurMainRank;
		public int subCurSplitCount;
		public int subOldStartRank;
		public int subOldEndRank;
		public List<string> oldIDlist;

		public bool MergeGraphs;

		public RankSplitInfo(RankMethod rmeth, OutputMethGraph mGraph, string seqFeature)
		{
			RMethod = rmeth;
			Graph = mGraph;
			//Text = null;
			SeqFeature = seqFeature;
			doGraph = true;
			doSubSeq = false;
			SeqDiv = SubSequenceDivision.None;
			SeqDivOut = SubSequenceDivision.None;
		}
		public RankSplitInfo(RankMethod rmeth, OutputMethText mText, string seqFeature)
		{
			RMethod = rmeth;
			//Graph = null;
			Text = mText;
			SeqFeature = seqFeature;
			doGraph = false;
			doSubSeq = false;
			SeqDiv = SubSequenceDivision.None;
			SeqDivOut = SubSequenceDivision.None;
		}
		public RankSplitInfo(RankMethod rmeth, OutputMethGraph mGraph, string seqFeature, int divBegin, int divFinish)
		{
			RMethod = rmeth;
			Graph = mGraph;
			//Text = null;
			SeqFeature = seqFeature;
			doGraph = true;
			doSubSeq = true;
			SeqDiv = SubSequenceDivision.Normalised;
			SeqDivOut = SubSequenceDivision.None;
			divStart = divBegin;
			divEnd = divFinish;
		}
		public RankSplitInfo(RankMethod rmeth, OutputMethText mText, string seqFeature, int divBegin, int divFinish)
		{
			RMethod = rmeth;
			//Graph = null;
			Text = mText;
			SeqFeature = seqFeature;
			doGraph = false;
			doSubSeq = true;
			SeqDiv = SubSequenceDivision.Normalised;
			SeqDivOut = SubSequenceDivision.None;
			divStart = divBegin;
			divEnd = divFinish;
		}
		public RankSplitInfo(RankMethod rmeth, OutputMethGraph mGraph, string seqFeature, int bases, bool goFromStart)
		{
			RMethod = rmeth;
			Graph = mGraph;
			//Text = null;
			SeqFeature = seqFeature;
			doGraph = true;
			doSubSeq = true;
			SeqDiv = SubSequenceDivision.AbsoluteBases;
			SeqDivOut = SubSequenceDivision.None;
			baseCount = bases;
			fromStart = goFromStart;
		}
		public RankSplitInfo(RankMethod rmeth, OutputMethText mText, string seqFeature, int bases, bool goFromStart)
		{
			RMethod = rmeth;
			//Graph = null;
			Text = mText;
			SeqFeature = seqFeature;
			doGraph = false;
			doSubSeq = true;
			SeqDiv = SubSequenceDivision.AbsoluteBases;
			SeqDivOut = SubSequenceDivision.None;
			baseCount = bases;
			fromStart = goFromStart;
		}
	}

	public static class RankSplit
	{
		public static Dictionary<RankMethod, string> MethodDesc;

		private static Dictionary<OutputMethGraph, string> xHistoLabDesc;
		private static Dictionary<OutputMethGraph, string> yHistoLabDesc;

		private static Dictionary<OutputMethGraph, string> xSpatialLabDesc;
		private static Dictionary<OutputMethGraph, string> ySpatialLabDesc;

		private static Dictionary<OutputMethGraph, string> xMeansLabDesc;
		private static Dictionary<OutputMethGraph, string> yMeansLabDesc;

		private static Dictionary<OutputMethText, string> OutputTextDesc;
		private static Dictionary<OutputMethGraph, string> OutputGraphDesc;

		private static Dictionary<GraphType, string> GraphTypeDesc;
		private static Dictionary<OutputMethGraph, List<GraphType>> GraphToTypes;

		public static Dictionary<RankMethod, Func<string, Dictionary<string, double>>> Methods;

		public static string FeatureToUse;
		public static string SampleToUse;
		public static bool UseSample;

		public static SubSequenceDivision RankMethodSubSequence;
		public static SubSequenceDivision OutputMethodSubSequence;

		public static int RankStartDiv;
		public static int RankEndDiv;
		public static int OutputStartDiv;
		public static int OutputEndDiv;

		public static int RankBaseCount;
		public static int OutputBaseCount;
		public static bool RankFromStart;
		public static bool OutputFromStart;

		public static string LastFileName;

		public static void Assemble ()
		{
			FeatureToUse = "";
			MethodDesc = new Dictionary<RankMethod, string> ();
			MethodDesc.Add (RankMethod.ExprHiLo, "Feature Expression Range");
			MethodDesc.Add (RankMethod.ExprTot, "Feature Expression");
			MethodDesc.Add (RankMethod.BlastE, "Blast 'e' Value");
			MethodDesc.Add (RankMethod.BlastID, "Blast perc. Identity");
			MethodDesc.Add (RankMethod.MethyCov, "SAM Read Coverage");
			MethodDesc.Add (RankMethod.MethyDepthCov, "SAM Read Depth-Coverage");
			MethodDesc.Add (RankMethod.VarCount, "Variant Occurance Rate");
			MethodDesc.Add (RankMethod.VarBetween, "Between-samps. Var. Occurance");
			MethodDesc.Add (RankMethod.VarWithin, "Within-samps. Var. Occurance");
			MethodDesc.Add (RankMethod.ExprNorm, "Feat. Expr. Range Normalised");

			OutputTextDesc = new Dictionary<OutputMethText, string> ();
			OutputTextDesc.Add (OutputMethText.BaseSeq, "Base Sequences Ranked");
			OutputTextDesc.Add (OutputMethText.BlastID, "Blast match ID Ranked");
			OutputTextDesc.Add (OutputMethText.FastaBase, "Base Seqs. as Fasta");
			OutputTextDesc.Add (OutputMethText.SeqNames, "Seq. Feat. to Read Name");

			OutputGraphDesc = new Dictionary<OutputMethGraph, string> ();
			OutputGraphDesc.Add (OutputMethGraph.Expr, "Gene Expression");
			OutputGraphDesc.Add (OutputMethGraph.ExprVar, "Gene Expr. Variation");
			OutputGraphDesc.Add (OutputMethGraph.ExprNorm, "G.Expr. Range-Normalised");

			OutputGraphDesc.Add (OutputMethGraph.MethyCov, "SAM Read Coverage (binary)");
			OutputGraphDesc.Add (OutputMethGraph.MethyDepthCov, "SAM depth-Coverages");

			OutputGraphDesc.Add (OutputMethGraph.Variant, "Variant Occurence Rate Means");
			OutputGraphDesc.Add (OutputMethGraph.BlastE, "Blast E-Value Means");
			OutputGraphDesc.Add (OutputMethGraph.BlastID, "Blast % Identity Means");

			xHistoLabDesc = new Dictionary<OutputMethGraph, string> ();
			yHistoLabDesc = new Dictionary<OutputMethGraph, string> ();

			xHistoLabDesc.Add (OutputMethGraph.Expr, "FKPM Gene Expression Distribution");
			yHistoLabDesc.Add (OutputMethGraph.Expr, "Density");
			xHistoLabDesc.Add (OutputMethGraph.ExprVar, "Distributions of Variance of FKPM Scores");
			yHistoLabDesc.Add (OutputMethGraph.ExprVar, "Density");
			xHistoLabDesc.Add (OutputMethGraph.ExprNorm, "Distribution of FKPMs normalised to Between-Samples range");
			yHistoLabDesc.Add (OutputMethGraph.ExprNorm, "Density");
			xHistoLabDesc.Add (OutputMethGraph.MethyDepthCov, "Distribution of SAM-read depth-coverage scores");
			yHistoLabDesc.Add (OutputMethGraph.MethyDepthCov, "Density");
			xHistoLabDesc.Add (OutputMethGraph.BlastE, "Distribution of Blast e-values");
			yHistoLabDesc.Add (OutputMethGraph.BlastE, "Density");
			xHistoLabDesc.Add (OutputMethGraph.BlastID, "Distribution of BLAST perc. Identities");
			yHistoLabDesc.Add (OutputMethGraph.BlastID, "Density");
			xHistoLabDesc.Add (OutputMethGraph.Variant, "Distribution of Variant Counts");
			yHistoLabDesc.Add (OutputMethGraph.Variant, "Density");

			xMeansLabDesc = new Dictionary<OutputMethGraph, string> ();
			yMeansLabDesc = new Dictionary<OutputMethGraph, string> ();

			yMeansLabDesc.Add (OutputMethGraph.Expr, "Mean Gene Expression");
			xMeansLabDesc.Add (OutputMethGraph.Expr, "Rank Group Number");
			yMeansLabDesc.Add (OutputMethGraph.ExprNorm, "Mean FKPMs normalised to Between-Samples range - per Group");
			xMeansLabDesc.Add (OutputMethGraph.ExprNorm, "Rank Group Number");
			yMeansLabDesc.Add (OutputMethGraph.ExprVar, "Mean Gene Expression Variation");
			xMeansLabDesc.Add (OutputMethGraph.ExprVar, "Rank Group Number");
			yMeansLabDesc.Add (OutputMethGraph.MethyCov, "Mean SAM-Read Coverage Probability");
			xMeansLabDesc.Add (OutputMethGraph.MethyCov, "Rank Group Number");
			yMeansLabDesc.Add (OutputMethGraph.MethyDepthCov, "Mean SAM-Read Coverage-depth Score");
			xMeansLabDesc.Add (OutputMethGraph.MethyDepthCov, "Rank Group Number");
			yMeansLabDesc.Add (OutputMethGraph.Variant, "Mean Variant Occurance Rate");
			xMeansLabDesc.Add (OutputMethGraph.Variant, "Rank Group Number");
			yMeansLabDesc.Add (OutputMethGraph.BlastE, "Mean Blast E-value");
			xMeansLabDesc.Add (OutputMethGraph.BlastE, "Rank Group Number");
			yMeansLabDesc.Add (OutputMethGraph.BlastID, "Mean Blast Percentage Identity");
			xMeansLabDesc.Add (OutputMethGraph.BlastID, "Rank Group Number");

			xSpatialLabDesc = new Dictionary<OutputMethGraph, string> ();
			ySpatialLabDesc = new Dictionary<OutputMethGraph, string> ();

			xSpatialLabDesc.Add (OutputMethGraph.MethyCov, "Normalised Division of Element");
			ySpatialLabDesc.Add (OutputMethGraph.MethyCov, "Spatial SAM-read coverage odds ratios");
			xSpatialLabDesc.Add (OutputMethGraph.MethyDepthCov, "Normalised Division of Element");
			ySpatialLabDesc.Add (OutputMethGraph.MethyDepthCov, "Spatial SAM-read depth-coverage odds ratios");

			GraphTypeDesc = new Dictionary<GraphType, string> ();

			GraphTypeDesc.Add (GraphType.Histo, "Distributions");
			GraphTypeDesc.Add (GraphType.Means, "Group Means");
			GraphTypeDesc.Add (GraphType.Spatial, "Spatial");
			GraphTypeDesc.Add (GraphType.Image, "Full Visual");

			GraphToTypes = new Dictionary<OutputMethGraph, List<GraphType>> ();
			GraphToTypes.Add (OutputMethGraph.BlastE, new List<GraphType> ());
			GraphToTypes [OutputMethGraph.BlastE].Add (GraphType.Histo);
			GraphToTypes [OutputMethGraph.BlastE].Add (GraphType.Means);

			GraphToTypes.Add (OutputMethGraph.BlastID, new List<GraphType> ());
			GraphToTypes [OutputMethGraph.BlastID].Add (GraphType.Histo);
			GraphToTypes [OutputMethGraph.BlastID].Add (GraphType.Means);

			GraphToTypes.Add (OutputMethGraph.Variant, new List<GraphType> ());
			GraphToTypes [OutputMethGraph.Variant].Add (GraphType.Histo);
			GraphToTypes [OutputMethGraph.Variant].Add (GraphType.Means);

			GraphToTypes.Add (OutputMethGraph.Expr, new List<GraphType> ());
			GraphToTypes [OutputMethGraph.Expr].Add (GraphType.Histo);
			GraphToTypes [OutputMethGraph.Expr].Add (GraphType.Means);

			GraphToTypes.Add (OutputMethGraph.ExprVar, new List<GraphType> ());
			GraphToTypes [OutputMethGraph.ExprVar].Add (GraphType.Histo);
			GraphToTypes [OutputMethGraph.ExprVar].Add (GraphType.Means);

			GraphToTypes.Add (OutputMethGraph.ExprNorm, new List<GraphType> ());
			GraphToTypes [OutputMethGraph.ExprNorm].Add (GraphType.Histo);
			GraphToTypes [OutputMethGraph.ExprNorm].Add (GraphType.Means);

			GraphToTypes.Add (OutputMethGraph.MethyCov, new List<GraphType> ());
			GraphToTypes [OutputMethGraph.MethyCov].Add (GraphType.Means);
			GraphToTypes [OutputMethGraph.MethyCov].Add (GraphType.Image);
			GraphToTypes [OutputMethGraph.MethyCov].Add (GraphType.Spatial);

			GraphToTypes.Add (OutputMethGraph.MethyDepthCov, new List<GraphType> ());
			GraphToTypes [OutputMethGraph.MethyDepthCov].Add (GraphType.Histo);
			GraphToTypes [OutputMethGraph.MethyDepthCov].Add (GraphType.Means);
			GraphToTypes [OutputMethGraph.MethyDepthCov].Add (GraphType.Image);
			GraphToTypes [OutputMethGraph.MethyDepthCov].Add (GraphType.Spatial);
		}
		
		public static void GetMethodsFromGenomeInstance()
		{
			Methods = new Dictionary<RankMethod, Func<string, Dictionary<string, double>>> ();
			Methods.Add (RankMethod.BlastE, MainData.Genome.RSPBlastEValue);
			Methods.Add (RankMethod.BlastID, MainData.Genome.RSPBlastPercID);

			Methods.Add (RankMethod.ExprTot, MainData.Genome.RSPExprValue);
			Methods.Add (RankMethod.ExprHiLo, MainData.Genome.RSPExprRange);
			Methods.Add (RankMethod.ExprNorm, MainData.Genome.RSPExprNorm);

			Methods.Add (RankMethod.VarCount, MainData.Genome.RSPVarCount);
			Methods.Add (RankMethod.VarWithin, MainData.Genome.RSPVarAllHet);
			Methods.Add (RankMethod.VarBetween, MainData.Genome.RSPVarAllHom);

			Methods.Add (RankMethod.MethyCov, MainData.Genome.RSPMethylCoverage);
			Methods.Add (RankMethod.MethyDepthCov, MainData.Genome.RSPMethylDepthCoverage);
		}

		public static Dictionary<string, OutputMethText> GetRelevantOutputText()
		{
			Dictionary<string, OutputMethText> dict = new Dictionary<string, OutputMethText> ();

			if (ProgramState.LoadedBlast) {
				dict.Add (OutputTextDesc [OutputMethText.BlastID], OutputMethText.BlastID);
			}

			dict.Add(OutputTextDesc [OutputMethText.BaseSeq], OutputMethText.BaseSeq);
			dict.Add(OutputTextDesc [OutputMethText.FastaBase], OutputMethText.FastaBase);

			if (AppSettings.Loading.LOAD_SEQ_NAMES.Item) {
				dict.Add (OutputTextDesc [OutputMethText.SeqNames], OutputMethText.SeqNames);
			}

			return dict;
		}

		public static Dictionary<string, OutputMethGraph> GetRelevantOutputGraph()
		{
			Dictionary<string, OutputMethGraph> dict = new Dictionary<string, OutputMethGraph> ();

			if (ProgramState.LoadedFPKM) {
				dict.Add (OutputGraphDesc [OutputMethGraph.Expr], OutputMethGraph.Expr);
				dict.Add (OutputGraphDesc [OutputMethGraph.ExprVar], OutputMethGraph.ExprVar);
				dict.Add (OutputGraphDesc [OutputMethGraph.ExprNorm], OutputMethGraph.ExprNorm);
			}
			if (ProgramState.LoadedVariants) {
				dict.Add (OutputGraphDesc [OutputMethGraph.Variant], OutputMethGraph.Variant);
			}

			if (ProgramState.LoadedBlast) {
				dict.Add (OutputGraphDesc [OutputMethGraph.BlastE], OutputMethGraph.BlastE);
				dict.Add (OutputGraphDesc [OutputMethGraph.BlastID], OutputMethGraph.BlastID);
			}

			dict.Add (OutputGraphDesc [OutputMethGraph.MethyDepthCov], OutputMethGraph.MethyDepthCov);
			dict.Add (OutputGraphDesc [OutputMethGraph.MethyCov], OutputMethGraph.MethyCov);

			return dict;
		}

		public static Dictionary<string, BioSample> GetRelevantSamples()
		{
			Dictionary<string, BioSample> samp = new Dictionary<string, BioSample> ();

			foreach (KeyValuePair<string, BioSample> smp in MainData.Samples) {
				if (smp.Value.HasBAM) {
					samp.Add (smp.Key, smp.Value);
				}
			}
			samp.Add ("All", null);
			return samp;
		}

		public static Dictionary<string, RankMethod> GetRelevantMethods()
		{
			Dictionary<string, RankMethod> dict = new Dictionary<string, RankMethod> ();

			if (ProgramState.LoadedBlast) {
				dict.Add (MethodDesc [RankMethod.BlastE], RankMethod.BlastE);
				dict.Add (MethodDesc [RankMethod.BlastID], RankMethod.BlastID);
			}

			if (ProgramState.LoadedFPKM) {
				dict.Add (MethodDesc [RankMethod.ExprTot], RankMethod.ExprTot);
				dict.Add (MethodDesc [RankMethod.ExprHiLo], RankMethod.ExprHiLo);
				dict.Add (MethodDesc [RankMethod.ExprNorm], RankMethod.ExprNorm);
			}

			if (ProgramState.LoadedVariants) {
				dict.Add (MethodDesc [RankMethod.VarCount], RankMethod.VarCount);
				dict.Add (MethodDesc [RankMethod.VarBetween], RankMethod.VarBetween);
				dict.Add (MethodDesc [RankMethod.VarWithin], RankMethod.VarWithin);
			}

			dict.Add (MethodDesc [RankMethod.MethyDepthCov], RankMethod.MethyDepthCov);
			dict.Add (MethodDesc [RankMethod.MethyCov], RankMethod.MethyCov);

			return dict;
		}

		public static Dictionary<string, GraphType> GetRelevantGraphTypes()
		{
			Dictionary<string, GraphType> redic = new Dictionary<string, GraphType> ();

			redic.Add (GraphTypeDesc [GraphType.Histo], GraphType.Histo);
			redic.Add (GraphTypeDesc [GraphType.Image], GraphType.Image);
			redic.Add (GraphTypeDesc [GraphType.Means], GraphType.Means);
			redic.Add (GraphTypeDesc [GraphType.Spatial], GraphType.Spatial);

			return redic;
		}

		public static Dictionary<OutputMethGraph, List<GraphType>> GetGraphTypes()
		{
			return GraphToTypes;
		}

		public static List<string> GraphTypesToStrings(List<GraphType> lgt)
		{
			List<string> lstr = new List<string> ();

			foreach (GraphType g in lgt) {
				lstr.Add (GraphTypeDesc [g]);	
			}
			return lstr;
		}

		public static List<string> GetRelevantFeatures()
		{
			return MainData.TypeDict.GetAllTypes ();
		}

		public static void ProcessRequest(object TheInfo)
		{
			try
			{
				RankSplitInfo info = TheInfo as RankSplitInfo;
				string fName = LastFileName;
				bool keepGoing = true;
				bool seqMatch = false;

				RankMethodSubSequence = info.SeqDiv;
				OutputMethodSubSequence = info.SeqDivOut;

				if (RankMethodSubSequence == SubSequenceDivision.Normalised) {
					RankStartDiv = info.divStart;
					RankEndDiv = info.divEnd;
				}
				if (OutputMethodSubSequence == SubSequenceDivision.Normalised) {
					OutputStartDiv = info.divStartOut;
					OutputEndDiv = info.divEndOut;
				}
				if (RankMethodSubSequence == SubSequenceDivision.AbsoluteBases) {
					RankBaseCount = info.baseCount;
					RankFromStart = info.fromStart;
				}
				if (OutputMethodSubSequence == SubSequenceDivision.AbsoluteBases) {
					OutputFromStart = info.fromStartOut;
					OutputBaseCount = info.baseCountOut;
				}

				if (info.GraphingType == GraphType.Image && OutputMethodSubSequence != SubSequenceDivision.AbsoluteBases) {
					OutputMethodSubSequence = SubSequenceDivision.AbsoluteBases;
					OutputFromStart = !AppSettings.Output.DEFAULT_IMG_FROMEND.Item;
					OutputBaseCount = AppSettings.Output.DEFAULT_IMG_BASES.Item;
				}

				if (info.sample != "All") {
					SampleToUse = info.sample;
					UseSample = true;
				} else {
					UseSample = false;
				}

				if (info.SeqFeature == info.SeqFeature2) {
					seqMatch = true;
				} else
					seqMatch = false;
				
				if (keepGoing) {
					
					RSplitContainer<string> rspcString = new RSplitContainer<string> ("none", false);
					RSplitContainer<double> rspcDouble = new RSplitContainer<double> (0, false);
					RSplitContainer<List<double>> rspcListDouble = new RSplitContainer<List<double>> (null, true);

					Dictionary<string, double> rankDict = Methods [info.RMethod].Invoke (info.SeqFeature);

					Dictionary<string, string> baseDict;
					Dictionary<string, double> singleDict;
					Dictionary<string, List<double>> listDict;

					if (info.doGraph == false) {
						switch (info.Text) {
						case OutputMethText.BlastID:
							baseDict = MainData.Genome.RSPOUTBlastHitID (info.SeqFeature);
							rspcString.MergeDictosToList (rankDict, baseDict, true, seqMatch);
							ProcessingClass.WriteDataFile (fName, rspcString.GetData (false));
							break;
						case OutputMethText.BaseSeq:
							baseDict = MainData.Genome.RSPOUTBaseSeq (info.SeqFeature);
							rspcString.MergeDictosToList (rankDict, baseDict, false, seqMatch);
							ProcessingClass.WriteDataFile (fName, rspcString.GetData (false));
							break;
						case OutputMethText.FastaBase:
							baseDict = MainData.Genome.RSPOUTBaseSeq (info.SeqFeature);
							rspcString.MergeDictosToList (rankDict, baseDict, false, seqMatch);
							rspcString.WriteFastaFile (fName);
							break;
						case OutputMethText.SeqNames:
							baseDict = MainData.Genome.RSPOUTReadName (info.SeqFeature);
							rspcString.MergeDictosToList (rankDict, baseDict, false, seqMatch);
							ProcessingClass.WriteDataFile (fName, rspcString.GetData (false));
							break;
						default:
							break;
						}
					} else {
						string yLab; string xLab;
						SetLabels (info.Graph, info.GraphingType, out xLab, out yLab);
						string title = info.SeqFeature + " ";

						if (info.doSubSeq) {
							title += "from ";
							if (info.SeqDiv == SubSequenceDivision.AbsoluteBases) {
								title += info.baseCount + " ";
								if (info.fromStart) {
									title += "from start ";
								} else {
									title += "to end ";
								}
							} else {
								title += "division " + info.divStart + "/20 to " + info.divEnd + "/20 ";
							}
						}
						title += "ranked by " + MethodDesc [info.RMethod] + " and split into " + info.SplitCount + " groups";
						Dictionary<string, List<double>> procCont;
						Dictionary<string, List<List<double>>> procContList;

						switch (info.Graph) {
						case OutputMethGraph.Expr:
							singleDict = MainData.Genome.RSPOUTExprValue (info.SeqFeature2);
							procCont = ProcessRSplitContainer<double> (rspcDouble, info, rankDict, singleDict, seqMatch);
							if (info.GraphingType == GraphType.Histo) {
								MainData.MakeMultiHistoRankGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							} else {
								MainData.MakeRankLineGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							}
							break;
						case OutputMethGraph.ExprNorm:
							singleDict = MainData.Genome.RSPOUTExprNorm (info.SeqFeature2);
							procCont = ProcessRSplitContainer<double> (rspcDouble, info, rankDict, singleDict, seqMatch);
							if (info.GraphingType == GraphType.Histo) {
								MainData.MakeMultiHistoRankGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							} else {
								MainData.MakeRankLineGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							}
							break;
						case OutputMethGraph.ExprVar:
							singleDict = MainData.Genome.RSPOUTExprRange (info.SeqFeature2);
							procCont = ProcessRSplitContainer<double> (rspcDouble, info, rankDict, singleDict, seqMatch);
							if (info.GraphingType == GraphType.Histo) {
								MainData.MakeMultiHistoRankGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							} else {
								MainData.MakeRankLineGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							}
							break;
						case OutputMethGraph.MethyCov:
							if (info.GraphingType == GraphType.Spatial) {
								listDict = MainData.Genome.RSPOUTMethylCoverageDivision (info.SeqFeature2);
								procContList = ProcessRSplitContainer<List<double>> (rspcListDouble, info, rankDict, listDict, seqMatch);
								if (info.SeqDivOut != SubSequenceDivision.AbsoluteBases) {
									MainData.MakeMultiBayesRankGraph (procContList, GraphWindow.Global, xLab, yLab, title, rspcListDouble, info.MergeGraphs);
								} else {
									MainData.MakeMultiBayesBaseRankGraph (procContList, GraphWindow.Global, xLab, yLab, title, info.fromStartOut, rspcListDouble, info.MergeGraphs);
								}
							} else if (info.GraphingType == GraphType.Means) {
								singleDict = MainData.Genome.RSPOUTQuantMethyCov (info.SeqFeature2);
								procCont = ProcessRSplitContainer<double> (rspcDouble, info, rankDict, singleDict, seqMatch);
								MainData.MakeRankLineGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							} else if (info.GraphingType == GraphType.Image) {
								//TODO: Make the image!
							}
							break;
						case OutputMethGraph.MethyDepthCov:
							if (info.GraphingType == GraphType.Spatial) {
								listDict = MainData.Genome.RSPOUTMethylCoverageDepthDivision (info.SeqFeature2);
								procContList = ProcessRSplitContainer<List<double>> (rspcListDouble, info, rankDict, listDict, seqMatch);
								if (info.SeqDivOut != SubSequenceDivision.AbsoluteBases) {
									MainData.MakeMultiBayesRankGraph (procContList, GraphWindow.Global, xLab, yLab, title, rspcListDouble, info.MergeGraphs);
								} else {
									MainData.MakeMultiBayesBaseRankGraph (procContList, GraphWindow.Global, xLab, yLab, title, info.fromStartOut, rspcListDouble, info.MergeGraphs);
								}
							} else if (info.GraphingType == GraphType.Means) {
								singleDict = MainData.Genome.RSPOUTQuantMethyDepthCov (info.SeqFeature2);
								procCont = ProcessRSplitContainer<double> (rspcDouble, info, rankDict, singleDict, seqMatch);
								MainData.MakeRankLineGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							} else if (info.GraphingType == GraphType.Histo) {
								singleDict = MainData.Genome.RSPOUTMethylDepthTotal (info.SeqFeature2);
								procCont = ProcessRSplitContainer<double> (rspcDouble, info, rankDict, singleDict, seqMatch);
								MainData.MakeMultiHistoRankGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							} else if (info.GraphingType == GraphType.Image) {
								listDict = MainData.Genome.RSPOUTMethylCoverageDepthDivision (info.SeqFeature2);
								procContList = ProcessRSplitContainer<List<double>> (rspcListDouble, info, rankDict, listDict, seqMatch);
								MainData.MakeRegionChangeMap (procContList, rspcListDouble, info.MergeGraphs);
							}
							break;
						case OutputMethGraph.BlastE:
							singleDict = MainData.Genome.RSPBlastEValue (info.SeqFeature2);
							procCont = ProcessRSplitContainer<double> (rspcDouble, info, rankDict, singleDict, seqMatch);
							if (info.GraphingType == GraphType.Means) {
								MainData.MakeRankLineGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							} else {
								MainData.MakeMultiHistoRankGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							}
							break;
						case OutputMethGraph.BlastID:
							singleDict = MainData.Genome.RSPBlastPercID (info.SeqFeature2);
							procCont = ProcessRSplitContainer<double> (rspcDouble, info, rankDict, singleDict, seqMatch);
							if (info.GraphingType == GraphType.Means) {
								MainData.MakeRankLineGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							} else {
								MainData.MakeMultiHistoRankGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							}
							break;
						case OutputMethGraph.Variant:
							singleDict = MainData.Genome.RSPOUTQuantVar (info.SeqFeature2);
							procCont = ProcessRSplitContainer<double> (rspcDouble, info, rankDict, singleDict, seqMatch);
							if (info.GraphingType == GraphType.Means) {
								MainData.MakeRankLineGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							} else {
								MainData.MakeMultiHistoRankGraph (procCont, GraphWindow.Global, xLab, yLab, title, rspcDouble, info.MergeGraphs);
							}
							break;
						default:
							break;
						}
					}
				}
			}
			catch(Exception e)
			{
				MainData.ShowMessageWindow (e.Message, true);
			}
		}

		private static void SetLabels(OutputMethGraph om, GraphType ty, out string xl, out string yl)
		{
			if (ty == GraphType.Histo) {
				xl = xHistoLabDesc [om];
				yl = yHistoLabDesc [om];
			} else if (ty == GraphType.Spatial) {
				xl = xSpatialLabDesc [om];
				yl = ySpatialLabDesc [om];
			} else {
				xl = xMeansLabDesc [om];
				yl = yMeansLabDesc [om];
			}
		}

		private static Dictionary<string, List<T>> ProcessRSplitContainer<T>(RSplitContainer<T> rspc, RankSplitInfo rsi, 
			Dictionary<string, double> rDict, Dictionary<string, T> outDict, bool seqMatch)
		{
			if (rsi.doSubSet && rsi.SubSetType == SubSetMethod.Previous) {
				rspc.AddIDFilterList (rsi.oldIDlist);
			}

			rspc.MergeDictosToList (rDict, outDict, false, seqMatch);

			if (rsi.doSubSet && rsi.SubSetType == SubSetMethod.Current) {
				return rspc.GetDivisionDict (rsi.SplitCount, rsi.subCurMainRank, rsi.subCurSplitCount);
			} else
				return rspc.GetDivisionDict (rsi.SplitCount);
		}

		public static bool PickFileName(out string fname)
		{
			bool result = false;

			FileChooserDialog chooser = new FileChooserDialog (
				"Where do you want to save this data?",
				MainData.MainWindow,
				FileChooserAction.Save,
				"Cancel", ResponseType.Cancel,
				"Save", ResponseType.Accept);

			if (chooser.Run () == (int)ResponseType.Accept) {
				fname = chooser.Filename;
				result = true;
			} else {
				fname = "";
				result = false;
			}

			chooser.Destroy ();

			return result;
		}
	}
}

