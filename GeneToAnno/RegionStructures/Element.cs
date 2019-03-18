using System;
using System.Collections.Generic;

namespace GeneToAnno
{
	public enum Sense {Sense, AntiSense, None};

	public class Element : IComparable
	{
		public int Start { get; set; }
		public int End { get; set; }
		public int Length { get { return length; } }
		public Sense Sense { get; set; }
		protected List<SeqRead> Reads { get; set; }
		protected List<Variant> Variants { get; set; }

		protected int length;

		public Element ()
		{
			Start = 0;
			End = 0;
			length = 0;
			Sense = Sense.Sense;
		}

		public Element (int _start, int _end, Sense _sense)
		{
			Sense = _sense;
			Start = _start;
			End = _end;
			length = _end - _start;
		}

		protected void DivsToBasePos(int st, int ed, out int start, out int end)
		{
			List<int> bounds = GetSectionBounds (20);
			if (st < 1)
				st = 1;
			if (ed > 20)
				ed = 20;
			start = Start;
			if (st > 1)
				start = bounds [st - 2];
			end = bounds [ed - 1];
		}

		public int CompareTo(object obj) {
			if (obj == null) return 1;

			Element other = obj as Element;
			if (other != null) {
				int result = Start.CompareTo (other.Start);

				if (Sense == Sense.AntiSense) {
					return result * -1;
				} else {
					return result;
				}
			}
			else 
				throw new ArgumentException("Object is not an Element!");
		}

		public bool HasOverLap(SeqRead sq)
		{
			if ((sq.Start >= Start && sq.Start <= End) || (sq.End <= End && sq.End >= Start) || (sq.Start <= Start && sq.End >= End)) {
				return true;
			} else
				return false;
		}

		public bool Contains(Variant vr)
		{
			if (vr.Start >= Start && vr.Start <= End) {
				return true;
			} else
				return false;
		}

		public bool HasOverLap(SeqRead sq, int st, int ed)
		{
			if ((sq.Start >= st && sq.Start <= ed) || (sq.End <= ed && sq.End >= st) || (sq.Start <= st && sq.End >= ed)) {
				return true;
			} else
				return false;
		}

		public void AddRead(SeqRead _read)
		{
			if (Reads == null) {
				Reads = new List<SeqRead> ();
			}
			MainData.LoadedSeqs++;
			lock (Reads) {
				Reads.Add (_read);
			}
		}

		public void AddVariant(Variant _var)
		{
			if (Variants == null) {
				Variants = new List<Variant> ();
			}
			MainData.LoadedVars++;
			lock (Variants) {
				Variants.Add (_var);
			}
		}

		public void AddReads(List<SeqRead> _reads)
		{
			if (Reads == null) {
				Reads = new List<SeqRead> ();
			}
			lock (Reads) {
				Reads.AddRange (_reads);
			}
		}

		public string GetBaseSeqAll(string scaff)
		{
			return MainData.Genome.GetScaffold (scaff).GetBases (Start, End);
		}

		public string GetBaseSeqSubset(string scaff, int st, int ed)
		{
			int start; int end;
			DivsToBasePos (st, ed, out start, out end);

			return MainData.Genome.GetScaffold (scaff).GetBases (start, end);
		}

		public string GetBaseSeqBaseLimit(string scaff, int bases, bool doStart)
		{
			int st = Start;
			int ed = End;
			if (this.Sense == Sense.AntiSense) {
				doStart = !doStart;
			}
			if (doStart && (Start + bases) < End)
				ed = Start + bases;
			if (!doStart && (End - bases) > Start)
				st = End - bases;
			
			return MainData.Genome.GetScaffold (scaff).GetBases (st, ed);
		}

		public double GetVarCount(bool norm)
		{
			if (Variants != null) {
				if (norm) {
					return ((double)Variants.Count / (double)Length);
				}
				else
					return Variants.Count;
			} else {
				return 0;
			}
		}

