using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MathNet.Numerics.Statistics;

namespace GeneToAnno
{
	public enum ColourScale {GreenRed, GreenBlue};
	public enum ColourRank {Sequential, Grouped};
	class RegionChangeMap
	{
		List<List<double>> Data;
		List<List<double>> VarData;
		List<double> rankVal;
		List<Color> sideColours;
		Dictionary<string, int> sideBoxes;
		Bitmap varMap;
		Bitmap peakMap;
		Bitmap blurMap;

		int rangeBoxCount;
		int baseCount;
		int entries;
		int useBases;

		bool FromStart;
		ColourRank ColRank;
		bool overlayLengths;
		bool groupsToSide;
		const int THE_RANGE = 200;
		const int HALFING_NUM = 50;
		const double HALFING_VAL = 0.3;
		const double COL2_SCALE = 0.5;
		const int SIDE_BOX_SIZE = 15;
		const double BLUR_RANGE = 3;

		public RegionChangeMap(Dictionary<string, List<List<double>>> spatialDict, ColourRank cr, int bases, int divisions, bool fromStart)
		{
			groupsToSide = true;
			ColRank = cr;
			useBases = bases;
			VarData = new List<List<double>>();
			Data = DictToData(spatialDict);
			MakeSideColours ();
			overlayLengths = false;
			rangeBoxCount = divisions;
			baseCount = Data[0].Count;
			entries = Data.Count;
			fromStart = FromStart;
			PadSpatialWithNegOnes ();
			MakeVarData();
		}

		public RegionChangeMap(Dictionary<string, List<List<double>>> spatialDict, ColourRank cr, List<double> val, int bases, int divisions, bool fromStart)
		{
			groupsToSide = true;
			ColRank = cr;
			useBases = bases;
			VarData = new List<List<double>>();
			Data = DictToData(spatialDict);
			MakeSideColours ();
			rankVal = val;
			overlayLengths = true;
			rangeBoxCount = divisions;
			baseCount = Data[0].Count;
			entries = Data.Count;
			fromStart = FromStart;
			PadSpatialWithNegOnes ();
			MakeVarData();
		}

		public RegionChangeMap(List<List<double>> spatial, List<double> val, int bases, int divisions, bool fromStart)
		{
			useBases = bases;
			VarData = new List<List<double>>();
			Data = spatial;
			rankVal = val;
			overlayLengths = true;
			rangeBoxCount = divisions;
			baseCount = spatial[0].Count;
			entries = spatial.Count;
			fromStart = FromStart;
			PadSpatialWithNegOnes ();
			MakeVarData();
		}

		public RegionChangeMap(List<List<double>> spatial, int divisions, int bases, bool fromStart)
		{
			useBases = bases;
			VarData = new List<List<double>>();
			Data = spatial;
			overlayLengths = false;
			rangeBoxCount = divisions;
			baseCount = spatial[0].Count;
			entries = spatial.Count;
			fromStart = FromStart;
			PadSpatialWithNegOnes ();
			MakeVarData();
		}

		protected List<List<double>> DictToData(Dictionary<string, List<List<double>>> spd)
		{
			List<List<double>> ldubs = new List<List<double>> ();
			sideBoxes = new Dictionary<string, int> ();

			foreach (KeyValuePair<string, List<List<double>>> kvp in spd) {
				int next = 0;
				foreach(List<double> ld in kvp.Value)
				{
					next++;
					ldubs.Add (ld);
				}
				sideBoxes.Add (kvp.Key, next);
			}
			return ldubs;
		}

		protected void MakeSideColours()
		{
			sideColours = new List<Color> ();

			double len = (double)sideBoxes.Count;

			if (ColRank == ColourRank.Sequential) {
				double r = 190;
				double g = 40;
				double b = 180;

				for (int i = 1; i <= (int)len; i++) {
					double scale = ((double)i) / len;

					sideColours.Add (Color.FromArgb ((int)(r * scale), (int)(g * scale), (int)(b * scale)));
				}
			} else {
				Random rand = new Random ();

				for (int i = 1; i <= (int)len; i++) {
					sideColours.Add (Color.FromArgb ((int)(rand.NextDouble() * 255), (int)(rand.NextDouble() * 255), (int)(rand.NextDouble() * 255)));
				}
			}
		}

