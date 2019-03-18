using System;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using MathNet.Numerics.Statistics;

namespace GeneToAnno
{
	public class DoublesHisto : GraphingClass
	{
		List<double> data;
		List<Bin> bins;
		List<double> boundIncrements;
		int bcount;
		double totalCount;
		double avg;
		double stdDev;
		//double stdSize;

		public DoublesHisto(List<double> l, int buckets, string name, bool isMulti, ModelDisplayType dtype)
			:base(dtype, name)
		{
			totalHistPosition = 1;
			totalHistos = 1;
			bcount = buckets;
			l.Sort ();
			data = new List<double> ();
			data.AddRange (l);
			totalCount = data.Count;
			avg = Statistics.Mean (data);
			stdDev = Statistics.StandardDeviation (data);
			if (!isMulti) {
				CountToBins ();
			}
		}
			
		public double GetMinusTwoStdDevs()
		{
			return avg - (stdDev * 2);
		}
		public double GetPlusTwoStdDevs()
		{
			return avg + (stdDev * 2);
		}

		public static List<double> CreateBoundaryIncrements(double min, double max, double lowStd, double highStd, int buckets)
		{
			List<double> divs = new List<double> ();
			int lowBucks = (int)Math.Max ((double)buckets * 0.02, 1);
			int highBucks = lowBucks;

			if (min >= lowStd) {
				lowBucks = 0;
			}
			if (max <= highStd) {
				highBucks = 0;
			}

			int midBucks = buckets - (highBucks + lowBucks);

			if (lowBucks > 0) {
				divs.AddRange (GetDivisions (min, lowStd, lowBucks));
			}

			divs.AddRange (GetDivisions (lowStd, highStd, midBucks));

			if (highBucks > 0) {
				divs.AddRange (GetDivisions (highStd, max, highBucks));
			}

			return divs;
		}

		public static List<double> GetDivisions(double lo, double hi, int split)
		{
			List<double> dubs = new List<double> ();
			double bound = (hi - lo) / (double)split;

			for (int i = 0; i < split; i++) {
				dubs.Add (lo + bound + (i * bound));
			}
			return dubs;
		}

		public void SetBoundaryIncrements(List<double> inc)
		{
			boundIncrements = inc;
		}

		public override NumericalText GetData()
		{
			NumericalText nt = new NumericalText ();

			if (AppSettings.Output.OUTPUT_PROCESSED.Item) {
					nt.Titles.Add (this.lineName + "_X");
					nt.Titles.Add (this.lineName + "_Y");
					List<DataPoint> dps = GetHistoCoords ();
					List<double> xs = new List<double> ();
					List<double> ys = new List<double> ();
					foreach (DataPoint dp in dps) {
						xs.Add (dp.X);
						ys.Add (dp.Y);
					}
					nt.Data.Add (xs);
					nt.Data.Add (ys);
			} else {
				nt.Titles.Add (this.lineName);
				nt.Data.Add (data);
			}
			return nt;
		}

		public override List<DataPoint> GetHistoCoords()
		{
			List<DataPoint> coords = new List<DataPoint> ();
			foreach (Bin b in bins) {
				coords.Add (new DataPoint (b.Mid, b.GetDensity (totalCount)));
			}

			return coords;
		}

		public override List<RectangleBarItem> GetHistoBars()
		{
			List<RectangleBarItem> lrect = new List<RectangleBarItem> ();

			foreach (Bin b in bins) {
				double startPos;
				double endPos;
				GetMultiHistobarPos (b.Min, b.Max, out startPos, out endPos);
				lrect.Add (new RectangleBarItem (startPos, 0, endPos, b.GetDensity (totalCount)));
			}
			return lrect;
		}

		public double GetLocalMax()
		{
			return data [data.Count - 1];
		}

		public double GetLocalMin()
		{
			return data [0];
		}

		private void CountToBins()
		{
			int index0 = 0; int indexL = data.Count - 1;

			double localMax = data[indexL];
			double localMin = data[index0];

			boundIncrements = CreateBoundaryIncrements (localMin, localMax, GetMinusTwoStdDevs (), GetPlusTwoStdDevs (), bcount);
			CountToBins (localMin);
		}

		public void CountToBins(double min)
		{
			bins = new List<Bin> ();

			double currentBoundary = boundIncrements[0];
			double oldBoundary = min;
			double frange = boundIncrements [boundIncrements.Count - 1] - min;
			Bin b = new Bin(oldBoundary, currentBoundary);
			b.FullRange = frange;
			bins.Add (b);
			int inc = 0;

			foreach (double d in data) {
				if (d > currentBoundary) {
					while (d > currentBoundary) {
						oldBoundary = boundIncrements[inc];
						inc++;
						currentBoundary = boundIncrements[inc];

						b = new Bin (oldBoundary, currentBoundary);
						b.FullRange = frange;
						bins.Add (b);
					}
					b.AddVal (d);
				} else {
					b.AddVal (d);
				}
			}

			foreach(Bin bb in bins)
			{
				bb.SetAvg();
			}
		}



		/// <summary>
		/// Inserts the binned data as a lineseries into the plotmodel
		/// </summary>
		/// <param name="model">Model.</param>


		/// <summary>
		/// Finds the X axis limit based on a certain proportion of the values
		/// </summary>
		/// <returns>The x axis limit.</returns>
		/// <param name="perc">The proportion, between 0 and 1</param>
		public override double GetXPercentLimit(double perc)
		{
			int index = (int)Math.Floor(((double)data.Count * perc));

			if (index == data.Count)
				index--;

			return data [index];
		}
		/// <summary>
		/// Finds the Y axis limit based on a certain proportion of the values
		/// </summary>
		/// <returns>The y axis limit.</returns>
		/// <param name="perc">The proportion, between 0 and 1</param>
		public override double GetYPercentLimit(double perc)
		{
			double dens = 0;

			foreach (Bin b in bins) {
				dens = Math.Max (dens, b.GetDensity ((double)totalCount));
			}

			return dens * perc;
		}
	}
}