		public double GetAllHetCount(bool norm, string samp, bool useSamp)
		{
			double res = 0;

			if (Variants != null) {
				if (useSamp) {
					foreach (Variant v in Variants) {
						res += v.IsHet (samp);
					}
				} else {
					List<string> mSamp = MainData.VariantSamples;
					foreach (Variant v in Variants) {
						foreach (string st in mSamp) {
							res += v.IsHet (st);
						}
					}
				}
			}
			if (norm) {
				return res / (double)this.Length;
			} else {
				return res;
			}
		}

		public double GetAllHomCount(bool norm, string samp, bool useSamp)
		{
			int res = 0;
			if (Variants != null) {
				if (useSamp) {
					foreach (Variant v in Variants) {
						res += v.IsHom (samp);
					}
				} else {
					List<string> mSamp = MainData.VariantSamples;
					foreach (Variant v in Variants) {
						foreach (string st in mSamp) {
							res += v.IsHom (st);
						}
					}
				}
			}
			if (norm) {
				return res / (double)this.Length;
			} else {
				return res;
			}
		}

		public double GetVarCountSubset(int st, int ed, bool norm)
		{
			int res = 0;
			int start = 0; int end = 0;
			if (this.Sense == Sense.AntiSense) {
				ReverseSubSection (ref st, ref ed);
			}
			if (Variants != null) {
				DivsToBasePos (st, ed, out start, out end);

				foreach (Variant v in Variants) {
					if (v.Start >= start && v.Start <= end)
						res++;
				}
			}
			if (norm && (end - start != 0)) {
				return ((double)res / ((double)(end - start)));
			} else
				return res;
		}

		public double GetAllHetCountSubset(int st, int ed, bool norm, string samp, bool useSamp)
		{
			int res = 0;
			int start = 0; int end = 0;
			if (this.Sense == Sense.AntiSense) {
				ReverseSubSection (ref st, ref ed);
			}
			if (Variants != null) {
				List<string> mSamp = MainData.VariantSamples;
				DivsToBasePos (st, ed, out start, out end);

				if (!useSamp) {
					foreach (Variant v in Variants) {
						if (v.Start >= start && v.Start <= end)
							foreach (string str in mSamp) {
								res += v.IsHet (str);
							}
					}
				} else {
					foreach (Variant v in Variants) {
						if (v.Start >= start && v.Start <= end)
							res += v.IsHet (samp);
					}
				}
			}
			if (norm && (end - start != 0)) {
				return ((double)res / ((double)(end - start)));
			} else
				return res;
		}

		public double GetAllHomCountSubset(int st, int ed, bool norm, string samp, bool useSamp)
		{
			int res = 0;
			int start = 0; int end = 0;

			if (this.Sense == Sense.AntiSense) {
				ReverseSubSection (ref st, ref ed);
			}

			if (Variants != null) {
				List<string> mSamp = MainData.VariantSamples;
				DivsToBasePos (st, ed, out start, out end);

				if (!useSamp) {
					foreach (Variant v in Variants) {
						if (v.Start >= start && v.Start <= end)
							foreach (string str in mSamp) {
								res += v.IsHom (str);
							}
					}
				} else {
					foreach (Variant v in Variants) {
						if (v.Start >= start && v.Start <= end)
							res += v.IsHom (samp);
					}
				}
			}
			if (norm && (end - start != 0)) {
				return ((double)res / ((double)(end - start)));
			} else
				return res;
		}

		public double GetVarCountBaseLimit(int bases, bool doStart, bool norm)
		{
			int res = 0;
			int st = Start;
			int ed = End;
			if (this.Sense == Sense.AntiSense) {
				doStart = !doStart;
			}

			if (Variants != null) {
				if (doStart && (Start + bases) < End)
					ed = Start + bases;
				if (!doStart && (End - bases) > Start)
					st = End - bases;

				foreach (Variant v in Variants) {
					if (v.Start >= st && v.Start <= ed)
						res++;
				}
			}
			if (norm && (ed - st != 0)) {
				return ((double)res / ((double)(ed - st)));
			} else
				return res;
		}