		protected void PadSpatialWithNegOnes()
		{
			List<List<double>> allFixed = new List<List<double>> ();

			int longest = 0;

			foreach (List<double> ld in Data) {
				longest = Math.Max (longest, ld.Count);
			}

			for (int i = 0; i < Data.Count; i++) {
				List<double> nextfix = new List<double> ();

				int thisLen = Data [i].Count;
				int diff = longest - thisLen;

				if (FromStart) {
					for (int j = 0; j < thisLen; j++) {
						nextfix.Add (Data [i] [j]);
					}
					for (int j = 0; j < diff; j++) {
						nextfix.Add (-1);
					}
				} else {
					for (int j = 0; j < diff; j++) {
						nextfix.Add (-1);
					}
					for (int j = 0; j < thisLen; j++) {
						nextfix.Add (Data [i] [j]);
					}
				}

				allFixed.Add (nextfix);
			}

			Data = allFixed;
		}

		protected void MakeVarData()
		{
			for (int j = 0; j < entries; j++)
			{
				VarData.Add(new List<double>());
			}
			for (int i = 0; i < baseCount; i++)
			{
				double cumu = 0;
				double counted = 0;
				for (int j = 0; j < entries; j++)
				{
					cumu += Data[j][i];
					counted++;
				}

				double totalRate = cumu / counted;
				for (int j = 0; j < entries; j++)
				{
					VarData[j].Add(Math.Abs(Data[j][i] - totalRate));
				}
			}
		}

		public List<string> GetStrs()
		{
			List<string> ls = new List<string> ();

			ls.Add ("Data Mapped");
			ls.Add ("Change Scale Mapped");
			ls.Add ("Data Smooth Map");

			return ls;
		}

		public List<Bitmap> GetBmps()
		{
			MakeBitMap ();
			List<Bitmap> bl = new List<Bitmap> ();

			bl.Add (peakMap);
			bl.Add (varMap);
			bl.Add (blurMap);

			return bl;
		}

		public void MakeBitMap()
		{
			varMap = MakeBmap (VarData, ColourScale.GreenRed);
			peakMap = MakeBmap (Data, ColourScale.GreenBlue);
			blurMap = MakeBlurMap (peakMap);
		}

		public Bitmap GetPeakBitmap()
		{
			return peakMap;
		}

		public Bitmap GetVarBitmap()
		{
			return varMap;
		}

		protected Bitmap MakeBlurMap(Bitmap bmp)
		{
			Bitmap blur = new Bitmap (bmp);
			int xLen = bmp.Width;
			int yLen = bmp.Height;
			int xStart = 0;

			if (groupsToSide) {
				xStart = SIDE_BOX_SIZE;
			}
			double r; double b; double g;
			double Cu; double dist;
			for (int x = xStart; x < xLen; x++) {
				for (int y = 0; y < yLen; y++) {
					
					Color c = bmp.GetPixel (x, y);
					r = (double)c.R; g = (double)c.G; b = (double)c.B; Cu = 1;

					for (double i = x - BLUR_RANGE; i <= x + BLUR_RANGE; i++) {
						for (double j = y - BLUR_RANGE; j <= y + BLUR_RANGE; j++) {
							
							if (i >= xStart && j >= 0 && i < xLen && j < yLen && (x != i && y != j)) {
								Color inCol = bmp.GetPixel ((int)i, (int)j);
								dist = 1/GetDistance (x, y, i, j);
								Cu += dist;
								r += ((double)inCol.R) * dist;
								g += ((double)inCol.G) * dist;
								b += ((double)inCol.B) * dist;
							}
						}
					}
					blur.SetPixel (x, y, Color.FromArgb ((int)(r / Cu), (int)(g / Cu), (int)(b / Cu)));
				}
			}

			return blur;
		}

		protected double GetDistance(double x, double y, double x1, double y1)
		{
			double dx = x - x1; double dy = y - y1;

			return Math.Sqrt ((dx * dx) + (dy * dy));
		}

		protected void DrawSideBoxes(Bitmap bmp)
		{
			int oldY = 0;
			int colInd = 0;
			int pixSave = 0;
			int rollover = 0;
			foreach (KeyValuePair<string, int> kvp in sideBoxes) {
				
				int divInc = rollover;
				int pixInc = pixSave;

				for (int y = 0; y < kvp.Value; y++) {
					divInc++;
					if (divInc % rangeBoxCount == 0) {
						for (int x = 0; x < SIDE_BOX_SIZE ; x++) {
							bmp.SetPixel (x, pixInc, sideColours[colInd]);
						}
						pixInc++;
					}
				}
				rollover = divInc;
				pixSave = pixInc;
				oldY += kvp.Value;
				colInd++;
			}
		}

