using System;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class MultiHisto : MultiGraphingClass
	{
		public MultiHisto (Dictionary<string, List<double>> dict, int buckets, ModelDisplayType dtype) : base()
		{
			histos = new List<GraphingClass> ();

			double fullMax = 0;
			double fullMin = 999999999;
			double stdMin = 999999999;
			double stdMax = 0;

			foreach (KeyValuePair<string, List<double>> kvp in dict) {
				if (kvp.Value.Count > 5) {
					
					DoublesHisto h = new DoublesHisto (kvp.Value, buckets, kvp.Key, true, dtype);
					histos.Add (h);

					fullMax = Math.Max (h.GetLocalMax(), fullMax);
					fullMin = Math.Min (h.GetLocalMin(), fullMin);
					stdMax = Math.Max (h.GetPlusTwoStdDevs(), stdMax);
					stdMin = Math.Min (h.GetMinusTwoStdDevs(), stdMin);
				}
			}

			stdMin = Math.Max (0, stdMin);
			List<double> increments = DoublesHisto.CreateBoundaryIncrements (fullMin, fullMax, stdMin, stdMax, buckets);

			int pos = 1;
			int cnt = histos.Count;

			foreach (DoublesHisto h in histos) {
				h.SetBoundaryIncrements (increments);
				h.CountToBins (fullMin);
				h.SetTotalHistos (cnt, pos);
				pos++;
			}
		}
	}
}