		public double GetAllHetCountBaseLimit(int bases, bool doStart, bool norm, string samp, bool useSamp)
		{
			int res = 0;
			int st = Start;
			int ed = End;
			if (this.Sense == Sense.AntiSense) {
				doStart = !doStart;
			}

			if (Variants != null) {
				List<string> mSamp = MainData.VariantSamples;
				if (doStart && (Start + bases) < End)
					ed = Start + bases;
				if (!doStart && (End - bases) > Start)
					st = End - bases;

				if (!useSamp) {
					foreach (Variant v in Variants) {
						if (v.Start >= st && v.Start <= ed)
							foreach (string str in mSamp) {
								res += v.IsHet (str);
							}
					}
				} else {
					foreach (Variant v in Variants) {
						if (v.Start >= st && v.Start <= ed)
							res += v.IsHet (samp);
					}
				}
			}
			if (norm && (ed - st != 0)) {
				return ((double)res / ((double)(ed - st)));
			} else
				return res;
		}

		public double GetAllHomCountBaseLimit(int bases, bool doStart, bool norm, string samp, bool useSamp)
		{
			int res = 0;
			int st = Start;
			int ed = End;
			if (this.Sense == Sense.AntiSense) {
				doStart = !doStart;
			}

			if (Variants != null) {
				List<string> mSamp = MainData.VariantSamples;
				if (doStart && (Start + bases) < End)
					ed = Start + bases;
				if (!doStart && (End - bases) > Start)
					st = End - bases;

				if (!useSamp) {
					foreach (Variant v in Variants) {
						if (v.Start >= st && v.Start <= ed)
							foreach (string str in mSamp) {
								res += v.IsHom (str);
							}
					}
				} else {
					foreach (Variant v in Variants) {
						if (v.Start >= st && v.Start <= ed)
							res += v.IsHom (samp);
					}
				}
			}
			if (norm && (ed - st != 0)) {
				return ((double)res / ((double)(ed - st)));
			} else
				return res;
		}

		public double GetCoverageBaseLimit(int bases, bool doStart)
		{
			if (this.Sense == Sense.AntiSense) {
				doStart = !doStart;
			}
			int st = Start; int ed = End;
			if (doStart && (Start + bases) < End)
				ed = Start + bases;
			if (!doStart && (End - bases) > Start)
				st = End - bases;

			return GetCoverageSection (st, ed);
		}

		public double GetCoverageDepthBaseLimit(int bases, bool doStart)
		{
			if (this.Sense == Sense.AntiSense) {
				doStart = !doStart;
			}
			int st = Start; int ed = End;
			if (doStart && (Start + bases) < End)
				ed = Start + bases;
			if (!doStart && (End - bases) > Start)
				st = End - bases;

			return GetCoverageDepthScoreSection (st, ed);
		}

		public virtual bool TryGetCoverageByDivision(int division, out List<double> ld)
		{
			if (length > division) {
				ld = new List<double> (division);

				List<int> boundaries = GetSectionBounds (division);

				int lastBound = Start;
				foreach (int ib in boundaries) {
					ld.Add (GetCoverageSection (lastBound, ib));
					lastBound = ib;
				}

				if (this.Sense == Sense.AntiSense) {
					ld.Reverse ();
				}

				return true;
			} else {
				ld = null;
				return false;
			}
		}

		public double GetCoverageDepthSubSet(int st, int ed)
		{
			if (this.Sense == Sense.AntiSense) {
				ReverseSubSection (ref st, ref ed);
			}
			int start; int end;
			DivsToBasePos (st, ed, out start, out end);

			return GetCoverageDepthScoreSection (start, end);
		}

		public double GetCoverageSubSet(int st, int ed)
		{
			if (this.Sense == Sense.AntiSense) {
				ReverseSubSection (ref st, ref ed);
			}
			int start; int end;
			DivsToBasePos (st, ed, out start, out end);

			return GetCoverageSection (start, end);
		}