		protected Bitmap MakeBmap(List<List<double>> colScal, ColourScale cs)
		{
			int xPix = baseCount;
			int xStart = 0;
			if (groupsToSide) {
				xPix += SIDE_BOX_SIZE ;
				xStart = SIDE_BOX_SIZE ;
			}
			int yPix = entries / rangeBoxCount;
			if (entries % rangeBoxCount != 0)
				yPix++;
			Bitmap bmp = new Bitmap(xPix, yPix);

			double min = 9999999;
			double max = 0;

			List<double> all = new List<double>();
			for (int i = 0; i < baseCount; i++)
			{
				for (int j = 0; j < entries; j++)
				{
					if (double.IsNaN (colScal [j] [i])) {
						colScal [j] [i] = 0;
					}
					all.Add(colScal[j][i]);
					min = Math.Min(min, colScal[j][i]);
					max = Math.Max(max, colScal[j][i]);
				}
			}

			double stdDev = Statistics.StandardDeviation(all);
			double mean = Statistics.Mean(all);

			for (int i = 0; i < baseCount; i++)
			{
				int divInc = 0;
				int pixInc = 0;
				List<Color> cols = new List<Color>();
				for (int j = 0; j < entries; j++)
				{
					divInc++;
					cols.Add(GetScaledColor(min, max, mean, stdDev, colScal[j][i], cs));
					if (divInc % rangeBoxCount == 0)
					{
						bmp.SetPixel(i + xStart, pixInc, MergeColList(cols));
						pixInc++;
						cols.Clear();
					}
				}
			}
			if(groupsToSide)
			{
				DrawSideBoxes (bmp);
			}
			if (overlayLengths)
			{
				AddOverlay(bmp, cs);
			}

			return bmp;
		}

		protected void AddOverlay(Bitmap bmp, ColourScale cs)
		{
			int divInc = 0;
			int pixInc = 0;
			int xStart = 0;
			if (groupsToSide)
				xStart = SIDE_BOX_SIZE;
			double cumu = 0;
			double cnt = 0;
			for (int j = 0; j < entries; j++)
			{
				cumu += rankVal[j];
				cnt++;
				divInc++;
				if (divInc % rangeBoxCount == 0)
				{
					int blen = Math.Min((int)(cumu / cnt), baseCount-1);
					for (int i = 0; i < blen; i++)
					{
						SetOverLayPix(cs, bmp, i + xStart, pixInc);
					}
					pixInc++;
				}
			}
		}

		protected void SetOverLayPix(ColourScale cs, Bitmap bmp, int x, int y)
		{
			Color stcol = bmp.GetPixel(x, y);
			Color next;
			if (cs == ColourScale.GreenBlue)
			{
				next = Color.FromArgb(Math.Min((int)stcol.R + 120, 255), Math.Min((int)stcol.G + 10, 255), Math.Min((int)stcol.B + 10, 255));
			}
			else
			{
				next = Color.FromArgb(Math.Min((int)stcol.R + 10, 255), Math.Min((int)stcol.G + 10, 255), Math.Min((int)stcol.B + 120, 255));
			}
			bmp.SetPixel(x, y, next);
		}

		protected Color MergeColList(List<Color> lcol)
		{
			double r = 0; double g = 0; double b = 0;
			foreach (Color c in lcol)
			{
				r += (int)c.R;
				b += (int)c.B;
				g += (int)c.G;
			}
			double cnt = lcol.Count;

			r /= cnt;
			b /= cnt;
			g /= cnt;

			return Color.FromArgb((int)r, (int)g, (int)b);
		}

		protected Color GetScaledColor(double min, double max, double mean, double stdDev, double v, ColourScale cs)
		{
			if (v > 0)
			{
				double st2 = mean + stdDev * COL2_SCALE;

				double scale = (v - min) / (st2 - min);

				int c1; int c2;

				if (scale < 1)
				{
					c1 = 30 + (int)(225.0 * scale);
					c2 = 30;
				}
				else
				{
					scale = (v - st2) / (max - st2);
					c1 = 255;
					c2 = 30 + (int)(225.0 * scale);
				}

				if (cs == ColourScale.GreenBlue)
				{
					return Color.FromArgb(0, c1, c2);
				}
				else
				{
					return Color.FromArgb(c2, c1, 0);
				}
			}
			else
				return Color.Black;
		}
	}
}