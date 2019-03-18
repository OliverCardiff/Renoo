using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace GeneToAnno
{
	public class RSplitUnit<TypeOut> : IComparable
	{
		public string ID;
		public TypeOut rankData;
		public double theValue;

		public RSplitUnit(string id, double val, TypeOut rankDat)
		{
			ID = id;
			rankData = rankDat;
			theValue = val;
		}

		public int CompareTo(object other)
		{
			if (other == null)
				return 1;
			
			RSplitUnit<TypeOut> ot = (other as RSplitUnit<TypeOut>);

			if (ot != null) {
				return this.theValue.CompareTo (ot.theValue);
			} else
				throw new ArgumentException ("Incompatible Comparison! Object is not an RSplitUnit<TypeOut>");
		}
	}
	public abstract class RSplitContainer
	{
		public abstract Dictionary<int, List<RSplitUnit<double>>> GetGroupedList ();
		public abstract List<RSplitUnit<double>> GetRankedList();
	}
	public class RSplitContainer<TypeOut> : RSplitContainer
	{
		protected List<RSplitUnit<TypeOut>> fullList;
		protected TypeOut NonEntry;
		public bool typeOutIsList;
		protected Dictionary<string, List<string>> inGroups;
		protected Dictionary<int, List<RSplitUnit<TypeOut>>> bigDivs;
		protected List<string> IdsToUse;
		protected bool FilterByIds;

		public int MainGroupCount { get { return bigDivs.Count; } } 

		public override List<RSplitUnit<double>> GetRankedList()
		{
			if (!typeOutIsList) {
				return (fullList as List<RSplitUnit<double>>);
			} else {
				return GetReplacementList (fullList);
			}
		}

		public override Dictionary<int, List<RSplitUnit<double>>> GetGroupedList()
		{
			if (!typeOutIsList) {
				return (bigDivs as Dictionary<int, List<RSplitUnit<double>>>);
			} else {
				Dictionary<int, List<RSplitUnit<double>>> dicto = new Dictionary<int, List<RSplitUnit<double>>> ();

				foreach (KeyValuePair<int, List<RSplitUnit<TypeOut>>> kvp in bigDivs) {
					dicto.Add(kvp.Key, GetReplacementList(kvp.Value));
				}
				return dicto;
			}
		}

		protected List<RSplitUnit<double>> GetReplacementList(List<RSplitUnit<TypeOut>> lrsu)
		{
			List<RSplitUnit<double>> replacementList = new List<RSplitUnit<double>> ();

			foreach (RSplitUnit<TypeOut> rsu in lrsu) {
				double cumu = 0;
				int accu = 0;
				List<double> all = rsu.rankData as List<double>;

				foreach (double d in all) {
					cumu += d;
					accu++;
				}
				double tot = cumu / ((double)accu);
				RSplitUnit<double> newU = new RSplitUnit<double> (rsu.ID, rsu.theValue, tot);
				replacementList.Add (newU);
			}

			return replacementList;
		}

		public RSplitContainer (TypeOut nullExpr, bool isListed)
		{
			FilterByIds = false;
			typeOutIsList = isListed;
			NonEntry = nullExpr;
			fullList = new List<RSplitUnit<TypeOut>> ();
		}

		public void AddIDFilterList(List<string> strs)
		{
			IdsToUse = strs;
			FilterByIds = true;
		}

		public void MergeDictosToList(Dictionary<string, double> dRank, Dictionary<string, TypeOut> dOut, bool IncludeNonEntry, bool seqMatch)
		{
			if (seqMatch) {
				foreach (KeyValuePair<string, double> kvp in dRank) {
					RSplitUnit<TypeOut> rsp = null;

					string strtest = kvp.Key.Split ('@') [0];

					if (!FilterByIds || IdsToUse.Contains (strtest)) {
					
						if (dOut.ContainsKey (kvp.Key)) {
							rsp = new RSplitUnit<TypeOut> (kvp.Key, kvp.Value, dOut [kvp.Key]);
						} else {
							if (IncludeNonEntry) {
								rsp = new RSplitUnit<TypeOut> (kvp.Key, kvp.Value, NonEntry);
							}
						}

						if (rsp != null) {
							fullList.Add (rsp);
						}
					} 
				}
			} else {
				Dictionary<string, double> mergeRank = new Dictionary<string, double> ();
				Dictionary<string, double> countRank = new Dictionary<string, double> ();

				foreach (KeyValuePair<string, double> kvp in dRank) {
					string strtest = kvp.Key.Split ('@') [0];

					if (!mergeRank.ContainsKey (strtest)) {
						mergeRank.Add (strtest, kvp.Value);
						countRank.Add (strtest, 1);
					} else {
						mergeRank [strtest] += kvp.Value;
						countRank [strtest]++;
					}
				}
				Dictionary<string, double> AvgdRank = new Dictionary<string, double> ();

				foreach (KeyValuePair<string, double> kvp in mergeRank) {
					AvgdRank.Add (kvp.Key, kvp.Value / countRank [kvp.Key]);
				}

				foreach (KeyValuePair<string, TypeOut> kvp in dOut) {
					RSplitUnit<TypeOut> rsp = null;
					string strtest = kvp.Key.Split ('@') [0];

					if (!FilterByIds || IdsToUse.Contains (strtest)) {

						if (AvgdRank.ContainsKey (strtest)) {
							rsp = new RSplitUnit<TypeOut> (kvp.Key, AvgdRank [strtest], kvp.Value);
						} else {
							if (IncludeNonEntry) {
								rsp = new RSplitUnit<TypeOut> (kvp.Key, 0, kvp.Value);
							}
						}

						if (rsp != null) {
							fullList.Add (rsp);
						}
					}
				}
			}
			if (fullList.Count == 0) {
				throw new Exception ("Cannot reconcile sequence features. No shared parent id?");
			}
			fullList.Sort ();
		}

		public Dictionary<string, List<TypeOut>> GetDivisionDict(int divs, int mainDiv, int thenSubDiv)
		{
			List<RSplitUnit<TypeOut>> flist = GetDivisionofList (divs, mainDiv);

			fullList = flist;

			return GetDivisionDict (thenSubDiv, flist);
		}

		protected List<RSplitUnit<TypeOut>> GetDivisionofList(int divs, int chosen)
		{
			Dictionary<int, List<RSplitUnit<TypeOut>>> dict = new Dictionary<int, List<RSplitUnit<TypeOut>>> ();

			int RemainingCount = fullList.Count;
			int Taken = 0;

			if (divs > 1) {
				int lim = fullList.Count / divs - 1;

				if (fullList [lim].theValue == 0) {
					List<RSplitUnit<TypeOut>> coll = new List<RSplitUnit<TypeOut>> ();
					int takenSlots = 0;
					foreach (RSplitUnit<TypeOut> sp in fullList) {
						if (sp.theValue == 0) {
							coll.Add (sp);
							takenSlots++;
						} else
							break;
					}
					RemainingCount -= takenSlots;
					dict.Add (0, coll);
					Taken = takenSlots;
				}
			}

			int oldLim = Taken;
			for (int i = 0; i < divs; i++) {
				int lim = Taken + (RemainingCount / divs) * (i + 1);
				if (i == divs - 1) {
					lim = Taken + RemainingCount - 1;
				}

				int label = (i+1);

				List<RSplitUnit<TypeOut>> thisCollect = new List<RSplitUnit<TypeOut>> ();

				for (int j = oldLim; j < lim; j++) {
					thisCollect.Add (fullList [j]);
				}

				dict.Add (label, thisCollect);
				oldLim = lim + 1;
			}

			return dict[chosen];
		}

		public Dictionary<string, List<TypeOut>> GetDivisionDict(int divs)
		{
			return GetDivisionDict (divs, fullList);
		}

		protected Dictionary<string, List<TypeOut>> GetDivisionDict(int divs, List<RSplitUnit<TypeOut>> flist)
		{
			Dictionary<string, List<TypeOut>> dict = new Dictionary<string, List<TypeOut>> ();
			Dictionary<string, List<string>> strDict = new Dictionary<string, List<string>> ();
			bigDivs = new Dictionary<int, List<RSplitUnit<TypeOut>>> ();

			int RemainingCount = flist.Count;
			int Taken = 0;

			if (divs > 1) {
				int lim = flist.Count / divs - 1;

				if (flist [lim].theValue == 0) {
					List<TypeOut> coll = new List<TypeOut> ();
					List<string> coll2 = new List<string> ();
					List<RSplitUnit<TypeOut>> coll3 = new List<RSplitUnit<TypeOut>> ();
					int takenSlots = 0;
					foreach (RSplitUnit<TypeOut> sp in flist) {
						if (sp.theValue == 0) {
							coll.Add (sp.rankData);
							coll2.Add (sp.ID);
							coll3.Add (sp);
							takenSlots++;
						} else
							break;
					}
					RemainingCount -= takenSlots;
					dict.Add ("0.00 - 0.00", coll);
					strDict.Add ("0.00 - 0.00", coll2);
					bigDivs.Add (0, coll3);
					Taken = takenSlots;
				}
			}

			int oldLim = Taken;
			for (int i = 0; i < divs; i++) {
				int lim = Taken + (RemainingCount / divs) * (i + 1);
				if (i == divs - 1) {
					lim = Taken + RemainingCount - 1;
				}

				double lo = flist [lim].theValue;
				double hi = flist [oldLim].theValue;

				string label = GetTrimmedDoubleString (lo) + " - " + GetTrimmedDoubleString (hi);

				List<TypeOut> thisCollect = new List<TypeOut> ();
				List<string> thisCollect2 = new List<string> ();
				List<RSplitUnit<TypeOut>> thisCollect3 = new List<RSplitUnit<TypeOut>> ();

				for (int j = oldLim; j < lim; j++) {
					thisCollect.Add (flist [j].rankData);
					thisCollect2.Add (flist [j].ID);
					thisCollect3.Add (flist [j]);
				}

				while (dict.ContainsKey (label)) {
					label += "i";
				}

				dict.Add (label, thisCollect);
				strDict.Add (label, thisCollect2);
				bigDivs.Add ((i + 1), thisCollect3);
				oldLim = lim + 1;
			}
			inGroups = strDict;
			return dict;
		}

		protected string GetTrimmedDoubleString(double d)
		{
			string sr = d.ToString ();
			if (sr.Length > 6) {
				sr = sr.Substring (0, 6);
			}

			return sr;
		}

		public NumericalText GetData (bool fromStart)
		{
			NumericalText nt = new NumericalText ();
			List<List<string>> toutlist = new List<List<string>> ();
			List<string> touts = new List<string> ();
			List<double> dubs = new List<double> ();
			List<string> strs = new List<string> ();
			List<double> rank = new List<double> ();

			bool madeFullList = false;

			int maxLen = 0;
			if (typeOutIsList) {
				foreach (RSplitUnit<TypeOut> rsu in fullList) {
					List<double> theItem = rsu.rankData as List<double>;
					maxLen = Math.Max (maxLen, theItem.Count);
				}
			}

			double r = 1;
			foreach (RSplitUnit<TypeOut> rsu in fullList) {
				if (typeOutIsList) {
					List<double> theItem = rsu.rankData as List<double>;

					if (!madeFullList) {
						for (int i = 0; i < maxLen; i++) {
							toutlist.Add (new List<string> ());
						}
						madeFullList = true;
					}

					int diff = maxLen - theItem.Count ;

					if (diff != 0) {
						for (int i = 0; i < maxLen; i++) {
							if (fromStart) {
								if (theItem.Count > i) {
									toutlist [i].Add (theItem [i].ToString ());
								} else {
									toutlist [i].Add ("x");
								}
							} else {
								if (i < diff) {
									toutlist [i].Add ("x");
								} else {
									toutlist [i].Add (theItem [i-diff].ToString ());
								}
							}
						}
					} else {
						for (int i = 0; i < maxLen; i++) {
							toutlist [i].Add (theItem [i].ToString ());
						}
					}

				} else {
					touts.Add (rsu.rankData.ToString());
				}
				dubs.Add (rsu.theValue);
				strs.Add (rsu.ID);
				rank.Add (r);
				r++;
			}

			nt.Titles.Add ("IDs");
			if (!typeOutIsList) {
				nt.Titles.Add ("Output data");
			} else {
				for (int i = 1; i <= toutlist.Count; i++) {
					nt.Titles.Add ("Output_data_" + i);
				}
			}
			nt.Titles.Add (" 'Ranked by' ");
			nt.Titles.Add ("Rank");

			nt.Tags.Add (strs);
			if (!typeOutIsList) {
				nt.Tags.Add (touts);
			} else {
				for (int i = 0; i < toutlist.Count; i++) {
					nt.Tags.Add (toutlist [i]);
				}
			}
			nt.Data.Add (dubs);
			nt.Data.Add (rank);

			return nt;
		}

		public void FilterOnParentID(List<string> oldIds)
		{
			int len = oldIds.Count;
			for (int i = 0; i < len; i++) {
				
			}
		}

		public List<string> GetAllGeneIDs(int st, int ed)
		{
			List<string> lout = new List<string> ();
			if (inGroups != null) {
				int incr = 1;
				foreach (KeyValuePair<string, List<string>> rsu in inGroups) {
					if (incr >= st && incr <= ed) {
						foreach(string str in rsu.Value)
						{
							string stradd = str.Split ('@') [0];
							lout.Add (stradd);
						}
					}
					incr++;
				}
			}
			return lout;
		}

		public void WriteFastaFile(string fileName)
		{
			List<RSplitUnit<string>> ls = fullList as List<RSplitUnit<string>>;

			using (StreamWriter sw = new StreamWriter (fileName)) {
				foreach (RSplitUnit<string> rsu in ls) {

					sw.WriteLine (">" + rsu.ID);
					sw.WriteLine (rsu.rankData);
				}
			}

			MainData.UpdateLog ("Success! Saved " + MainData.ShortFileName (fileName), true);
		}

	}
}

