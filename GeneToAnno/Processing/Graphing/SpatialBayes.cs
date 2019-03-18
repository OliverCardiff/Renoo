using System;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class SpatialBayes : GraphingClass
	{
		public static List<List<double>> FlipListofLists(List<List<double>> toFlip)
		{
			List<List<double>> res = new List<List<double>> ();
			int setSize = toFlip [0].Count;

			for (int i = 0; i < setSize; i++) {
				res.Add (new List<double> ());
				foreach (List<double> ldbl in toFlip) {
					res [i].Add (ldbl [i]);
				}
			}
			return res;
		}

		List<List<double>> data;
		List<double> processed;
		double localBg;
		int startIndex;

		public SpatialBayes (List<List<double>> ls, string name, ModelDisplayType dTy) 
			: base(dTy, name)
		{
			startIndex = 1;
			data = ls;
			localBg = makeBgFactor (data);
			ProcessData ();
		}

		public SpatialBayes (List<List<double>> ls,  double bgFactor, string name, ModelDisplayType dTy)
			:base(dTy, name)
		{
			startIndex = 1;
			localBg = bgFactor;
			data = ls;
			ProcessData ();
		}

		public SpatialBayes (List<List<double>> ls,  double bgFactor, int stIndex, string name, ModelDisplayType dTy)
			:base(dTy, name)
		{
			startIndex = stIndex;
			localBg = bgFactor;
			data = ls;
			ProcessData ();
		}

		protected void ProcessData()
		{
			double counts = data [0].Count;
			processed = new List<double> ();

			foreach (List<double> ldbl in data) {
				double next = 0;
				foreach (double d in ldbl) {
					next += d;
				}
				processed.Add ((next/counts)/localBg);
			}
		}

		public static double makeBgFactor(List<List<double>> data)
		{
			double cumu = 0;
			double counts = data [0].Count;
			double unitCount = data.Count;

			foreach (List<double> ldb in data) {
				foreach (double d in ldb) {
					cumu += d;
				}
			}

			return (cumu / counts) / unitCount;
		}

		public static void GetBGCumu(List<List<double>> data, ref double cumu, ref double count)
		{
			foreach (List<double> ldb in data) {
				foreach (double d in ldb) {
					cumu += d;
					count++;
				}
			}
		}

		public override List<OxyPlot.Series.RectangleBarItem> GetHistoBars ()
		{
			double accu = startIndex;
			List<RectangleBarItem> dps = new List<RectangleBarItem> ();

			foreach (double d in processed) {
				if (startIndex != 0) {
					dps.Add (new RectangleBarItem (accu, 0, accu + 1, d));
				} else {
					double startP;
					double endP;
					GetMultiHistobarPos (accu, accu + 1, out startP, out endP);
					dps.Add (new RectangleBarItem (startP, 0, endP, d));
				}
				accu += 1;
			}

			return dps; 
		}

		public override List<OxyPlot.DataPoint> GetHistoCoords ()
		{
			List<DataPoint> dps = new List<DataPoint> ();
			double accu = startIndex;

			foreach (double d in processed) {
				dps.Add (new DataPoint (accu, d));
				accu += 1;
			}
			return dps;
		}

		public override void AddToModel (OxyPlot.PlotModel model)
		{
			base.AddToModel (model);
		}

		public override NumericalText GetData ()
		{
			NumericalText nt = new NumericalText ();

			if (!AppSettings.Output.OUTPUT_PROCESSED.Item) {
				nt.Titles.Add (lineName);
				nt.Data.Add (processed);
			} else {
				int inc = 1;
				foreach (List<double> ldbl in data) {
					nt.Titles.Add(lineName + "_div" + inc);
					nt.Data.Add (ldbl);
					inc++;
				}
			}

			return nt;
		}

		public override double GetXPercentLimit (double perc)
		{
			double limit = (startIndex + processed.Count) * perc;

			return limit;
		}

		public override double GetYPercentLimit (double perc)
		{
			double max = 0;

			foreach (double d in processed) {
				max = Math.Max (max, d);
			}

			return max * perc;
		}
	}
}

