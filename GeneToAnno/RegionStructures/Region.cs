using System;
using System.Collections.Generic;
using MathNet.Numerics.Statistics;

namespace GeneToAnno
{
	public class Region : Element
	{
		public string RegionTypeName { get; set; }
		public string RegionName { get; set; }
		public Dictionary<string, List<FKPM>> FragPerKiloMil { get; set; }
		public BlastHit BlastHit;

		public Dictionary<string, List<MiscElement>> RegElements { get; set; }

		public Region (string _name, string typename) : base()
		{
			BlastHit = null;
			RegionTypeName = typename;
			RegionName = _name;
			RegElements = new Dictionary<string, List<MiscElement>> ();
			FragPerKiloMil = new Dictionary<string, List<FKPM>> ();
		}

		public Region(string _name, string typename, int _start, int _end, Sense sense)
			:base(_start, _end, sense)
		{
			BlastHit = null;
			RegionTypeName = typename;
			RegionName = _name;
			RegElements = new Dictionary<string, List<MiscElement>> ();
			FragPerKiloMil = new Dictionary<string, List<FKPM>> ();
		}

		public Region(string _name, string typename, int _start, int _end)
			:base(_start, _end, Sense.None)
		{
			BlastHit = null;
			RegionTypeName = typename;
			RegionName = _name;
			RegElements = new Dictionary<string, List<MiscElement>> ();
			FragPerKiloMil = new Dictionary<string, List<FKPM>> ();
		}

		public virtual void AddFileElement(string ele_name, int start, int end)
		{
			if (RegElements == null) {
				RegElements = new Dictionary<string, List<MiscElement>> ();
			}
			if (!RegElements.ContainsKey (ele_name)) {
				RegElements.Add (ele_name, new List<MiscElement> ());
			}

			RegElements [ele_name].Add (new MiscElement (ele_name, start, end, this.Sense));
		}

		protected void AddMiscEle(MiscElement ele)
		{
			if (RegElements == null) {
				RegElements = new Dictionary<string, List<MiscElement>> ();
			}
			if (!RegElements.ContainsKey (ele.Component)) {
				RegElements.Add (ele.Component, new List<MiscElement> ());
			}
			RegElements [ele.Component].Add (ele);
		}

		public void SendFKPM(FKPM fkp)
		{
			if (!FragPerKiloMil.ContainsKey (fkp.Sample.ID)) {
				FragPerKiloMil.Add (fkp.Sample.ID, new List<FKPM> ());
			}

			FragPerKiloMil [fkp.Sample.ID].Add (fkp);
			MainData.LoadedFKPM++;
		}

		public bool TryTurnInsideOut(out Region toAssign)
		{
			toAssign = null;
			bool found = false;
			MiscElement largest = null;

			foreach (KeyValuePair<string, List<MiscElement>> kvp in RegElements) {
				foreach (MiscElement e in kvp.Value) {
					if (e.Length > this.Length) {
						largest = e;
						found = true;
					}
				}
			}

			if (found) {
				toAssign = new Region (RegionName, largest.Component, largest.Start, largest.End, this.Sense);
				MiscElement rtoe = new MiscElement (this.RegionTypeName, Start, End, Sense);
				toAssign.AddMiscEle (rtoe);

				RegElements [largest.Component].Remove (largest);

				foreach (KeyValuePair<string, List<MiscElement>> kvp in RegElements) {
					foreach (MiscElement e in kvp.Value) {
						toAssign.AddMiscEle (e);
					}
				}
			}
			return found;
		}

		public virtual void GetElementLengths(Dictionary<string, List<double>> lens)
		{
			foreach (KeyValuePair<string, List<MiscElement>> kvp in RegElements) {

				foreach (MiscElement msc in kvp.Value) {
					if (lens.ContainsKey (kvp.Key)) {
						lens [kvp.Key].Add (msc.Length);
					}
				}
			}
			if(lens.ContainsKey(this.RegionTypeName))
			{
				lens [this.RegionTypeName].Add (this.Length);
			}
		}

		public virtual void SendRead(SeqRead seq)
		{
			AddRead (seq);

			if (RegElements != null) {
				foreach (KeyValuePair<string, List<MiscElement>> kvp in RegElements) {
					foreach (MiscElement ge in kvp.Value) {
						if (ge.HasOverLap(seq)) {
							ge.AddRead (seq);
						}
					}
				}
			}
		}

