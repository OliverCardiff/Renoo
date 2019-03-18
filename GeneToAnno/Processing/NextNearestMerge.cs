using System;
using MathNet.Numerics.Statistics;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class BinComparison : IComparable
	{
		public Bin smaller;
		public Bin larger;

		public double AvgDiff;

		public BinComparison(Bin small, Bin big)
		{
			smaller = small;
			larger = big;

			AvgDiff = larger.GetAvg () - smaller.GetAvg ();
		}

		public Bin MergeComp()
		{
			Bin b = new Bin (smaller, larger);
			return b;
		}
		public bool StillValid()
		{
			if (smaller.RecentlyMerged == false && larger.RecentlyMerged == false)
				return true;
			else
				return false;
		}

		public int CompareTo(object obj) {
			if (obj == null) return 1;

			BinComparison other = obj as BinComparison;
			if (other != null) {
				int result = AvgDiff.CompareTo (other.AvgDiff);

				return result;
			}
			else 
				throw new ArgumentException("Object is not an Element!");
		}
	}
	public class Bin : IComparable
	{
		public List<double> vals;
		public bool RecentlyMerged;
		public double Avg;
		public double Mid;
		public double Min;
		public double Max;
		public double FullRange;

		public Bin(double v, double min, double max)
		{
			Min = min;
			Max = max;
			Mid = ((max - min) / 2) + min;
			RecentlyMerged = false;
			vals = new List<double> ();
			vals.Add (v);
		}
		public Bin(double min, double max)
		{
			Min = min;
			Max = max;
			Mid = ((max - min) / 2) + min;
			RecentlyMerged = false;
			vals = new List<double> ();
		}
		public Bin(List<double> dubs)
		{
			RecentlyMerged = false;
			vals = dubs;
		}
		public Bin(List<Bin> bins)
		{
			RecentlyMerged = false;
			vals = new List<double> ();
			foreach (Bin b in bins) {
				vals.AddRange (b.vals);
			}
		}
		public Bin(Bin a, Bin b)
		{
			RecentlyMerged = false;
			a.RecentlyMerged = true;
			b.RecentlyMerged = true;

			vals = new List<double> ();
			vals.AddRange (a.vals);
			vals.AddRange (b.vals);
		}
		public double GetDensity(double total)
		{
			return ((double)vals.Count / total) / ((Max - Min) / FullRange);
		}
		public void AddVal(double v)
		{
			vals.Add(v);
		}
		public void SetAvg()
		{
			Avg = Statistics.Mean (vals);
		}
		public double GetAvg()
		{
			return Avg;	
		}

		public int CompareTo(object obj) {
			if (obj == null) return 1;

			Bin other = obj as Bin;
			if (other != null) {
				int result = GetAvg().CompareTo(other.GetAvg());

				return result;
			}
			else 
				throw new ArgumentException("Object is not a Bin!");
		}
	}
	/*public class NextNearestMerge
	{
		private List<Bin> bins;
		//private List<double> data;
		//private double totalCount;
		//private int bcount;
		//private int min;
		//private int max;

		public NextNearestMerge ()
		{
		}

		private List<BinComparison> CompareBins()
		{
			List<BinComparison> bcomp = new List<BinComparison> ();
			int cnt = bins.Count;
			for (int i = 1; i < cnt; i++) {
				bcomp.Add (new BinComparison (bins [i - 1], bins [i]));
			}
			bcomp.Sort ();
			return bcomp;
		}

		private void MergeDownBins(List<BinComparison> bcomps, ref int mergesRemain)
		{
			List<Bin> nextList = new List<Bin> ();
			int cnt = bcomps.Count;

			for (int i = 0; i < cnt; i++) {
				if (mergesRemain > 0) {
					if (bcomps [i].StillValid ()) {
						Bin b = bcomps [i].MergeComp ();
						b.SetAvg ();
						nextList.Add (b);
						mergesRemain--;
					}
				} else
					break;
			}

			foreach (Bin b in bins) {
				if (!b.RecentlyMerged) {
					nextList.Add (b);
				}
			}
			bins = nextList;
			bins.Sort ();
		}

		private void CountToBins()
		{
			bins = new List<Bin> ();
			Bin cb = null;
			int initialbin = (int)Math.Max(totalCount/((double)bcount * 10), 2);
			int currentCnt = 0;
			int cnt = data.Count;

			for (int i = 0; i < cnt; i++) {
				if (currentCnt == 0) {
					cb = new Bin (data [i], 0, 1);
					bins.Add (cb);
				} else {
					cb.AddVal (data [i]);
				}

				currentCnt++;
				if (currentCnt >= initialbin) {
					currentCnt = 0;
					cb.SetAvg ();
				}
			}

			int mergesToAchieve = bins.Count - bcount;

			while (mergesToAchieve > 0) {
				MergeDownBins (CompareBins (), ref mergesToAchieve);
			}

			//min = bins [0].GetAvg ();
			//max = bins [bins.Count - 1].GetAvg ();
		}
	}
	*/
}

