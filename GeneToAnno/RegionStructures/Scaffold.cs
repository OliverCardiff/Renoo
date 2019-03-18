using System;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class Scaffold
	{
		public string ID { get; set;}
		public int Length { get; set;}
		public bool LiteLoad { get; set; }

		public Dictionary<string, Gene> Genes { get; set; }
		public Dictionary<string, Region> Regions { get; set; }

		protected string seq;

		protected List<SeqRead> UnattachedReads { get; set; }

		public Scaffold (string _ID, ref string _seq, bool isLite)
		{
			UnattachedReads = new List<SeqRead> ();
			LiteLoad = isLite;
			this.ID = _ID;
			if (!isLite) {
				seq = _seq;
			}
			Length = _seq.Length;
			Regions = new Dictionary<string, Region> ();
			Genes = new Dictionary<string, Gene> ();
		}

		protected void AddRegion(string id, string typename, int start, int end, Sense sense)
		{
			try{
				if(typename.Equals("gene")) {
					Gene g = new Gene (id, typename, start, end, sense);
					Genes.Add (id, g);
				} else {
					Region r = new Region(id, typename, start, end, sense);
					Regions.Add(id, r);
				}
			}
			catch(Exception e) {
				MainData.ShowMessageWindow ("Your Gff3 IDs are not consistent!\n " +
					"Change Gff3 loader options to fix them!\n Error: " + e.Message + "\n Offending id:" + id, true);
			}
		}

		public void RunGenePostLoad()
		{
			foreach (KeyValuePair<string, Gene> kvp in Genes) {
				kvp.Value.RunGenerateOptions (this);
			}
			Dictionary<string, Region>.KeyCollection kys = Regions.Keys;

			foreach (string str in kys) {
				Region r = null;
				if (Regions [str].TryTurnInsideOut (out r)) {
					Regions [str] = r;
				}
			}
		}

		public void SendComponent(string _id, int _start, int _end, Sense sn)
		{
			char[] chrs = { '#' };
			string[] strs = _id.Split (chrs, 3);
			if (Genes.ContainsKey (strs [0])) {
				Genes [strs [0]].AddFileElement (strs [1], _start, _end);
			} else if (Regions.ContainsKey (strs [0])) {
				Regions [strs [0]].AddFileElement (strs [1], _start, _end);
			} else {
				AddRegion (strs [0], strs [1], _start, _end, sn);
			}
		}

		/// <summary>
		/// Gets the base ratio for a given range within the scaffold.
		/// </summary>
		/// <returns>The base ratio.</returns>
		/// <param name="_start">Start (1-index inclusive).</param>
		/// <param name="_end">End (1-index inclusive).</param>
		public double GetBaseRatio(int _start, int _end)
		{
			if (!LiteLoad) {
				if (_start < 1)
					_start = 1;
				if (_end > Length)
					_end = Length;
				char[] bases = seq.ToCharArray (_start - 1, _end - (_start - 1)); 

				double CG = 0;
				double AT = 0;

				foreach (char pair in bases) {
					if (pair == 'A' || pair == 'T' || pair == 'a' || pair == 't') {
						AT += 1;
					} else if (pair == 'C' || pair == 'G' || pair == 'c' || pair == 'g') {
						CG += 1;
					}
				}

				if (CG + AT < (_end - _start) - 2) {
					return 0;
				} else {
					return CG / (AT + CG);
				}
			} else {
				throw new Exception ("No sequences have been loaded! Disable Lite-load from genome loader settings..");
			}
		}

		public string GetBases(int st, int ed)
		{
			if (st < 1)
				st = 1;
			if (ed > Length)
				ed = Length;

			int len = ed - st;

			return seq.Substring (st, len);
		}

		public bool TryGetBaseRatio(int _start, int _end, out double ratio)
		{
			ratio = GetBaseRatio (_start, _end);

			if (ratio <= 0 || ratio >= 1 || double.IsNaN(ratio)) {
				return false;
			} else
				return true;
		}

		public double GetBaseRatio()
		{
			return GetBaseRatio (1, Length);
		}

		/// <summary>
		/// Fragments the scaffold into a given size and returns a list of base ratios for those fragments
		/// </summary>
		/// <returns>The fragment base ratio.</returns>
		/// <param name="_maxfragmentSize">Maxfragment size.</param>
		public List<double> GetFragmentBaseRatio(int _maxfragmentSize)
		{
			double theVal;

			List<double> ratios = new List<double> ();

			if (Length < _maxfragmentSize) {
				ratios.Add (GetBaseRatio ());
				return ratios;
			} else {
				int remainingLen = Length;
				int nextIndex = 1;

				while (remainingLen > _maxfragmentSize) {
					if (TryGetBaseRatio (nextIndex, nextIndex + _maxfragmentSize - 1, out theVal)) {
						ratios.Add(theVal);
					}
					nextIndex += _maxfragmentSize;
					remainingLen -= _maxfragmentSize;
				}

				return ratios;
			}
		}

		public void GetElementLengths(Dictionary<string, List<double>> lens)
		{
			foreach (KeyValuePair<string, Gene> kvp in Genes) {
				kvp.Value.GetElementLengths (lens);
			}
			foreach (KeyValuePair<string, Region> kvp in Regions) {
				kvp.Value.GetElementLengths (lens);
			}
		}

		public void GetElementCoverages(Dictionary<string, List<double>> covs)
		{
			foreach (KeyValuePair<string, Gene> ges in Genes) {
				ges.Value.GetElementCoverages (covs);
			}
			foreach (KeyValuePair<string, Region> res in Regions) {
				res.Value.GetElementCoverages (covs);
			}
		}

		public void GetElementDepthCoverages(Dictionary<string, List<double>> covs)
		{
			foreach (KeyValuePair<string, Gene> ges in Genes) {
				ges.Value.GetElementDepthCoverages (covs);
			}
			foreach (KeyValuePair<string, Region> res in Regions) {
				res.Value.GetElementDepthCoverages (covs);
			}
		}

		public void GetElementSpatialCoverages(Dictionary<string, List<List<double>>> spats, int division)
		{
			foreach (KeyValuePair<string, Gene> ge in Genes) {
				ge.Value.GetElementSpatialCoverages (spats, division);
			}
			foreach (KeyValuePair<string, Region> re in Regions) {
				re.Value.GetElementSpatialCoverages (spats, division);
			}
		}

		public void GetElementSpatialDepthCoverages(Dictionary<string, List<List<double>>> spats, int division)
		{
			foreach (KeyValuePair<string, Gene> ge in Genes) {
				ge.Value.GetElementSpatialDepthCoverages (spats, division);
			}
			foreach (KeyValuePair<string, Region> re in Regions) {
				re.Value.GetElementSpatialDepthCoverages (spats, division);
			}
		}

		public void SendRead(SeqRead seq)
		{
			foreach (KeyValuePair<string, Gene> kvp in Genes) {
				if (kvp.Value.HasOverLap(seq)) {
					kvp.Value.SendRead (seq);
				}
			}
			foreach (KeyValuePair<string, Region> kvp in Regions) {
				if (kvp.Value.HasOverLap(seq)) {
					kvp.Value.SendRead (seq);
				}
			}
		}

		public void SendVariant(Variant vri)
		{
			foreach (KeyValuePair<string, Gene> kvp in Genes) {
				if (kvp.Value.Contains(vri)) {
					kvp.Value.SendVariant(vri);
				}
			}
			foreach (KeyValuePair<string, Region> kvp in Regions) {
				if (kvp.Value.Contains(vri)) {
					kvp.Value.SendVariant(vri);
				}
			}
		}

		public void SendFKPM(FKPMGene fkp)
		{
			if(Genes.ContainsKey(fkp.gene))
			{
				Genes [fkp.gene].SendFKPM (fkp.fkpm);
			}
			if(Regions.ContainsKey(fkp.gene))
			{
				Regions [fkp.gene].SendFKPM (fkp.fkpm);
			}
		}

		public void SendElementFilter(List<ElementFilterInstruction> ins)
		{
			List<string> toRemoveGene = new List<string> ();
			List<string> toRemoveRegion = new List<string> ();

			foreach (ElementFilterInstruction ele in ins) {
				foreach (KeyValuePair<string, Gene> kvp in Genes) {
					if (ele.ElementType == kvp.Value.RegionTypeName) {
						if (kvp.Value.Length < ele.MinLength || kvp.Value.Length > ele.MaxLength) {
							toRemoveGene.Add (kvp.Key);
						}
					} else {
						kvp.Value.RecieveFilterInstruction (ele);
					}
				} 
				foreach (KeyValuePair<string, Region> kvp in Regions) {
					if (kvp.Value.RegionTypeName == ele.ElementType) {
						if (kvp.Value.Length < ele.MinLength || kvp.Value.Length > ele.MaxLength) {
							toRemoveRegion.Add (kvp.Key);
						}
					} else {
						kvp.Value.RecieveFilterInstruction (ele);
					}
				}
			}
			foreach (string st in toRemoveGene) {
				Genes.Remove (st);
			}
			foreach (string st in toRemoveRegion) {
				Regions.Remove (st);
			}
		}
			
	}
}

