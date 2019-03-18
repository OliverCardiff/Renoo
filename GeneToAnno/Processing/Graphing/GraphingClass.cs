using System;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class GraphingClass : ProcessingClass
	{
		protected ModelDisplayType dType;
		protected string lineName;
		protected int totalHistos;
		protected int totalHistPosition;

		public void SetTotalHistos(int tot, int pos)
		{
			totalHistPosition = pos;
			totalHistos = tot;
		}

		public GraphingClass (ModelDisplayType ty, string name)
		{
			dType = ty;
			lineName = name;
		}

		public virtual List<DataPoint> GetHistoCoords()
		{
			List<DataPoint> ld = new List<DataPoint> ();
			return ld;
		}
		public virtual List<RectangleBarItem> GetHistoBars()
		{
			List<RectangleBarItem> rtl = new List<RectangleBarItem> ();
			return rtl;
		}

		protected void GetMultiHistobarPos (double min, double max, out double startPos, out double endPos)
		{
			double widthMul = 1 / (double)totalHistos;
			double width = (max - min) * widthMul;
			if (totalHistos > 1) {
				width *= 0.85;
			}
			startPos = min + (width * (totalHistPosition - 1));
			endPos = startPos + width;
		}

		public override void AddToModel (PlotModel model)
		{
			if (dType == ModelDisplayType.Line) {
				LineSeries lser = new LineSeries (this.lineName);

				List<DataPoint> lxy = GetHistoCoords ();

				foreach (DataPoint xy in lxy) {
					lser.Points.Add (xy);
				}

				lser.Smooth = true;
				model.Series.Add (lser);
			} else if (dType == ModelDisplayType.Bar) {
				RectangleBarSeries inSeries = new RectangleBarSeries ();
				inSeries.Title = this.lineName;

				List<RectangleBarItem> lrect = GetHistoBars ();

				foreach(RectangleBarItem r in lrect)
				{
					inSeries.Items.Add (r);
				}

				model.Series.Add (inSeries);
			}
		}

		public List<Series> GetNextSeries(string addIt)
		{
			List<Series> srl = new List<Series> ();

			if (dType == ModelDisplayType.Line) {
				LineSeries lser = new LineSeries (addIt + this.lineName);

				List<DataPoint> lxy = GetHistoCoords ();

				foreach (DataPoint xy in lxy) {
					lser.Points.Add (xy);
				}

				lser.Smooth = true;
				srl.Add (lser);
			} else if (dType == ModelDisplayType.Bar) {
				RectangleBarSeries inSeries = new RectangleBarSeries ();
				inSeries.Title = addIt + this.lineName;

				List<RectangleBarItem> lrect = GetHistoBars ();

				foreach(RectangleBarItem r in lrect)
				{
					inSeries.Items.Add (r);
				}

				srl.Add (inSeries);
			}

			return srl;
		}
	}
}