		public List<double> GetCoverageSpatialBaseLimit(int bases, bool fromStart)
		{
			int st = Start;
			int ed = End;
			if (this.Sense == Sense.AntiSense) {
				fromStart = !fromStart;
			}
			if (fromStart && (Start + bases) < End)
				ed = Start + bases - 1;
			if (!fromStart && (End - bases) > Start)
				st = End - bases + 1;

			List<double> baseList = new List<double> (ed-st + 1);

			if (Reads != null) {
				for (int i = st; i <= ed; i++) {
					double baseAdd = 0;
					foreach (SeqRead sq in Reads) {
						if (sq.Contains (i)) {
							baseAdd = 1;
							break;
						}
					}
					baseList.Add (baseAdd);
				}
			} else {
				for (int i = st; i <= ed; i++) {
					baseList.Add (0);
				}
			}

			if (this.Sense == Sense.AntiSense) {
				baseList.Reverse ();
			}
			return baseList;
		}

		public List<double> GetCoverageDepthSpatialBaseLimit(int bases, bool fromStart)
		{
			int st = Start;
			int ed = End;
			if (this.Sense == Sense.AntiSense) {
				fromStart = !fromStart;
			}
			if (fromStart && (Start + bases) < End)
				ed = Start + bases - 1;
			if (!fromStart && (End - bases) > Start)
				st = End - bases + 1;

			List<double> baseList = new List<double> (ed-st + 1);

			if (Reads != null) {
				for (int i = st; i <= ed; i++) {
					double baseAdd = 0;
					foreach (SeqRead sq in Reads) {
						if (sq.Contains (i)) {
							baseAdd += 1;
						}
					}
					baseList.Add (baseAdd);
				}
			} else {
				for (int i = st; i <= ed; i++) {
					baseList.Add (0);
				}
			}
			if (this.Sense == Sense.AntiSense) {
				baseList.Reverse ();
			}
			return baseList;
		}

		public virtual bool TryGetCoverageDepthSubSet(int st, int ed, int division, out List<double> ld)
		{
			if (length > division) {
				if (st < 1)
					st = 1;
				if (ed > 20)
					ed = 20;

				if (this.Sense == Sense.AntiSense) {
					ReverseSubSection (ref st, ref ed);
				}
			
				ld = new List<double> ();

				List<int> bounds = GetSectionBounds (division);

				int lastBound = Start;
				if (st > 1)
					lastBound = bounds [st - 2];
				for (int i = st; i <= ed; i++) {
					ld.Add (GetCoverageDepthScoreSection (lastBound, bounds [i - 1]));
					lastBound = bounds [i - 1];
				}

				if (this.Sense == Sense.AntiSense) {
					ld.Reverse ();
				}
				return true;
			} else {
				ld = null;
				return false;
			}
		}

		public virtual bool TryGetCoverageSubSet(int st, int ed, int division, out List<double> ld)
		{
			if (length > division) {
				if (st < 1)
					st = 1;
				if (ed > 20)
					ed = 20;

				if (this.Sense == Sense.AntiSense) {
					ReverseSubSection (ref st, ref ed);
				}

				ld = new List<double> ();

				List<int> bounds = GetSectionBounds (division);

				int lastBound = Start;
				if (st > 1)
					lastBound = bounds [st - 2];
				for (int i = st; i <= ed; i++) {
					ld.Add (GetCoverageSection (lastBound, bounds [i - 1]));
					lastBound = bounds [i - 1];
				}

				if (this.Sense == Sense.AntiSense) {
					ld.Reverse ();
				}
				return true;
			} else {
				ld = null;
				return false;
			}
		}

		public virtual bool TryGetCoverageDepthByDivision(int division, out List<double> ld)
		{
			if (length > division) {
				ld = new List<double> (division);

				List<int> boundaries = GetSectionBounds (division);

				int lastBound = Start;
				foreach (int ib in boundaries) {
					ld.Add (GetCoverageDepthScoreSection (lastBound, ib));
					lastBound = ib;
				}

				if (this.Sense == Sense.AntiSense) {
					ld.Reverse ();
				}

				return true;
			} else {
				ld = null;
				return false;
			}
		}