		public virtual void SendVariant(Variant vri)
		{
			AddVariant (vri);

			if (RegElements != null) {
				foreach (KeyValuePair<string, List<MiscElement>> kvp in RegElements) {
					foreach (MiscElement ge in kvp.Value) {
						if (ge.Contains(vri)) {
							ge.AddVariant(vri);
						}
					}
				}
			}
		}

		public virtual void RecieveFilterInstruction(ElementFilterInstruction ele)
		{
			foreach (KeyValuePair<string, List<MiscElement>> kvp in RegElements) {
				if (kvp.Key == ele.ElementType) {
					TrimWrongSizedElements (kvp.Value, ele.MinLength, ele.MaxLength);
					break;
				}
			}
		}

		protected void TrimWrongSizedElements(List<GeneElement> glst, int min, int max)
		{
			List<GeneElement> toRemove = new List<GeneElement> ();
			foreach (GeneElement ge in glst) {
				if (ge.Length < min || ge.Length > max) {
					toRemove.Add (ge);
				}
			}
			foreach (GeneElement ge in toRemove) {
				glst.Remove (ge);
			}
		}

		protected void TrimWrongSizedElements(List<MiscElement> glst, int min, int max)
		{
			List<MiscElement> toRemove = new List<MiscElement> ();
			foreach (MiscElement ge in glst) {
				if (ge.Length < min || ge.Length > max) {
					toRemove.Add (ge);
				}
			}
			foreach (MiscElement ge in toRemove) {
				glst.Remove (ge);
			}
		}

		public virtual void GetElementCoverages(Dictionary<string, List<double>> covs)
		{
			foreach (KeyValuePair<string, List<double>> kvp in covs) {
				if (RegElements.ContainsKey (kvp.Key)) {
					foreach (MiscElement msc in RegElements[kvp.Key]) {
						kvp.Value.Add (msc.GetCoverageAll());
					}
				}
			}

			if (covs.ContainsKey (this.RegionTypeName)) {
				covs [this.RegionTypeName].Add (this.GetCoverageAll ());
			}
		}

		public virtual void GetElementSpatialCoverages(Dictionary<string, List<List<double>>> spats, int division)
		{
			List<double> receive;

			foreach (KeyValuePair<string, List<List<double>>> kvp in spats) {
				foreach (KeyValuePair<string, List<MiscElement>> ges in RegElements) {
					if (kvp.Key == ges.Key) {
						foreach (MiscElement ge in ges.Value) {
							if (ge.TryGetCoverageByDivision (division, out receive)) {
								MergeToEach (kvp.Value, receive);
							}
						}
					}
				}
			}
			if (spats.ContainsKey (this.RegionTypeName)) {
				if (this.TryGetCoverageByDivision (division, out receive)) {
					MergeToEach (spats [this.RegionTypeName], receive);
				}
			}
		}

		public virtual void GetElementSpatialDepthCoverages(Dictionary<string, List<List<double>>> spats, int division)
		{
			List<double> receive;

			foreach (KeyValuePair<string, List<List<double>>> kvp in spats) {
				foreach (KeyValuePair<string, List<MiscElement>> ges in RegElements) {
					if (kvp.Key == ges.Key) {
						foreach (MiscElement ge in ges.Value) {
							if (ge.TryGetCoverageDepthByDivision (division, out receive)) {
								MergeToEach (kvp.Value, receive);
							}
						}
					}
				}
			}
			if (spats.ContainsKey (this.RegionTypeName)) {
				if (this.TryGetCoverageDepthByDivision (division, out receive)) {
					MergeToEach (spats [this.RegionTypeName], receive);
				}
			}
		}

		protected void MergeToEach<T>(List<List<T>> l1, List<T> l2)
		{
			int cnt = l2.Count;

			for (int i = 0; i < cnt; i++) {
				l1 [i].Add (l2 [i]);
			}
		}

		protected double GetMeanFKPM()
		{
			double cumu = 0;
			double div = 0;

			foreach (KeyValuePair<string, List<FKPM>> kvp in FragPerKiloMil) {
				foreach (FKPM fk in kvp.Value) {
					div++;
					cumu += fk.FKPMCount;
				}
			}

			return cumu / div;
		}

