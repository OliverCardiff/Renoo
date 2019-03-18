using System;
using Gtk;
using OxyPlot.GtkSharp;
using OxyPlot;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class NumericalText
	{
		public List<string> Titles;
		public List<List<double>> Data;
		public List<List<string>> Tags;

		public NumericalText()
		{
			Titles = new List<string> ();
			Data = new List<List<double>> ();
			Tags = new List<List<string>> ();
		}

		public void Merge(NumericalText txt)
		{
			Titles.AddRange (txt.Titles);
			Data.AddRange (txt.Data);
			Tags.AddRange (txt.Tags);
		}
	}
	public class GraphWindowPair
	{
		public GraphWindowPair(ScrolledWindow win, PlotView p)
		{
			Window = win;
			Plot = p;
			win.Add (p);
		}

		protected ScrolledWindow Window;
		protected PlotView Plot;
		protected ProcessingClass proc;

		public NumericalText GetData()
		{
			return proc.GetData ();
		}

		public void AssignAndShow(PlotModel m, ProcessingClass procCl)
		{
			proc = procCl;
			Plot.Model = m;
			Plot.InvalidatePlot (true);
			Plot.Show ();
		}

		public void DetachAndHide()
		{
			Plot.Model = null;
			Plot.InvalidatePlot (true);
			Plot.Hide ();
		}

		public PlotModel GetModel()
		{
			return Plot.Model;
		}
	}
}

