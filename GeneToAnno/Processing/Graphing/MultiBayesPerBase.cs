using System;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class MultiBayesPerBase : MultiGraphingClass
	{
		protected int DivSize;

		public MultiBayesPerBase (Dictionary<string, List<List<double>>> dict, bool fromStart) 
		{
			histos = new List<GraphingClass> ();
			DivSize = 0;

			if (dict.Count == 1) {
				foreach (KeyValuePair<string, List<List<double>>> kvp in dict) {
					BayesPerBase bpb = new BayesPerBase (kvp.Value, kvp.Key, ModelDisplayType.Line, fromStart);
					histos.Add (bpb);
					DivSize = (int)bpb.MaxLen;
				}
			} else {
				double bgFac = GetMegaBg (dict);

				foreach (KeyValuePair<string, List<List<double>>> kvp in dict) {
					BayesPerBase bpb = new BayesPerBase (kvp.Value, bgFac, kvp.Key, ModelDisplayType.Line, fromStart);
					histos.Add (bpb);
					DivSize = Math.Max (DivSize, (int)bpb.MaxLen);
				}
			}

		}

		public static double GetMegaBg(Dictionary<string, List<List<double>>> data)
		{
			double cumu = 0;
			double count = 0;

			foreach (KeyValuePair<string, List<List<double>>> kvp in data) {
				SpatialBayes.GetBGCumu(kvp.Value, ref cumu, ref count);
			}

			return cumu / count;
		}

		public override NumericalText GetData ()
		{
			if (AppSettings.Output.OUTPUT_PROCESSED.Item) {
				NumericalText nt = base.GetData ();
				List<double> rankpos = new List<double> ();

				for (int i = 1; i <= DivSize; i++) {
					rankpos.Add (i);
				}
				nt.Data.Insert (0, rankpos);
				nt.Titles.Insert (0, "Base Number");

				return nt;
			} else {
				return base.GetData ();
			}
		}
	}
}