		protected double GetVarianceFKPM()
		{
			List<double> nzero = new List<double> ();

			foreach (KeyValuePair<string, List<FKPM>> kvp in FragPerKiloMil) {
				foreach (FKPM fk in kvp.Value) {
					if (fk.HI != 0 || fk.LO != 0 || fk.FKPMCount == 0) {
						nzero.Add (fk.HI);
						nzero.Add (fk.LO);
					}
					nzero.Add (fk.FKPMCount);
				}
			}

			if (nzero.Count != 0) {
				return Statistics.Variance (nzero);
			} else {
				return 0;
			}
		}

		protected double GetMeanFKPM(string samp, bool norm, bool useSamp)
		{
			double cumu = 0;
			double div = 0;

			if (!norm) {
				if (useSamp) {
					foreach (KeyValuePair<string, List<FKPM>> kvp in FragPerKiloMil) {
						if (kvp.Key == samp) {
							foreach (FKPM fk in kvp.Value) {
								div++;
								cumu += fk.FKPMCount;
							}
						}
					}

					return cumu / div;
				} else {
					return GetMeanFKPM ();
				}
			} else {
				List<double> allSam = new List<double> ();
				double mainVal;
				if (useSamp) {
					mainVal = GetMeanFKPM (samp, false, useSamp);
				} else {
					mainVal = GetMeanFKPM ();
				}

				foreach (KeyValuePair<string, List<FKPM>> kvp in FragPerKiloMil) {
					foreach (FKPM fk in kvp.Value) {
						allSam.Add(fk.FKPMCount);
						if (fk.HI != 0 || fk.LO != 0 || fk.FKPMCount == 0) {
							allSam.Add (fk.HI);
							allSam.Add (fk.LO);
						}
					}
				}

				double min = 999999999; double max = 0;

				foreach (double d in allSam) {
					min = Math.Min (d, min);
					max = Math.Max (d, max);
				}

				if (max - min != 0) {
					return (mainVal - min) / (max - min);
				} else {
					if (mainVal == 0) {
						return 0;
					} else {
						return 0.5;
					}
				}
			}
		}

		protected double GetVarianceFKPM(string samp, bool useSamp)
		{
			if (useSamp) {
				List<double> nzero = new List<double> ();

				foreach (KeyValuePair<string, List<FKPM>> kvp in FragPerKiloMil) {
					if (samp == kvp.Key) {
						foreach (FKPM fk in kvp.Value) {
							if (fk.HI != 0 || fk.LO != 0 || fk.FKPMCount == 0) {
								nzero.Add (fk.HI);
								nzero.Add (fk.LO);
							}
							nzero.Add (fk.FKPMCount);
						}
					}
				}

				if (nzero.Count != 0) {
					return Statistics.StandardDeviation (nzero) / GetMeanFKPM(samp, false, useSamp);
				} else {
					return 0;
				}
			} else
				return GetVarianceFKPM ();
		}

		public virtual void GetElementDepthCoverages(Dictionary<string, List<double>> covs)
		{
			foreach (KeyValuePair<string, List<double>> kvp in covs) {
				if (RegElements.ContainsKey (kvp.Key)) {
					foreach (MiscElement msc in RegElements[kvp.Key]) {
						kvp.Value.Add (msc.GetCoverageDepthScoreAll());
					}
				}
			}

			if (covs.ContainsKey (this.RegionTypeName)) {
				covs [this.RegionTypeName].Add (this.GetCoverageDepthScoreAll());
			}
		}

		//RSP RANK METHODS

