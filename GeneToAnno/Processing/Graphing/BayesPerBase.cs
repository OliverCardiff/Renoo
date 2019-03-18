using System;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class BayesPerBase : GraphingClass
	{
		List<List<double>> data;
		List<double> processed;
		bool FromStart;
		double BGFactor;
		public double MaxLen;

		public BayesPerBase (List<List<double>> bases, string name, ModelDisplayType dtype, bool fromStart)
			:base(dtype, name)
		{
			FromStart = fromStart;
			data = bases;	
			BGFactor = makeBgFactor (data);
			ProcessData ();
		}

		public BayesPerBase (List<List<double>> bases, double bgFactor, string name, ModelDisplayType dtype, bool fromStart)
			:base(dtype, name)
		{
			FromStart = fromStart;
			data = bases;
			BGFactor = bgFactor;
			ProcessData ();
		}

		public static double makeBgFactor(List<List<double>> data)
		{
			double cumu = 0;
			double counts = 0;

			foreach (List<double> ldb in data) {
				foreach (double d in ldb) {
					cumu += d;
					counts++;
				}
			}

			return (cumu / counts);
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

		public override NumericalText GetData ()
		{
			NumericalText nt = new NumericalText ();

			if (AppSettings.Output.OUTPUT_PROCESSED.Item) {
				nt.Titles.Add (lineName);
				nt.Data.Add (processed);
			} else {
				int inc = 1;
				foreach (List<double> ldbl in data) {
					nt.Data.Add (ldbl);
					nt.Titles.Add (lineName + "_base" + inc);
					inc++;
				}
			}

			return nt;
		}

		public override double GetXPercentLimit (double perc)
		{
			double limit = (double)(processed.Count) * perc;

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

		protected void ProcessData()
		{
			List<List<double>> allFixed = new List<List<double>> ();

			int longest = 0;

			foreach (List<double> ld in data) {
				longest = Math.Max (longest, ld.Count);
			}

			for (int i = 0; i < data.Count; i++) {
				List<double> nextfix = new List<double> ();

				int thisLen = data [i].Count;
				int diff = longest - thisLen;

				if (FromStart) {
					for (int j = 0; j < thisLen; j++) {
						nextfix.Add (data [i] [j]);
					}
					for (int j = 0; j < diff; j++) {
						nextfix.Add (-1);
					}
				} else {
					for (int j = 0; j < diff; j++) {
						nextfix.Add (-1);
					}
					for (int j = 0; j < thisLen; j++) {
						nextfix.Add (data [i] [j]);
					}
				}

				allFixed.Add (nextfix);
			}

			processed = new List<double> ();
			List<double> counts = new List<double> ();
			List<double> cumus = new List<double> ();
			MaxLen = longest;

			for (int i = 0; i < longest; i++) {
				counts.Add (0);
				cumus.Add (0);
			}

			foreach (List<double> ldbl in allFixed) {

				for (int i = 0; i < longest; i++) {
					if (ldbl [i] != -1) {
						counts [i] += 1;
						cumus [i] += ldbl [i];
					}
				}
			}

			for (int i = 0; i < longest; i++) {
				processed.Add ((cumus [i] / counts [i])/BGFactor);
			}
		}

		public override List<RectangleBarItem> GetHistoBars ()
		{
			List<RectangleBarItem> dps = new List<RectangleBarItem> ();
			double accu = 0;
			foreach (double d in processed) {
				double startP;
				double endP;
				GetMultiHistobarPos (accu, accu + 1, out startP, out endP);
				dps.Add (new RectangleBarItem (startP, 0, endP, d));
				accu += 1;
			}
				
			return dps; 
		}

		public override List<OxyPlot.DataPoint> GetHistoCoords ()
		{
			List<DataPoint> dps = new List<DataPoint> ();
			double accu = 1;

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
	}
}

