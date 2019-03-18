using System;
using Gtk;
using System.Threading;
using Meta.Numerics.Statistics;
using System.Collections.Generic;

namespace GeneToAnno
{
	public partial class StatsOutputWindow : Gtk.Window
	{
		RSplitContainer DataToProcess;
		List<List<Label>> SampleComparisons;

		public StatsOutputWindow (RSplitContainer rsp) :
			base (Gtk.WindowType.Toplevel)
		{
			DataToProcess = rsp;
			this.Build ();
			BuildComparisonMatrix ();
			PopulateCombos ();
		}

		protected void PopulateCombos()
		{
			foreach (KeyValuePair<string, PairedTest> kvp in StatsTester.PairedTestEnum) {
				covTestCombo.AppendText (kvp.Key);
			}
			foreach (KeyValuePair<string, BetweenGroupTest> kvp in StatsTester.BetweenTestEnum) {
				groupTestCombo.AppendText (kvp.Key);
			}
		}

		protected void BuildComparisonMatrix()
		{
			Dictionary<int, List<RSplitUnit<double>>> units = DataToProcess.GetGroupedList();
			SampleComparisons = new List<List<Label>> ();
			uint width = (uint)units.Count;
			groupCountLabel.Text = units.Count.ToString ();
			int pairCount = 0;

			Table mainTable = new Table (width + 2, width + 1, true);

			mainTable.Attach (new Label ("Groups"), 0, 1, 0, 1);

			uint rowInc = 1;
			SampleComparisons = new List<List<Label>> ();

			foreach (KeyValuePair<int, List<RSplitUnit<double>>> kvp in units) {
				List<Label> gn = new List<Label> ();

				mainTable.Attach (new Label ("G - " + kvp.Key.ToString ()), rowInc, rowInc + 1, 0, 1);
				mainTable.Attach (new Label ("G - " + kvp.Key.ToString ()), 0, 1, rowInc, rowInc + 1);

				for (uint i = 1; i <= width; i++) {
					Label next = new Label ("-");
					gn.Add (next);
					mainTable.Attach (next, i, i + 1, rowInc, rowInc + 1);
				}

				pairCount += kvp.Value.Count;
				rowInc++;
				SampleComparisons.Add (gn);
			}

			rankPairsLabel.Text = pairCount.ToString ();

			vbox10.PackStart (mainTable, true, true, 2);

			vbox10.ShowAll ();
		}

		protected void OnRunCovTest (object sender, EventArgs e)
		{
			PairedTest pt;

			try
			{
				pt = StatsTester.PairedTestEnum [covTestCombo.ActiveText];

				Thread t = new Thread (delegate () {TestDoWorkPaired(pt);});
				t.Start ();
			}
			catch(Exception ex) {
				MainData.ShowMessageWindow ("You need to Select A Correlation Test!\n" + ex.Message, false);
			}
		}

		protected void OnRunGroupTest (object sender, EventArgs e)
		{
			BetweenGroupTest pt;

			try
			{
				pt = StatsTester.BetweenTestEnum [groupTestCombo.ActiveText];
				Thread t = new Thread (delegate () {TestDoWorkBetween(pt);});
				t.Start ();
			}
			catch(Exception ex) {
				MainData.ShowMessageWindow ("You need to Select A Comparison Test!\n" + ex.Message, false);
			}
		}

		protected void OnClose (object sender, EventArgs e)
		{
			this.Destroy ();
		}

		protected void TestDoWorkBetween(BetweenGroupTest bt)
		{
			Dictionary<int, List<RSplitUnit<double>>> units = DataToProcess.GetGroupedList ();
			int cnt = units.Count;
			List<List<double>> rankData = new List<List<double>> ();

			foreach (KeyValuePair<int, List<RSplitUnit<double>>> kvp in units) {
				List<double> x = new List<double> ();
				List<double> y = new List<double> ();

				ExtractXYLists (kvp.Value, x, y);
				rankData.Add (y);
			}

			List<List<TestResult>> results = new List<List<TestResult>> ();

			for (int i = 0; i < cnt - 1; i++) {
				List<TestResult> tsr = new List<TestResult> ();
				for (int j = i + 1; j < cnt; j++) {
					tsr.Add (StatsTester.RunBetweenTest (rankData [i], rankData [j], bt));
				}
				results.Add (tsr);
			}

			Gtk.Application.Invoke (delegate {
				AssignResultsToTable(results, cnt, bt);
			});
		}

		protected void AssignResultsToTable(List<List<TestResult>> results, int groupCount, BetweenGroupTest bt)
		{
			for (int i = 0; i < groupCount - 1; i++) {
				for (int j = 0; j < groupCount -1 - i; j++) {
					if (showStatRadio.Active) {
						SampleComparisons [i] [j + 1 + i].Text = results [i] [j].Statistic.ToString ("G7");
					} else {
						if (results [i] [j].Statistic > 0) {
							SampleComparisons [i] [j + 1 + i].Text = results [i] [j].RightProbability.ToString ("e2");
						} else {
							SampleComparisons [i] [j + 1 + i].Text = results [i] [j].LeftProbability.ToString ("e2");
						}
					}
				}
			}
		}

		protected void TestDoWorkPaired(PairedTest pt)
		{
			List<double> x = new List<double> ();
			List<double> y = new List<double> ();
			ExtractXYLists(DataToProcess.GetRankedList (), x, y);

			TestResult ts = StatsTester.RunPairedTest (x, y, pt);

			Gtk.Application.Invoke (delegate {
				covStatLabel.Text = ts.Statistic.ToString("G7");
				covLProbLabel.Text = ts.LeftProbability.ToString("e2");
				covRProbLabel.Text = ts.RightProbability.ToString("e2");

				if(ts.Statistic > 0)
				{
					covRProbLabel.Text += " *";
				} else {
					covLProbLabel.Text += " *";
				}
			});
				
		}

		protected void ExtractXYLists (List<RSplitUnit<double>> lrsp, List<double> x, List<double> y)
		{
			foreach(RSplitUnit<double> rsu in lrsp)
			{
				x.Add (rsu.theValue);
				y.Add (rsu.rankData);
			}
		}
	}
}

