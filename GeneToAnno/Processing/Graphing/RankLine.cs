using System;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Distributions;
using System.Linq;

namespace GeneToAnno
{
	public class RankLine : GraphingClass
	{
		protected Dictionary<string, List<double>> Data;
		protected List<DataPoint> points;

		public RankLine (Dictionary<string, List<double>> ranks, string name, ModelDisplayType dtype)
			:base(dtype, name)
		{
			Data = ranks;
			points = CalculateHistoCoords ();
		}

		public override List<DataPoint> GetHistoCoords ()
		{
			return points;
		}

		protected double NormMean (List<double> thl)
		{
			return Normal.Estimate (thl).Mean;
		}

		protected List<DataPoint> CalculateHistoCoords()
		{
			List<DataPoint> dps = new List<DataPoint> ();
			double incr = 1;
			foreach (KeyValuePair<string, List<double>> kvp in Data) {
				dps.Add (new DataPoint (incr, NormMean (kvp.Value)));

				incr++;
			}
			return dps;
		}

		public List<ErrorColumnItem> GetHistoErrorCoords ()
		{
			List<ErrorColumnItem> dps = new List<ErrorColumnItem> ();
			int incr = 1;
			foreach (KeyValuePair<string, List<double>> kvp in Data) {
				dps.Add (new ErrorColumnItem (NormMean (kvp.Value), Statistics.StandardDeviation(kvp.Value), incr));

				incr++;
			}
			return dps;
		}

		public override void AddToModel (PlotModel model)
		{
			/*ErrorColumnSeries errser = new ErrorColumnSeries ();

			List<ErrorColumnItem> errs = GetHistoErrorCoords ();

			foreach (ErrorColumnItem ei in errs) {
				errser.Items.Add (ei);
			}

			model.Series.Add (errser);*/
			base.AddToModel (model);
		}

		public override NumericalText GetData ()
		{
			NumericalText nt = new NumericalText ();

			if (AppSettings.Output.OUTPUT_PROCESSED.Item) {
				List<DataPoint> dps = GetHistoCoords ();
				List<string> names = Data.Keys.ToList ();
				List<double> xs = new List<double> ();
				List<double> ys = new List<double> ();

				foreach (DataPoint dp in dps) {
					xs.Add (dp.X);
					ys.Add (dp.Y);
				}

				nt.Titles.Add ("Groups");
				nt.Titles.Add (lineName + "_X");
				nt.Titles.Add (lineName + "_Y");
				nt.Tags.Add (names);
				nt.Data.Add (xs);
				nt.Data.Add (ys);
			} else {
				foreach (KeyValuePair<string, List<double>> kvp in Data) {
					nt.Titles.Add (kvp.Key);
					nt.Data.Add (kvp.Value);
				}
			}

			return nt;
		}

		public override double GetXPercentLimit (double perc)
		{
			return (double)Data.Count * perc;;
		}

		public override double GetYPercentLimit (double perc)
		{
			double highestY = 0;
			foreach (DataPoint dp in points) {
				highestY = Math.Max (highestY, dp.Y);
			}
			return highestY * perc;
		}
			
	}
}

