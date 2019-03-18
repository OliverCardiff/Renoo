using System;
using System.Collections.Generic;

namespace GeneToAnno
{
	public partial class Genome
	{
		public static string SingleSeqForRSP;

		//RSP RANK METHODS

		public Dictionary<string, double> RSPMethylCoverage(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPMethlyCovScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPMethlyCovScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPMethlyCovScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPElementMethlyCoverage (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPElementMethlyCoverage (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPMethylDepthCoverage(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPMethylDepthCovScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPMethylDepthCovScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPMethylDepthCovScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPElementMethlyDepthCoverage (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPElementMethlyDepthCoverage (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPExprValue(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPExprValueScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPExprValueScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPExprValueScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPExpr (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPExpr (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPExprNorm(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPExprNormScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPExprNormScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPExprNormScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPExprNorm (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPExprNorm (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPExprRange(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPExprRangeScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPExprRangeScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPExprRangeScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPExprRange (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPExprRange (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPBlastEValue(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPBlastEValueScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPBlastEValueScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPBlastEValueScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPBlastEVal (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPBlastEVal (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPBlastPercID(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPBlastPercIDScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPBlastPercIDScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPBlastPercIDScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPBlastPercID (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPBlastPercID (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPVarCount(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPVarCountScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPVarCountScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPVarCountScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPVarCount (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPVarCount (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPVarAllHet(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPVarAllHetScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPVarAllHetScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPVarAllHetScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPVarAllHet (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPVarAllHet (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPVarAllHom(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPVarAllHomScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPVarAllHomScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPVarAllHomScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPVarAllHom (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPVarAllHom (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		//RSP OUTPUT METHODS

		//text only

		public Dictionary<string, string> RSPOUTBlastHitID(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTBlastHitIDScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, string>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, string>>
				(RSPOUTBlastHitIDScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<string> (thList);
			}
		}

		protected Dictionary<string, string> RSPOUTBlastHitIDScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, string> tdict = new Dictionary<string, string> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPOUTBlastHitID (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPOUTBlastHitID (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, string> RSPOUTBaseSeq(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTBaseSeqScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, string>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, string>>
				(RSPOUTBaseSeqScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<string> (thList);
			}
		}

		protected Dictionary<string, string> RSPOUTBaseSeqScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, string> tdict = new Dictionary<string, string> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPOUTBaseSeq (scf.ID, SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPOUTBaseSeq (scf.ID, SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, string> RSPOUTReadName(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTReadNameScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, string>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, string>>
				(RSPOUTReadNameScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<string> (thList);
			}
		}

		protected Dictionary<string, string> RSPOUTReadNameScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, string> tdict = new Dictionary<string, string> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPOUTReadNames (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPOUTReadNames (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		//rankline only

		public Dictionary<string, double> RSPOUTQuantMethyCov(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTQuantMethyCovScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPOUTQuantMethyCovScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPOUTQuantMethyCovScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPOUTQuantMethyCov (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPOUTQuantMethyCov (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPOUTQuantMethyDepthCov(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTQuantMethyDepthCovScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPOUTQuantMethyDepthCovScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPOUTQuantMethyDepthCovScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPOUTQuantMethyDepthCov (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPOUTQuantMethyDepthCov (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPOUTQuantVar(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTQuantVarScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPOUTQuantVarScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPOUTQuantVarScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPOUTQuantVar (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPOUTQuantVar (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		//all the rest

		public Dictionary<string, double> RSPOUTExprValue(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTExprValueScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPOUTExprValueScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPOUTExprValueScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPExpr (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPExpr (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPOUTExprNorm(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTExprNormScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPOUTExprNormScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPOUTExprNormScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPExprNorm (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPExprNorm (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPOUTExprRange(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTExprRangeScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPOUTExprRangeScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPOUTExprRangeScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPExprRange (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPExprRange (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, List<double>> RSPOUTMethylCoverageDivision(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTMethylCovDivScafffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, List<double>>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, List<double>>>
				(RSPOUTMethylCovDivScafffList, ShuffledScaffolds);

				return MainData.TypeDict.MergeDownDictsSingular<List<double>>(thList);
			}
		}

		protected Dictionary<string, List<double>> RSPOUTMethylCovDivScafffList(List<Scaffold> scfs)
		{
			Dictionary<string, List<double>> tdict = new Dictionary<string, List<double>>();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPOUTMethlyCoverage (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPOUTMethlyCoverage (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, List<double>> RSPOUTMethylCoverageDepthDivision(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTMethylCovDepthDivScafffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, List<double>>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, List<double>>>
				(RSPOUTMethylCovDepthDivScafffList, ShuffledScaffolds);

				return MainData.TypeDict.MergeDownDictsSingular<List<double>>(thList);
			}
		}

		protected Dictionary<string, List<double>> RSPOUTMethylCovDepthDivScafffList(List<Scaffold> scfs)
		{
			Dictionary<string, List<double>> tdict = new Dictionary<string, List<double>>();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPOUTMethlyDepthCoverage (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPOUTMethlyDepthCoverage (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}

		public Dictionary<string, double> RSPOUTMethylDepthTotal(string item)
		{
			SingleSeqForRSP = item;

			if (!AppSettings.Processing.USE_THREADS.Item) {
				return RSPOUTMethylDepthTotalScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, double>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, double>>
				(RSPOUTMethylDepthTotalScaffList, ShuffledScaffolds);
				return MainData.TypeDict.MergeDownDictsSingular<double> (thList);
			}
		}

		protected Dictionary<string, double> RSPOUTMethylDepthTotalScaffList(List<Scaffold> scfs)
		{
			Dictionary<string, double> tdict = new Dictionary<string, double> ();

			foreach (Scaffold scf in scfs) {
				foreach (KeyValuePair<string, Gene> gcf in scf.Genes) {
					gcf.Value.RSPOUTElementMethlyDepthCoverage (SingleSeqForRSP, tdict);
				}
				foreach (KeyValuePair<string, Region> rcf in scf.Regions) {
					rcf.Value.RSPOUTElementMethlyDepthCoverage (SingleSeqForRSP, tdict);
				}
			}

			return tdict;
		}
	}
}

