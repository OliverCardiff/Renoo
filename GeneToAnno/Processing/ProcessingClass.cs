using System;
using OxyPlot;
using System.Collections.Generic;
using System.IO;

namespace GeneToAnno
{
	public enum ModelDisplayType {Line, Bar};
	public enum PrintOutNext {Tag, Data};

	public class ProcessingClass
	{
		public ProcessingClass ()
		{
		}

		public virtual NumericalText GetData()
		{
			return new NumericalText ();
		}

		public virtual void AddToModel(PlotModel model)
		{
			//do something
		}

		public static void WriteDataFile(string fileName, NumericalText nt)
		{
			if (((nt.Data != null) && (nt.Data.Count > 0)) || ((nt.Tags != null) && (nt.Tags.Count > 0))) {
				int useLen = Math.Min (nt.Titles.Count, (nt.Data.Count + nt.Tags.Count));
				int dataLen = nt.Data [0].Count;
				List<int> counts = new List<int> ();
				List<int> currentIndexes = new List<int> ();
				List<PrintOutNext> printTypes = new List<PrintOutNext> ();

				if (nt.Tags != null) {
					int tagInd = 0;
					foreach (List<string> sl in nt.Tags) {
						counts.Add (sl.Count - 1);
						printTypes.Add (PrintOutNext.Tag);
						dataLen = Math.Max (dataLen, sl.Count);
						currentIndexes.Add (tagInd);
						tagInd++;
					}
				}
				if (nt.Data != null) {
					int datInd = 0;
					foreach (List<Double> ld in nt.Data) {
						counts.Add (ld.Count - 1);
						printTypes.Add (PrintOutNext.Data);
						dataLen = Math.Max (dataLen, ld.Count);
						currentIndexes.Add (datInd);
						datInd++;
					}
				}

				using (StreamWriter sw = new StreamWriter (fileName)) {
					for (int i = 0; i < useLen; i++) {
						if (i > 0) {
							sw.Write ("\t");
						}
						sw.Write (nt.Titles [i]);
					}
					sw.Write ("\n");

					for(int j = 0; j < dataLen; j++)
					{
						for (int i = 0; i < useLen; i++) {
							if (i > 0) {
								sw.Write ("\t");
							}
							if (counts [i] >= j) {
								if (printTypes [i] == PrintOutNext.Data) {
									sw.Write (nt.Data [currentIndexes[i]] [j]);
								} else {
									sw.Write (nt.Tags [currentIndexes [i]] [j]);
								}
							} else
								sw.Write ("x");
						}
						sw.Write ("\n");
					}
				}
				MainData.UpdateLog ("Saved " + MainData.MainWindow.MaxLenString (fileName, 20), false);
			} else {
				MainData.ShowMessageWindow ("There isn't any data there to save!", false);
			}
		}

		public virtual double GetXPercentLimit(double perc)
		{
			double limit = 0;

			return limit;
		}

		public virtual double GetYPercentLimit(double perc)
		{
			double limit = 0;

			return limit;
		}
	}
}

