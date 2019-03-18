using System;
using OxyPlot.Series;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class MultiGraphingClass : ProcessingClass
	{
		protected List<GraphingClass> histos;

		public MultiGraphingClass ()
		{
		}

		public override void AddToModel (OxyPlot.PlotModel model)
		{
			foreach (GraphingClass dh in histos) {
				if (dh.GetHistoCoords ().Count > 1) {
					dh.AddToModel (model);
				}
			}
		}

		public List<Series> GetNextSeries (string addIt)
		{
			List<Series> lrs = new List<Series> ();

			foreach (GraphingClass dh in histos) {
				if (dh.GetHistoCoords ().Count > 1) {
					lrs.AddRange (dh.GetNextSeries (addIt));
				}
			}

			return lrs;
		}

		public override double GetXPercentLimit(double perc)
		{
			double limit = 0;

			foreach (GraphingClass dhis in histos) {

				limit = Math.Max (dhis.GetXPercentLimit (perc), limit);
			}
			return limit;
		}

		public override double GetYPercentLimit(double perc)
		{
			double limit = 0;

			foreach (GraphingClass dhis in histos) {

				limit = Math.Max (dhis.GetYPercentLimit (perc), limit);
			}
			return limit;
		}

		public override NumericalText GetData ()
		{
			NumericalText nextDat = new NumericalText ();

			foreach (DoublesHisto dh in histos) {
				nextDat.Merge (dh.GetData ());
			}
			return nextDat;
		}
	}
}