		public virtual void RSPElementMethlyCoverage(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.RankMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetCoverageAll ());
				} else if (RankSplit.RankMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetCoverageSubSet (RankSplit.RankStartDiv, RankSplit.RankEndDiv));
				} else {
					tdict.Add (this.RegionName, this.GetCoverageBaseLimit (RankSplit.RankBaseCount, RankSplit.RankFromStart));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.RankMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetCoverageAll ());
					} else if (RankSplit.RankMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetCoverageSubSet (RankSplit.RankStartDiv, RankSplit.RankEndDiv));
					} else {
						tdict.Add (toAdd, gele.GetCoverageBaseLimit (RankSplit.RankBaseCount, RankSplit.RankFromStart));
					}
				}
			}
		}

		public virtual void RSPElementMethlyDepthCoverage(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.RankMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetCoverageDepthScoreAll());
				} else if (RankSplit.RankMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetCoverageDepthSubSet (RankSplit.RankStartDiv, RankSplit.RankEndDiv));
				} else {
					tdict.Add (this.RegionName, this.GetCoverageDepthBaseLimit (RankSplit.RankBaseCount, RankSplit.RankFromStart));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.RankMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetCoverageDepthScoreAll ());
					} else if (RankSplit.RankMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetCoverageDepthSubSet (RankSplit.RankStartDiv, RankSplit.RankEndDiv));
					} else {
						tdict.Add (toAdd, gele.GetCoverageDepthBaseLimit (RankSplit.RankBaseCount, RankSplit.RankFromStart));
					}
				}
			}
		}

		public virtual void RSPExpr(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				double mean = this.GetMeanFKPM (RankSplit.SampleToUse, false,  RankSplit.UseSample);
				if (!double.IsNaN (mean)) {
					tdict.Add (this.RegionName, mean);
				}
			}
			else if (RegElements.ContainsKey (id)) {
				double mean = this.GetMeanFKPM (RankSplit.SampleToUse, false,  RankSplit.UseSample);
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (!double.IsNaN (mean)) {
						tdict.Add (toAdd, mean);
					}
				}
			}
		}

		public virtual void RSPExprRange(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				double vari = this.GetVarianceFKPM (RankSplit.SampleToUse, RankSplit.UseSample);
				if (!double.IsNaN (vari)) {
					tdict.Add (this.RegionName, vari);
				}
			}
			else if (RegElements.ContainsKey (id)) {
				double vari = this.GetVarianceFKPM (RankSplit.SampleToUse, RankSplit.UseSample);
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (!double.IsNaN (vari)) {
						tdict.Add (toAdd, vari);
					}
				}
			}
		}

		public virtual void RSPExprNorm(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				double mean = this.GetMeanFKPM (RankSplit.SampleToUse, true,  RankSplit.UseSample);
				if (!double.IsNaN (mean)) {
					tdict.Add (this.RegionName, mean);
				}
			}
			else if (RegElements.ContainsKey (id)) {
				double mean = this.GetMeanFKPM (RankSplit.SampleToUse, true,  RankSplit.UseSample);
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (!double.IsNaN (mean)) {
						tdict.Add (toAdd, mean);
					}
				}
			}
		}

		public virtual void RSPBlastEVal(string id, Dictionary<string, double> tdict)
		{
			if (this.BlastHit != null) {
				if (this.RegionTypeName == id) {
					tdict.Add (this.RegionName, this.BlastHit.EValue);
				}
				else if (RegElements.ContainsKey (id)) {
					foreach (MiscElement gele in RegElements[id]) {
						string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
						while (tdict.ContainsKey (toAdd)) {
							toAdd = toAdd + "i";
						}

						tdict.Add (toAdd, this.BlastHit.EValue);
					}
				}
			}
		}

		public virtual void RSPBlastPercID(string id, Dictionary<string, double> tdict)
		{
			if (this.BlastHit != null) {
				if (this.RegionTypeName == id) {
					tdict.Add (this.RegionName, this.BlastHit.PercIdentity);
				}
				else if (RegElements.ContainsKey (id)) {
					foreach (MiscElement gele in RegElements[id]) {
						string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
						while (tdict.ContainsKey (toAdd)) {
							toAdd = toAdd + "i";
						}

						tdict.Add (toAdd, this.BlastHit.PercIdentity);
					}
				}
			}
		}

		public virtual void RSPVarCount(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.RankMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetVarCount(AppSettings.Statistic.NORMALISE_VARIANTS.Item));
				} else if (RankSplit.RankMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetVarCountSubset (RankSplit.RankStartDiv, RankSplit.RankEndDiv
						, AppSettings.Statistic.NORMALISE_VARIANTS.Item));
				} else {
					tdict.Add (this.RegionName, this.GetVarCountBaseLimit (RankSplit.RankBaseCount, RankSplit.RankFromStart
						, AppSettings.Statistic.NORMALISE_VARIANTS.Item));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.RankMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetVarCount (AppSettings.Statistic.NORMALISE_VARIANTS.Item));
					} else if (RankSplit.RankMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetVarCountSubset (RankSplit.RankStartDiv, RankSplit.RankEndDiv
							, AppSettings.Statistic.NORMALISE_VARIANTS.Item));
					} else {
						tdict.Add (toAdd, gele.GetVarCountBaseLimit (RankSplit.RankBaseCount, RankSplit.RankFromStart
							, AppSettings.Statistic.NORMALISE_VARIANTS.Item));
					}
				}
			}
		}

		public virtual void RSPVarAllHom(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.RankMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetAllHomCount(AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
				} else if (RankSplit.RankMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetAllHomCountSubset (RankSplit.RankStartDiv, RankSplit.RankEndDiv,
						AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
				} else {
					tdict.Add (this.RegionName, this.GetAllHomCountBaseLimit (RankSplit.RankBaseCount, RankSplit.RankFromStart,
						AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.RankMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetAllHomCount (AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
					} else if (RankSplit.RankMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetAllHomCountSubset (RankSplit.RankStartDiv, RankSplit.RankEndDiv,
							AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
					} else {
						tdict.Add (toAdd, gele.GetAllHomCountBaseLimit (RankSplit.RankBaseCount, RankSplit.RankFromStart,
							AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
					}
				}
			}
		}

		public virtual void RSPVarAllHet(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.RankMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetAllHetCount(AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
				} else if (RankSplit.RankMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetAllHetCountSubset (RankSplit.RankStartDiv, RankSplit.RankEndDiv,
						AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
				} else {
					tdict.Add (this.RegionName, this.GetAllHetCountBaseLimit (RankSplit.RankBaseCount, RankSplit.RankFromStart,
						AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.RankMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetAllHetCount (AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
					} else if (RankSplit.RankMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetAllHetCountSubset (RankSplit.RankStartDiv, RankSplit.RankEndDiv,
							AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
					} else {
						tdict.Add (toAdd, gele.GetAllHetCountBaseLimit (RankSplit.RankBaseCount, RankSplit.RankFromStart,
							AppSettings.Statistic.NORMALISE_VARIANTS.Item, RankSplit.SampleToUse, RankSplit.UseSample));
					}
				}
			}
		}

		//RSP OUTPUT METHODS

		public virtual void RSPOUTQuantMethyCov(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetCoverageAll ());
				} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetCoverageSubSet (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv));
				} else {
					tdict.Add (this.RegionName, this.GetCoverageBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetCoverageAll ());
					} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetCoverageSubSet (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv));
					} else {
						tdict.Add (toAdd, gele.GetCoverageBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
					}
				}
			}
		}

		public virtual void RSPOUTQuantMethyDepthCov(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetCoverageDepthScoreAll());
				} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetCoverageDepthSubSet (RankSplit.OutputStartDiv, RankSplit.RankEndDiv));
				} else {
					tdict.Add (this.RegionName, this.GetCoverageDepthBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetCoverageDepthScoreAll ());
					} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetCoverageDepthSubSet (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv));
					} else {
						tdict.Add (toAdd, gele.GetCoverageDepthBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
					}
				}
			}
		}

		public virtual void RSPOUTQuantVar(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetVarCount(AppSettings.Statistic.NORMALISE_VARIANTS.Item));
				} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetVarCountSubset (RankSplit.OutputStartDiv, RankSplit.RankEndDiv,
						AppSettings.Statistic.NORMALISE_VARIANTS.Item));
				} else {
					tdict.Add (this.RegionName, this.GetVarCountBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart,
						AppSettings.Statistic.NORMALISE_VARIANTS.Item));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetVarCount (AppSettings.Statistic.NORMALISE_VARIANTS.Item));
					} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetVarCountSubset (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv,
							AppSettings.Statistic.NORMALISE_VARIANTS.Item));
					} else {
						tdict.Add (toAdd, gele.GetVarCountBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart,
							AppSettings.Statistic.NORMALISE_VARIANTS.Item));
					}
				}
			}
		}

		public virtual void RSPOUTBaseSeq(string scaff, string id, Dictionary<string, string> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetBaseSeqAll(scaff));
				} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetBaseSeqSubset (scaff, RankSplit.OutputStartDiv, RankSplit.OutputEndDiv));
				} else {
					tdict.Add (this.RegionName, this.GetBaseSeqBaseLimit (scaff, RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetBaseSeqAll (scaff));
					} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetBaseSeqSubset (scaff, RankSplit.OutputStartDiv, RankSplit.OutputEndDiv));
					} else {
						tdict.Add (toAdd, gele.GetBaseSeqBaseLimit (scaff, RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
					}
				}
			}
		}

		public virtual void RSPOUTBlastHitID(string id, Dictionary<string, string> tdict)
		{
			if (this.BlastHit != null) {
				if (this.RegionTypeName == id) {
					tdict.Add (this.RegionName, this.BlastHit.HitID);
				}
				else if (RegElements.ContainsKey (id)) {
					foreach (MiscElement gele in RegElements[id]) {
						string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
						while (tdict.ContainsKey (toAdd)) {
							toAdd = toAdd + "i";
						}

						tdict.Add (toAdd, this.BlastHit.HitID);
					}
				}
			}
		}

		public virtual void RSPOUTMethlyCoverage(string id, Dictionary<string, List<double>> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
					List<double> res;
					if (this.TryGetCoverageByDivision (20, out res)) {
						tdict.Add (this.RegionName, res);
					}
				} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
					List<double> res;
					if (this.TryGetCoverageSubSet (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv, 20, out res)) {
						tdict.Add (this.RegionName, res);
					}
				} else {
					tdict.Add (this.RegionName, this.GetCoverageSpatialBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}

					if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
						List<double> res;
						if (gele.TryGetCoverageByDivision (20, out res)) {
							tdict.Add (toAdd, res);
						}
					} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
						List<double> res;
						if (gele.TryGetCoverageSubSet (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv, 20, out res)) {
							tdict.Add (toAdd, res);
						}
					} else {
						tdict.Add (toAdd, gele.GetCoverageSpatialBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
					}
				}
			}
		}

		public virtual void RSPOUTMethlyDepthCoverage(string id, Dictionary<string, List<double>> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
					List<double> res;
					if (this.TryGetCoverageDepthByDivision (20, out res)) {
						tdict.Add (this.RegionName, res);
					}
				} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
					List<double> res;
					if (this.TryGetCoverageDepthSubSet (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv, 20, out res)) {
						tdict.Add (this.RegionName, res);
					}
				} else {
					tdict.Add (this.RegionName, this.GetCoverageDepthSpatialBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}

					if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
						List<double> res;
						if (gele.TryGetCoverageDepthByDivision (20, out res)) {
							tdict.Add (toAdd, res);
						}
					} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
						List<double> res;
						if (gele.TryGetCoverageDepthSubSet (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv, 20, out res)) {
							tdict.Add (toAdd, res);
						}
					} else {
						tdict.Add (toAdd, gele.GetCoverageDepthSpatialBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
					}
				}
			}
		}

		public virtual void RSPOUTElementMethlyDepthCoverage(string id, Dictionary<string, double> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetCoverageDepthScoreAll());
				} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetCoverageDepthSubSet (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv));
				} else {
					tdict.Add (this.RegionName, this.GetCoverageDepthBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetCoverageDepthScoreAll ());
					} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetCoverageDepthSubSet (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv));
					} else {
						tdict.Add (toAdd, gele.GetCoverageDepthBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
					}
				}
			}
		}

		public virtual void RSPOUTReadNames(string id, Dictionary<string, string> tdict)
		{
			if (this.RegionTypeName == id) {
				if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
					tdict.Add (this.RegionName, this.GetReadNamesAll());
				} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
					tdict.Add (this.RegionName, this.GetReadNamesSubset (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv));
				} else {
					tdict.Add (this.RegionName, this.GetReadNamesBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
				}
			}
			else if (RegElements.ContainsKey (id)) {
				foreach (MiscElement gele in RegElements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.None) {
						tdict.Add (toAdd, gele.GetReadNamesAll ());
					} else if (RankSplit.OutputMethodSubSequence == SubSequenceDivision.Normalised) {
						tdict.Add (toAdd, gele.GetReadNamesSubset (RankSplit.OutputStartDiv, RankSplit.OutputEndDiv));
					} else {
						tdict.Add (toAdd, gele.GetReadNamesBaseLimit (RankSplit.OutputBaseCount, RankSplit.OutputFromStart));
					}
				}
			}
		}
	}
}