		protected List<int> GetSectionBounds(int division)
		{
			int divSize = length / division;
			int overflow = length % division;
			int sporadicity = 0;
			if (overflow > 0) {
				sporadicity = (divSize * division) / overflow;
			}
			int currentlyAddedOverflow = 0;
			int sporacityLim = sporadicity;

			List<int> boundaries = new List<int> (division);

			for (int i = 0; i < division; i++) {
				int nextB = Start + divSize + (divSize * i) + currentlyAddedOverflow;

				if(overflow > 0)
				{
					if ((i+1)*divSize >= sporacityLim) {
						nextB++;
						currentlyAddedOverflow++;
						overflow--;
						sporacityLim += sporadicity;
					}
				}
				boundaries.Add (nextB);
			}

			return boundaries;
		}

		public virtual double GetCoverageAll()
		{
			return GetCoverageSection (Start, End);
		}

		protected virtual string GetReadNamesSection(int st, int ed)
		{
			if (Reads != null) {
				string retStr = "";
				foreach (SeqRead sq in Reads) {
					NameRead nq = sq as NameRead;
					if (nq == null) {
						throw new Exception ("Sequences were not loaded with read names!");
					}
					if (HasOverLap (nq, st, ed)) {
						if (retStr.Length > 1) {
							retStr += "\t" + nq.Name;
						} else
							retStr = nq.Name;
					}
				}
				return retStr;
			} else
				return "";
		}
		public virtual string GetReadNamesAll()
		{
			return GetReadNamesSection (Start, End);
		}
		public virtual string GetReadNamesSubset(int st, int ed)
		{
			if (this.Sense == Sense.AntiSense) {
				ReverseSubSection (ref st, ref ed);
			}
			int start; int end;
			DivsToBasePos (st, ed, out start, out end);

			return GetReadNamesSection (start, end);
		}
		public virtual string GetReadNamesBaseLimit(int bases, bool doStart)
		{
			if (this.Sense == Sense.AntiSense) {
				doStart = !doStart;
			}
			int st = Start; int ed = End;
			if (doStart && (Start + bases) < End)
				ed = Start + bases;
			if (!doStart && (End - bases) > Start)
				st = End - bases;

			return GetReadNamesSection (st, ed);
		}
			
		public virtual double GetCoverageSection(int st, int ed)
		{
			if (Reads != null) {
				int len = ed - st + 1;
				List<CovPoint> points = new List<CovPoint> ();
				List<CovPoint> remPoints = new List<CovPoint> ();
				CovPoint OldEnd;
				bool previousWasStart = false;


				foreach (SeqRead sq in Reads) {
					if (HasOverLap(sq, st, ed)) {
						foreach (CovPoint co in points) {
							if (sq.Contains (co.Point)) {
								remPoints.Add (co);
							}
						}
						foreach (CovPoint co in remPoints) {
							points.Remove (co);
						}
						remPoints.Clear ();

						if (sq.Start < st) {
							points.Add (new CovPoint (st, true));
						} else {
							points.Add (new CovPoint (sq.Start, true));
						}

						if (sq.End > ed) {
							points.Add (new CovPoint (ed, false));
						} else {
							points.Add (new CovPoint (sq.End, false));
						}

						points.Sort ();

						previousWasStart = false;
						OldEnd = null;
						foreach (CovPoint co in points) {
							if (co.IsStart) {
								if (previousWasStart) {
									remPoints.Add (co);
								}
							} else {
								if (!previousWasStart) {
									remPoints.Add (OldEnd);
								}
							}
							OldEnd = co;
							previousWasStart = co.IsStart;
						}

						foreach (CovPoint c in remPoints) {
							points.Remove (c);
						}
						remPoints.Clear ();
					}
				}

				int totalGap = 0;
				int lInd = points.Count - 1;

				if (lInd > 0) {
					if (points [0].IsStart) {
						totalGap += points [0].Point - st;
					}
					if (!points [lInd].IsStart) {
						totalGap += ed - points [lInd].Point;
					}
				} else {
					totalGap = len;
				}
				if (lInd > 1) {
					CovPoint previous = null;
					previousWasStart = true;
					foreach (CovPoint co in points) {
						if (previous != null) {
							if (co.IsStart && !previousWasStart) {
								totalGap += co.Point - previous.Point;
							}
						}
						previous = co;
						previousWasStart = co.IsStart;
					}
				}

				return (1 - totalGap / (double)len);
			} else {
				return 0;
			}
		}

