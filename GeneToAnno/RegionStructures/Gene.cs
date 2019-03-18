using System;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class Gene : Region
	{
		public Dictionary<string, List<GeneElement>> Elements { get; set; }

		public Gene (string _name, string typename) : base(_name, typename)
		{
			InitEleDict();
		}
		public Gene(string _name, string typename, int _start, int _end, Sense sense)
			:base(_name, typename,_start, _end, sense)
		{
			InitEleDict();
		}

		private void InitEleDict()
		{
			Elements = new Dictionary<string, List<GeneElement>> ();
		}

		public void RunGenerateOptions(Scaffold scaff)
		{
			foreach (KeyValuePair<string, List<GeneElement>> kvp in Elements) {
				kvp.Value.Sort ();
			}

			RemoveDuplicates ();

			int len = scaff.Length;

			if (AppSettings.Genes.GENERATE_ANY_PROMO.Item) {
				int rollingStartPoint = 0;

				if (AppSettings.Genes.GENERATE_PROMO_1.Item) {
					GeneElement tempEle;
					if (GenerateSideElement (
						    "promoter",
						    true,
						    AppSettings.Genes.PROMO_1_SIZE.Item,
						    0,
						    len,
						    out tempEle)) {
						if (!Elements.ContainsKey (tempEle.ToString ())) {
							Elements.Add (tempEle.ToString (), new List<GeneElement> ());
						}
						Elements [tempEle.ToString ()].Add (tempEle);
					}
					rollingStartPoint += AppSettings.Genes.PROMO_1_SIZE.Item;
				}
				if (AppSettings.Genes.GENERATE_PROMO_2.Item) {
					GeneElement tempEle;
					if (GenerateSideElement (
						    "promoter",
						    true,
						    AppSettings.Genes.PROMO_2_SIZE.Item,
						    rollingStartPoint,
						    len,
						    out tempEle)) {
						if (!Elements.ContainsKey (tempEle.ToString ())) {
							Elements.Add (tempEle.ToString (), new List<GeneElement> ());
						}
						Elements [tempEle.ToString ()].Add (tempEle);
					}
					rollingStartPoint += AppSettings.Genes.PROMO_2_SIZE.Item;
				}
				if (AppSettings.Genes.GENERATE_PROMO_3.Item) {
					GeneElement tempEle;
					if (GenerateSideElement (
						    "promoter",
						    true,
						    AppSettings.Genes.PROMO_3_SIZE.Item,
						    rollingStartPoint,
						    len,
						    out tempEle)) {
						if (!Elements.ContainsKey (tempEle.ToString ())) {
							Elements.Add (tempEle.ToString (), new List<GeneElement> ());
						}
						Elements [tempEle.ToString ()].Add (tempEle);
					}
				}
			}
			if (AppSettings.Genes.GENERATE_FLANK3.Item) {
				GeneElement tempEle;
				if (GenerateSideElement (
					    "flank3",
					    false,
					    AppSettings.Genes.FLANK3_SIZE.Item,
					    0,
					    len,
					    out tempEle)) {
					if (!Elements.ContainsKey (tempEle.ToString ())) {
						Elements.Add (tempEle.ToString (), new List<GeneElement> ());
					}
					Elements [tempEle.ToString ()].Add (tempEle);
				}
			}

			if (AppSettings.Genes.GENERATE_INTRONS.Item && !Elements.ContainsKey("intron")) {
				GenerateIntrons ();
			}
		}

		protected void GenerateIntrons()
		{
			if (!Elements.ContainsKey ("intron")) {
				Elements.Add ("intron", new List<GeneElement> ());
			}
			List<GeneElement> l = Elements ["exon"];
			int exCnt = l.Count;

			if (exCnt > 1) {
				for (int i = 1; i < exCnt; i++) {
					int newSt, newEd;
					if (this.Sense == Sense.Sense) {
						newSt = l [i - 1].End + 1;
						newEd = l [i].Start - 1;
					} else {
						newSt = l [i].End - 1;
						newEd = l [i - 1].Start - 1;
					}
					Elements ["intron"].Add (new Intron (i, newSt, newEd, this.Sense));
				}
			}
		}

		protected bool GenerateSideElement(string comp, bool from5Prime, int size, int buff, int scaffLen, out GeneElement result)
		{
			result = null;

			if (this.Sense == Sense.Sense) {
				if (from5Prime) {
					if (this.Start - buff - size > 0) {
						result = MakeSideEle (comp, buff, this.Start - buff - size, this.Start - buff);
					}
				} else {
					if (this.End + buff + size < scaffLen) {
						result = MakeSideEle (comp, buff, this.End + buff, this.End + buff + size);
					}
				}
			}
			else if (this.Sense != Sense.Sense) {
				if (from5Prime) {
					if (this.End + buff + size < scaffLen) {
						result = MakeSideEle (comp, buff, this.End + buff, this.End + buff + size);
					}
				} else {
					if (this.Start - buff - size < 0) {
						result = MakeSideEle (comp, buff, this.Start - buff - size, this.Start - buff);
					}
				}
			}
			if (result == null) {
				return false;
			} else
				return true;
		}

		protected GeneElement MakeSideEle(string comp, int buff, int st, int ed)
		{
			GeneElement ele = null;

			if (comp == "promoter") {
				ele = new Promoter (buff, st, ed, this.Sense);
			} else if (comp == "flank3") {
				ele = new Flank3 (st, ed, this.Sense);
			}

			return ele;
		}

		public override void AddFileElement(string ele_name, int start, int end)
		{
			if (end - start > 0) {
				GeneElement ele;
				int cnt = 0;
				switch (ele_name) {
				case "five_prime_UTR":
					ele = new UTR5 (start, end, this.Sense);
					if (!Elements.ContainsKey ("five_prime_UTR")) {
						Elements.Add ("five_prime_UTR", new List<GeneElement> ());
					}
					Elements ["five_prime_UTR"].Add (ele);
					break;
				case "three_prime_UTR":
					ele = new UTR3 (start, end, this.Sense);
					if (!Elements.ContainsKey ("three_prime_UTR")) {
						Elements.Add ("three_prime_UTR", new List<GeneElement> ());
					}
					Elements ["three_prime_UTR"].Add (ele);
					break;
				case "exon":
					if (!Elements.ContainsKey ("exon")) {
						Elements.Add ("exon", new List<GeneElement> ());
					}
					cnt = Elements ["exon"].Count + 1;
					ele = new Exon (cnt, start, end, this.Sense);
					Elements ["exon"].Add (ele);
					break;
				case "CDS":
					if (!Elements.ContainsKey ("CDS")) {
						Elements.Add ("CDS", new List<GeneElement> ());
					}
					cnt = Elements ["CDS"].Count + 1;
					ele = new CDS (cnt, start, end, this.Sense);
					Elements ["CDS"].Add (ele);
					break;
				default:
					if (ele_name.Contains ("promoter")) {
						ele = new Promoter (ele_name, start, end, this.Sense);
						if (!Elements.ContainsKey (ele_name)) {
							Elements.Add (ele_name, new List<GeneElement> ());
						}
						Elements [ele_name].Add (ele);
					} else {
						if (!Elements.ContainsKey (ele_name)) {
							Elements.Add (ele_name, new List<GeneElement> ());
						}
						ele = new MiscElement (ele_name, start, end, this.Sense);
						Elements [ele_name].Add (ele);
					}
					break;
				}
			}
		}

		public override void GetElementLengths(Dictionary<string, List<double>> lens)
		{
			foreach (KeyValuePair<string, List<GeneElement>> kvp in Elements) {
				if (kvp.Value.Count > 0) {
					if (kvp.Key == "three_prime_UTR") {
						if (lens.ContainsKey ("three_prime_UTR")) {
							lens ["three_prime_UTR"].Add (GetMasterLength (kvp.Value));
						}
					} else if (kvp.Key == "five_prime_UTR") {
						if (lens.ContainsKey ("five_prime_UTR")) {
							lens ["five_prime_UTR"].Add (GetMasterLength (kvp.Value));
						}
					} else {
						foreach (GeneElement gen in kvp.Value) {
							if (lens.ContainsKey (gen.ToString ())) {
								lens [gen.ToString ()].Add (gen.Length);
							}
						}
					}
				}
			}
			if (lens.ContainsKey (this.RegionTypeName)) {
				lens [this.RegionTypeName].Add (this.Length);
			}
		}

		protected double GetMasterLength(List<GeneElement> ld)
		{
			double len = 0;
			foreach (GeneElement ge in ld) {
				len += ge.Length;
			}
			return len;
		}

		protected void RemoveDuplicates()
		{
			List<GeneElement> toRemove = new List<GeneElement> ();

			foreach (KeyValuePair<string, List<GeneElement>> kvp in Elements) {
				toRemove.Clear ();

				int oldEnd = 0;
				int cnt = 0;
				foreach (GeneElement ge in kvp.Value) {
					if (oldEnd == ge.End) {
						if (cnt > 0) {
							toRemove.Add (ge);
						}
					}
					cnt++;
					oldEnd = ge.End;
				}
				foreach (GeneElement gen in toRemove) {
					kvp.Value.Remove (gen);
				}
			}
		}

		public override void SendRead(SeqRead seq)
		{
			foreach (KeyValuePair<string, List<GeneElement>> kvp in Elements) {
				foreach (GeneElement ge in kvp.Value) {
					if (ge.HasOverLap(seq)) {
						ge.AddRead (seq);
					}
				}
			}
			base.SendRead (seq);
		}

		public override void SendVariant(Variant vri)
		{
			foreach (KeyValuePair<string, List<GeneElement>> kvp in Elements) {
				foreach (GeneElement ge in kvp.Value) {
					if (ge.Contains(vri)) {
						ge.AddVariant (vri);
					}
				}
			}
			base.SendVariant(vri);
		}

		public override void RecieveFilterInstruction(ElementFilterInstruction ele)
		{
			foreach (KeyValuePair<string, List<GeneElement>> kvp in Elements) {
				if (kvp.Key == ele.ElementType) {
					switch (kvp.Key) {
					case "three_prime_UTR":
						TrimCompoundedWrongSizedElement (kvp.Value, ele.MinLength, ele.MaxLength);
						break;
					case "five_prime_UTR":
						TrimCompoundedWrongSizedElement (kvp.Value, ele.MinLength, ele.MaxLength);
						break;
					default:
						TrimWrongSizedElements (kvp.Value, ele.MinLength, ele.MaxLength);
						break;
					}
					break;
				}
			}
		}

		protected void TrimCompoundedWrongSizedElement(List<GeneElement> glst, int min, int max)
		{
			double masterLen = GetMasterLength (glst);

			if (masterLen < min || masterLen > max) {
				glst.Clear ();
			}
		}

		public override void GetElementCoverages(Dictionary<string, List<double>> covs)
		{
			foreach (KeyValuePair<string, List<double>> kvp in covs) {
				foreach(KeyValuePair<string, List<GeneElement>> ges in Elements) {
					if (ges.Value.Count != 0) {
						if (kvp.Key == ges.Key) {
							foreach (GeneElement ge in ges.Value) {
								kvp.Value.Add (ge.GetCoverageAll ());
							}
						}
					}
				}
			}

			if (covs.ContainsKey (this.RegionTypeName)) {
				covs [this.RegionTypeName].Add (this.GetCoverageAll ());
			}
		}

		public override void GetElementSpatialCoverages(Dictionary<string, List<List<double>>> spats, int division)
		{
			List<double> receive;

			foreach (KeyValuePair<string, List<List<double>>> kvp in spats) {
				foreach (KeyValuePair<string, List<GeneElement>> ges in Elements) {
					if (ges.Value.Count != 0) {
						if (kvp.Key == ges.Key) {
							foreach (GeneElement ge in ges.Value) {
								if (ge.TryGetCoverageByDivision (division, out receive)) {
									MergeToEach (kvp.Value, receive);
								}
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

		public override void GetElementSpatialDepthCoverages(Dictionary<string, List<List<double>>> spats, int division)
		{
			List<double> receive;

			foreach (KeyValuePair<string, List<List<double>>> kvp in spats) {
				foreach (KeyValuePair<string, List<GeneElement>> ges in Elements) {
					if (ges.Value.Count != 0) {
						if (kvp.Key == ges.Key) {
							foreach (GeneElement ge in ges.Value) {
								if (ge.TryGetCoverageDepthByDivision (division, out receive)) {
									MergeToEach (kvp.Value, receive);
								}
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

		public override void GetElementDepthCoverages(Dictionary<string, List<double>> covs)
		{
			foreach (KeyValuePair<string, List<double>> kvp in covs) {
				foreach(KeyValuePair<string, List<GeneElement>> ges in Elements) {
					if (ges.Value.Count != 0) {
						if (kvp.Key == ges.Key) {
							foreach (GeneElement ge in ges.Value) {
								double score = ge.GetCoverageDepthScoreAll ();
								if (score > 0) {
									kvp.Value.Add (score);
								}
							}
						}
					}
				}
			}

			if (covs.ContainsKey (this.RegionTypeName)) {
				double score = this.GetCoverageDepthScoreAll();
				if (score > 0) {
					covs [this.RegionTypeName].Add (score);
				}
			}
		}

		//RSP RANK METHODS

		public override void RSPElementMethlyCoverage(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPElementMethlyCoverage (id, tdict);
			}
		}

		public override void RSPElementMethlyDepthCoverage(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPElementMethlyDepthCoverage (id, tdict);
			}
		}

		public override void RSPExpr(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				double mean = this.GetMeanFKPM (RankSplit.SampleToUse, false, RankSplit.UseSample);
				foreach (GeneElement gele in Elements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (!double.IsNaN (mean)) {
						tdict.Add (toAdd, mean);
					}
				}
			} else {
				base.RSPExpr (id, tdict);
			}
		}

		public override void RSPExprRange(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				double vari = this.GetVarianceFKPM (RankSplit.SampleToUse, RankSplit.UseSample);
				foreach (GeneElement gele in Elements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (!double.IsNaN (vari)) {
						tdict.Add (toAdd, vari);
					}
				}
			} else {
				base.RSPExprRange (id, tdict);
			}
		}

		public override void RSPExprNorm(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				double mean = this.GetMeanFKPM (RankSplit.SampleToUse, true, RankSplit.UseSample);
				foreach (GeneElement gele in Elements[id]) {
					string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
					while (tdict.ContainsKey (toAdd)) {
						toAdd = toAdd + "i";
					}
					if (!double.IsNaN (mean)) {
						tdict.Add (toAdd, mean);
					}
				}
			} else {
				base.RSPExprNorm (id, tdict);
			}
		}

		public override void RSPBlastEVal(string id, Dictionary<string, double> tdict)
		{
			if (this.BlastHit != null) {
				if (Elements.ContainsKey (id)) {
					foreach (GeneElement gele in Elements[id]) {
						string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
						while (tdict.ContainsKey (toAdd)) {
							toAdd = toAdd + "i";
						}

						tdict.Add (toAdd, this.BlastHit.EValue);
					}
				} else {
					base.RSPBlastEVal (id, tdict);
				}
			}
		}

		public override void RSPBlastPercID(string id, Dictionary<string, double> tdict)
		{
			if (this.BlastHit != null) {
				if (Elements.ContainsKey (id)) {
					foreach (GeneElement gele in Elements[id]) {
						string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
						while (tdict.ContainsKey (toAdd)) {
							toAdd = toAdd + "i";
						}

						tdict.Add (toAdd, this.BlastHit.PercIdentity);
					}
				} else {
					base.RSPBlastPercID (id, tdict);
				}
			}	
		}

		public override void RSPVarCount(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPVarCount (id, tdict);
			}
		}

		public override void RSPVarAllHom(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPVarAllHom (id, tdict);
			}
		}

		public override void RSPVarAllHet(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPVarAllHet (id, tdict);
			}
		}

		//RSP OUTPUT METHODS

		public override void RSPOUTQuantMethyCov(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPOUTQuantMethyCov (id, tdict);
			}
		}

		public override void RSPOUTQuantMethyDepthCov(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPOUTQuantMethyDepthCov (id, tdict);
			}
		}

		public override void RSPOUTQuantVar(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPOUTQuantVar (id, tdict);
			}
		}

		public override void RSPOUTBaseSeq(string scaff, string id, Dictionary<string, string> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPOUTBaseSeq (scaff, id, tdict);
			}
		}

		public override void RSPOUTBlastHitID(string id, Dictionary<string, string> tdict)
		{
			if (this.BlastHit != null) {
				if (Elements.ContainsKey (id)) {
					foreach (GeneElement gele in Elements[id]) {
						string toAdd = this.RegionName + "@" + gele.ToStringPlusNumber ();
						while (tdict.ContainsKey (toAdd)) {
							toAdd = toAdd + "i";
						}

						tdict.Add (toAdd, this.BlastHit.HitID);
					}
				} else {
					base.RSPOUTBlastHitID (id, tdict);
				}
			}	
		}

		public override void RSPOUTMethlyCoverage(string id, Dictionary<string, List<double>> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPOUTMethlyCoverage (id, tdict);
			}
		}

		public override void RSPOUTMethlyDepthCoverage(string id, Dictionary<string, List<double>> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPOUTMethlyDepthCoverage (id, tdict);
			}
		}

		public override void RSPOUTElementMethlyDepthCoverage(string id, Dictionary<string, double> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPOUTElementMethlyDepthCoverage (id, tdict);
			}
		}

		public override void RSPOUTReadNames(string id, Dictionary<string, string> tdict)
		{
			if (Elements.ContainsKey (id)) {
				foreach (GeneElement gele in Elements[id]) {
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
			} else {
				base.RSPOUTReadNames (id, tdict);
			}
		}
	}
}

