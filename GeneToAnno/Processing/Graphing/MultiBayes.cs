using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneToAnno
{
	public enum MultiBayesLayout{ Stacked, Sequential };

	public class MultiBayes : MultiGraphingClass
	{
		MultiBayesLayout layout;
		protected int DivSize;

		public static void FlipFullDictionary(Dictionary<string, List<List<double>>> toFlip)
		{
			foreach (string st in toFlip.Keys.ToList ()) {
				toFlip [st] = SpatialBayes.FlipListofLists (toFlip [st]);
			}
		}

		public MultiBayes (Dictionary<string, List<List<double>>> dict, ModelDisplayType mTy, MultiBayesLayout lyo)
		{
			layout = lyo;

			histos = new List<GraphingClass> ();


			if (layout == MultiBayesLayout.Sequential) {
				double bgFac = GetMegaBg (dict);
				int cnt = 0;
				foreach (KeyValuePair<string, List<List<double>>> kvp in dict) {
					DivSize = kvp.Value.Count;
					int tot = kvp.Value.Count;
					histos.Add (new SpatialBayes (kvp.Value, bgFac, tot * cnt, kvp.Key, mTy));
					cnt += 1;
				}
			} else {
				if (dict.Count == 1) {
					foreach (KeyValuePair<string, List<List<double>>> kvp in dict) {
						histos.Add (new SpatialBayes (kvp.Value, kvp.Key, mTy));
					}
				} else {
					double bgFac = GetMegaBg (dict);

					foreach (KeyValuePair<string, List<List<double>>> kvp in dict) {
						histos.Add (new SpatialBayes (kvp.Value, bgFac, kvp.Key, mTy));
					}
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
				nt.Titles.Insert (0, "Division");

				return nt;
			} else {
				return base.GetData ();
			}
		}
	}
}