		public virtual double GetCoverageDepthScoreAll()
		{
			return GetCoverageDepthScoreSection(Start, End);
		}

		public virtual double GetCoverageDepthScoreSection(int st, int ed)
		{
			if (Reads != null) {
				int len = ed - st;
				if (len == 0)
					len = 1;
				List<CovPoint> points = new List<CovPoint> ();
				List<CovPoint> remPoints = new List<CovPoint> ();
				CovPoint OldEnd;
				bool previousWasStart = false;
				int totalCov = 0;

				foreach (SeqRead sq in Reads) {
					if (HasOverLap(sq, st, ed)) {
						foreach (CovPoint co in points) {
							if (sq.Contains (co.Point)) {
								remPoints.Add (co);
							}
						}
						foreach (CovPoint co in remPoints) {
							points.Remove (co);
						}
						remPoints.Clear ();

						CovPoint nextSt;
						CovPoint nextEd;
						if (sq.Start < st) {
							nextSt = new CovPoint (st, true);
						} else {
							nextSt = new CovPoint (sq.Start, true);
						}

						if (sq.End > ed) {
							nextEd = new CovPoint (ed, false);
						} else {
							nextEd = new CovPoint (sq.End, false);
						}
						points.Add (nextSt);
						points.Add (nextEd);

						totalCov += nextEd.Point - nextSt.Point;
						points.Sort ();

						previousWasStart = false;
						OldEnd = null;
						foreach (CovPoint co in points) {
							if (co.IsStart) {
								if (previousWasStart) {
									remPoints.Add (co);
								}
							} else {
								if (!previousWasStart) {
									remPoints.Add (OldEnd);
								}
							}
							OldEnd = co;
							previousWasStart = co.IsStart;
						}

						foreach (CovPoint c in remPoints) {
							points.Remove (c);
						}
						remPoints.Clear ();
					}
				}

				int totalGap = 0;
				int lInd = points.Count - 1;

				if (lInd > 0) {
					if (points [0].IsStart) {
						totalGap += points [0].Point - st;
					}
					if (!points [lInd].IsStart) {
						totalGap += ed - points [lInd].Point;
					}
				} else {
					totalGap = len;
				}
				if (lInd > 1) {
					CovPoint previous = null;
					previousWasStart = true;
					foreach (CovPoint co in points) {
						if (previous != null) {
							if (co.IsStart && !previousWasStart) {
								totalGap += co.Point - previous.Point;
							}
						}
						previous = co;
						previousWasStart = co.IsStart;
					}
				}

				return (1 - totalGap / (double)len) * ((double)totalCov/(double)len);
			} else {
				return 0;
			}
		}

		public virtual void ReverseSubSection(ref int st, ref int ed)
		{
			int tmpSt = st;
			int tmpEd = ed;

			tmpSt = (20 - ed) + 1;
			tmpEd = (20 - st) + 1;

			st = tmpSt;
			ed = tmpEd;
		}

		protected List<SeqRead> GetSampleReads(string samp)
		{
			if (Reads != null) {
				List<SeqRead> nreads = new List<SeqRead> ();
				foreach (SeqRead sqr in Reads) {
					if (sqr.Sample.ID == samp) {
						nreads.Add (sqr);
					}
				}
				return nreads;
			} else
				return null;
		}
			
		public class CovPoint : IComparable
		{
			public int Point;
			public bool IsStart;

			public CovPoint(int p, bool isS)
			{
				Point = p;
				IsStart = isS;
			}

			public int CompareTo(object other)
			{
				CovPoint oth = (CovPoint)other;

				int res = Point.CompareTo (oth.Point);
				if (res == 0) {
					if (this.IsStart == oth.IsStart) {
						return 0;
					} else if (IsStart) {
						return -1;
					} else {
						return 1;
					}
				}
				return res;
			}
		}
	}
}

