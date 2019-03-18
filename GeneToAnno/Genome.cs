using System;
using System.IO;
using System.Collections.Generic;
using Bio.IO.Gff;

namespace GeneToAnno
{
	public partial class Genome
	{
		protected Dictionary<string, Scaffold> Scaffolds;
		public string FileName { get; set; }
		public string ShortFileName { get{
				char [] chrs = {'/','\\'};
				string[] strs = FileName.Split (chrs, 50);
				int lastInd = strs.Length - 1;
				return strs [lastInd];
			}
		}
		public List<Scaffold> ShuffledScaffolds { get {
				List<Scaffold> nscaf = new List<Scaffold> (Scaffolds.Values);
				Shuffle<Scaffold> (nscaf);
				return nscaf;
			}
		}
		public static void Shuffle<T>(IList<T> list)  
		{  
			Random rng = new Random();  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = rng.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}

		public Scaffold GetScaffold(string st)
		{
			if (Scaffolds.ContainsKey (st)) {
				return Scaffolds [st];
			} else
				return null;
		}

		public Genome (string _fileName)
		{
			Scaffolds = new Dictionary<string, Scaffold> ();
			FileName = _fileName;
		}
		public Genome ()
		{
			Scaffolds = new Dictionary<string, Scaffold> ();
			FileName = "";
		}

		public List<Scaffold> GetScaffoldList()
		{
			List<Scaffold> scaffs = new List<Scaffold> ();
			foreach (KeyValuePair<string, Scaffold> scfs in Scaffolds) {
				scaffs.Add (scfs.Value);
			}
			return scaffs;
		}

		public void LoadGenomeLongSeq(string _fileName)
		{
			bool lite = AppSettings.Loading.GEN_LITE_LOAD.Item;
			FileName = _fileName;
			Scaffolds.Clear ();

			long count = 0;
			string line = "";
			string sequence = "";
			string id = "";
			//long modulo = MainData.ScanFileForModulo (_fileName, 10);
			long nextMark = 50000000;
			long markIncrement = 50000000;
			int markCnt = 0;
			count = 0;

			MainData.UpdateLog ("Reading...", true);

			using (StreamReader sr = new StreamReader (_fileName)) {
				while (!sr.EndOfStream) {
					line = sr.ReadLine ();
					if (line.Length < 3) {
						continue;
					} else if (line.StartsWith (">")) {
						id = line.TrimStart ('>');
						GENApplyOptionsToID 	 (ref id);
					} else {
						sequence = line;

						Scaffolds.Add (id, (new Scaffold (id, ref sequence, lite)));
						count += sequence.Length;
					}
					if (count > nextMark) {
						markCnt++;
						nextMark += markIncrement;
						MainData.UpdateLog ("Read " + (markCnt * 50).ToString() + " mega-bases from genome..", true);
					}
				}

			}
			long finalCnt = count / 1000000;
			MainData.UpdateLog ("Read " + finalCnt.ToString() + " mega-bases in total.", true);
		}

		public void LoadGenomeReturnSeq(string _fileName)
		{
			bool lite = AppSettings.Loading.GEN_LITE_LOAD.Item;
			FileName = _fileName;
			Scaffolds.Clear ();

			long count = 0;
			string line = "";
			string sequence = "";
			string id = "";
			//long modulo = MainData.ScanFileForModulo (_fileName, 10);
			long nextMark = 50000000;
			long markIncrement = 50000000;
			int markCnt = 0;
			count = 0;

			MainData.UpdateLog ("Reading...", true);

			using (StreamReader sr = new StreamReader (_fileName)) {

				while ((line = sr.ReadLine ()) != null) {
					if (!line.StartsWith (">")) {
						sequence += line;
					} else {
						if (sequence != "") {
							Scaffolds.Add (id, (new Scaffold (id, ref sequence, lite)));
							count += sequence.Length;
							sequence = "";
						}
						id = line.TrimStart ('>');
						GENApplyOptionsToID (ref id);
					}
				
					if (count > nextMark) {
						markCnt++;
						nextMark += markIncrement;
						MainData.UpdateLog ("Read " + (markCnt * 50).ToString() + " mega-bases from genome..", true);
					}
				}
			}
			long finalCnt = count / 1000000;
			MainData.UpdateLog ("Read " + finalCnt.ToString() + " mega-bases in total.", true);
		}

		protected void GENApplyOptionsToID(ref string line)
		{
			if (AppSettings.Loading.GEN_SPLIT_ID.Item) {
				line = line.Split (AppSettings.Loading.GEN_ID_SPLIT_CHARS.Item, 10, StringSplitOptions.RemoveEmptyEntries) [AppSettings.Loading.GEN_SPLIT_SUBSTR.Item - 1];
			}
			if (AppSettings.Loading.GEN_REMOVE_SUFF.Item) {
				foreach (string st in AppSettings.Loading.GEN_PRESUFFIX_TO_TRIM.Item) {
					int index = line.IndexOf (st);
					line = (index < 0) ? line : line.Remove (index, st.Length);
				}
			}
		}

		protected void GFF3ApplyOptionsToID(ref string line)
		{
			if (AppSettings.Loading.GFF3_SPLIT_ID.Item) {
				line = line.Split (AppSettings.Loading.GFF3_ID_SPLIT_CHARS.Item, 10, StringSplitOptions.RemoveEmptyEntries) [AppSettings.Loading.GFF3_SPLIT_SUBSTR.Item - 1];
			}
			if (AppSettings.Loading.GFF3_REMOVE_SUFF.Item) {
				foreach (string st in AppSettings.Loading.GFF3_PRESUFFIX_TO_TRIM.Item) {
					int index = line.IndexOf (st);
					line = (index < 0) ? line : line.Remove (index, st.Length);
				}
			}
		}

		public void LoadOutFmt6(string fileName)
		{
			MainData.LoadedBlasts = 0;
			Dictionary<string, BlastHit> bhits = new Dictionary<string, BlastHit> ();

			string line = "";
			using (StreamReader sr = new StreamReader (fileName)) {
				while ((line = sr.ReadLine ()) != null) {
					string[] spstr = line.Split ('\t');
					string[] spstr2 = spstr [0].Split ('|');

					bool parseSuccess = true;
					double ev; double pid;

					if (!double.TryParse (spstr [2], out pid)) {
						parseSuccess = false;
					}
					if (!double.TryParse (spstr [10], out ev)) {
						parseSuccess = false;
					}

					if (parseSuccess) {
						BlastHit next = new BlastHit (spstr [1], pid, ev);
						if (!bhits.ContainsKey (spstr2 [0])) {
							bhits.Add (spstr2 [0], next);
						}
					}
				}
			}

			foreach (KeyValuePair<string, BlastHit> kvp in bhits) {
				if (MainData.GeneToScaffDict.ContainsKey (kvp.Key)) {
					if (Scaffolds [MainData.GeneToScaffDict [kvp.Key]].Genes.ContainsKey (kvp.Key)) {
						Scaffolds [MainData.GeneToScaffDict [kvp.Key]].Genes [kvp.Key].BlastHit = kvp.Value;
						MainData.LoadedBlasts++;
					}
				}
				if (MainData.RegionToScaffDict.ContainsKey (kvp.Key)) {
					if (Scaffolds [MainData.RegionToScaffDict [kvp.Key]].Regions.ContainsKey (kvp.Key)) {
						Scaffolds [MainData.RegionToScaffDict [kvp.Key]].Regions [kvp.Key].BlastHit = kvp.Value;
						MainData.LoadedBlasts++;
					}
				}
			}
		}

		public void LoadGFF3(string fileName)
		{
			string idTag = AppSettings.Loading.GFF3_ID_TAG.Item;
			int tagSize = idTag.Length;

			string line = "";
			string parentTag = "arent=";
			int pTagSize = parentTag.Length;
			int count = 0; 
			long modulo = MainData.ScanFileForModulo (fileName, 5);
			long lineCount = modulo * 5;
			MainData.UpdateLog ("Reading... ", true);

			char[] spChar = {'\t'}; char[] spChar2 = { ';' };

			using (StreamReader sr = new StreamReader (fileName)) {
				while ((line = sr.ReadLine ()) != null) {
					if (line.Length > 10) {
						string[] spstr = line.Split (spChar, 10);
						string[] spstr2 = spstr [8].Split (spChar2, 20);
						string ID = "";
						string pID = "";
						foreach (string st in spstr2) {
							if (st.Contains (idTag)) {
								ID = st;
								break;
							}
						}
						foreach (string st in spstr2) {
							if (st.Contains (parentTag)) {
								pID = st;
								break;
							}
						}
						ID = ID.Substring (tagSize);
						GFF3ApplyOptionsToID (ref ID);
						if (pID != "") {
							GFF3ApplyOptionsToID (ref pID);
						}

						Sense sn = Sense.Sense;
						if (spstr [6].Equals ("-")) {
							sn = Sense.AntiSense;
						} else if (spstr [6].Equals (".")) {
							sn = Sense.None;
						}

						string nameOfType = ID + "#" + spstr [2] + "#" + pID;;
						int sta, end;
						bool A = int.TryParse (spstr [3], out sta);
						bool B = int.TryParse (spstr [4], out end);
						if (A && B) {
							if (Scaffolds.ContainsKey (spstr [0])) {
								Scaffolds [spstr [0]].SendComponent (nameOfType, sta, end, sn);
							} else
								throw new Exception ("Gff3 references scaffold not found in genome!");
						}

					}
					count++;
					if (count % modulo == 0) {
						long perc = (long)(((float)count / (float)lineCount) * 100);
						MainData.UpdateLog ("Read " + perc + "% of gff3..", true);
					}
				}
			}
			if (AppSettings.Genes.GENERATE_ANY_PROMO.Item || AppSettings.Genes.GENERATE_FLANK3.Item) {
				MainData.UpdateLog ("Generating GFF3 additional elements as described in Settings -> Genes", true);
				foreach (KeyValuePair<string, Scaffold> kvp in Scaffolds) {
					kvp.Value.RunGenePostLoad ();
				}
			}
		}

		public List<double> FindAllBaseRatios()
		{
			List<double> ratios = new List<double> ();

			if (!AppSettings.Processing.USE_THREADS.Item) {
				foreach (KeyValuePair<string, Scaffold> scaffpair in Scaffolds) {
					List<double> scafRats = scaffpair.Value.GetFragmentBaseRatio (400);
					ratios.AddRange (scafRats);
				}

				return ratios;
			} else {
				List<List<double>> dataPool = MainThreader.RunDividedList<Scaffold, List<double>> 
				(FindSubGroupBaseRatio, ShuffledScaffolds);

				foreach (List<double> dl in dataPool) {
					ratios.AddRange(dl);
				}
				return ratios;
			}
		}

		protected List<double> FindSubGroupBaseRatio(List<Scaffold> scf)
		{
			List<double> rats = new List<double> ();

			foreach (Scaffold sc in scf) {
				List<double> scafRats = sc.GetFragmentBaseRatio (400);
				rats.AddRange (scafRats);
			}

			return rats;
		}

		public Dictionary<string, List<double>> FindAllElementBaseRatios()
		{
			if (!AppSettings.Processing.USE_THREADS.Item) {
				return FindElementBRScaffList (new List<Scaffold> (Scaffolds.Values));
			} else {
				
				List<Dictionary<string, List<double>>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, List<double>>>
				(FindElementBRScaffList, ShuffledScaffolds);

				return MainData.TypeDict.MergeDownDicts<double> (thList);
			}
		}

		protected Dictionary<string, List<double>> FindElementBRScaffList(List<Scaffold> scaffs)
		{
			double theVal = 0;

			Dictionary<string, List<double>> eDict = MainData.TypeDict.GetEmptyTypeDict<double> (TypeDictCase.Genes);

			foreach (Scaffold scaff in scaffs) {
				foreach (KeyValuePair<string, Gene> genKVP in scaff.Genes) {
					foreach (KeyValuePair<string, List<GeneElement>> kvp in genKVP.Value.Elements) {
						foreach (GeneElement ge in kvp.Value) {
							if (eDict.ContainsKey (ge.ToString ())) {
								if (scaff.TryGetBaseRatio (ge.Start, ge.End, out theVal)) {
									eDict [ge.ToString ()].Add (theVal);
								}
							}
						}
						if (eDict.ContainsKey (genKVP.Value.RegionTypeName)) {
							if (scaff.TryGetBaseRatio (genKVP.Value.Start, genKVP.Value.End, out theVal)) {
								eDict [genKVP.Value.RegionTypeName].Add (theVal);
							}
						}
					}
				}
				foreach (KeyValuePair<string, Region> renKVP in scaff.Regions) {
					foreach (KeyValuePair<string, List<MiscElement>> kvp in renKVP.Value.RegElements) {
						foreach (MiscElement me in kvp.Value) {
							if (eDict.ContainsKey (me.ToString ())) {
								if (scaff.TryGetBaseRatio (me.Start, me.End, out theVal)) {
									eDict [me.ToString ()].Add (theVal);
								}
							}
						}
						if (eDict.ContainsKey (renKVP.Value.RegionTypeName)) {
							if (scaff.TryGetBaseRatio (renKVP.Value.Start, renKVP.Value.End, out theVal)) {
								eDict [renKVP.Value.RegionTypeName].Add (theVal);
							}
						}
					}
				}
			}
			return eDict;
		}

		public Dictionary<string, List<double>> GetAllElementCoverages()
		{
			if(!AppSettings.Processing.USE_THREADS.Item) {
				return GetAllEleCovScaffList(new List<Scaffold>(Scaffolds.Values));
			} else {
				List<Dictionary<string, List<double>>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, List<double>>>
				(GetAllEleCovScaffList, ShuffledScaffolds);

				return MainData.TypeDict.MergeDownDicts<double>(thList);
			}
		}

		protected Dictionary<string, List<double>> GetAllEleCovScaffList(List<Scaffold> scaffs)
		{
			Dictionary<string, List<double>> covs = MainData.TypeDict.GetEmptyTypeDict<double> (TypeDictCase.Methylation);

			foreach (Scaffold scf in scaffs) {
				scf.GetElementCoverages (covs);
			}

			return covs;
		}

		public Dictionary<string, List<double>> GetAllElementDepthScores()
		{
			if (!AppSettings.Processing.USE_THREADS.Item) {
				return GetAllEleDepthScaffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, List<double>>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, List<double>>>
				(GetAllEleDepthScaffList, ShuffledScaffolds);

				return MainData.TypeDict.MergeDownDicts<double> (thList);
			}
		}

		protected Dictionary<string, List<double>> GetAllEleDepthScaffList(List<Scaffold> scaffs)
		{
			Dictionary<string, List<double>> dict = MainData.TypeDict.GetEmptyTypeDict<double> (TypeDictCase.Methylation);

			foreach (Scaffold scf in scaffs) {
				scf.GetElementDepthCoverages (dict);
			}

			return dict;
		}

		public Dictionary<string, List<double>> GetAllElementLengths()
		{
			if (!AppSettings.Processing.USE_THREADS.Item) {
				return GetAllEleLenScaffList (new List<Scaffold> (Scaffolds.Values));
			} else {
				List<Dictionary<string, List<double>>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, List<double>>>
				(GetAllEleLenScaffList, ShuffledScaffolds);

				return MainData.TypeDict.MergeDownDicts<double> (thList);
			}
		}

		protected Dictionary<string, List<double>> GetAllEleLenScaffList(List<Scaffold> scaffs)
		{
			Dictionary<string, List<double>> lens = MainData.TypeDict.GetEmptyTypeDict<double> (TypeDictCase.Genes);

			foreach (Scaffold scf in scaffs) {
				scf.GetElementLengths (lens);
			}
			return lens;
		}

		public Dictionary<string, List<List<double>>> GetAllSingleSpatialCoverages()
		{
			if (!AppSettings.Processing.USE_THREADS.Item) {
				return GetAllSingleSpatCovScafffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, List<List<double>>>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, List<List<double>>>>
				(GetAllSingleSpatCovScafffList, ShuffledScaffolds);

				return MainData.TypeDict.MergeDownDicts2D<double> (thList);
			}
		}

		protected Dictionary<string, List<List<double>>> GetAllSingleSpatCovScafffList(List<Scaffold> scfs)
		{
			Dictionary<string, List<List<double>>> tdict = MainData.TypeDict.GetEmptyTypeDict2D<double> (TypeDictCase.Methylation, 20);

			foreach (Scaffold scf in scfs) {
				scf.GetElementSpatialCoverages (tdict, 20);
			}

			return tdict;
		}

		public Dictionary<string, List<List<double>>> GetAllSingleSpatialDepthCoverages()
		{
			if (!AppSettings.Processing.USE_THREADS.Item) {
				return GetAllSingleSpatDepthCovScafffList (ShuffledScaffolds);
			} else {
				List<Dictionary<string, List<List<double>>>> thList = MainThreader.RunDividedList<Scaffold, Dictionary<string, List<List<double>>>>
				(GetAllSingleSpatDepthCovScafffList, ShuffledScaffolds);

				return MainData.TypeDict.MergeDownDicts2D<double> (thList);
			}
		}

		protected Dictionary<string, List<List<double>>> GetAllSingleSpatDepthCovScafffList(List<Scaffold> scfs)
		{
			Dictionary<string, List<List<double>>> tdict = MainData.TypeDict.GetEmptyTypeDict2D<double> (TypeDictCase.Methylation, 20);

			foreach (Scaffold scf in scfs) {
				scf.GetElementSpatialDepthCoverages (tdict, 20);
			}

			return tdict;
		}

		public void FillGeneToScaffDict(Dictionary<string, string> dict)
		{
			foreach (KeyValuePair<string, Scaffold> kvp in Scaffolds) {
				foreach (KeyValuePair<string, Gene> kvp2 in kvp.Value.Genes) {
					dict.Add (kvp2.Key, kvp.Key);
				}
			}
		}

		public void FillRegionToScaffDict(Dictionary<string, string> dict)
		{
			foreach (KeyValuePair<string, Scaffold> kvp in Scaffolds) {
				foreach (KeyValuePair<string, Region> kvp2 in kvp.Value.Regions) {
					dict.Add (kvp2.Key, kvp.Key);
				}
			}
		}

		public void SendVars(List<VarScaffPair> vpr)
		{
			if (!AppSettings.Processing.USE_THREADS.Item) {
				SendVarsScaffList (vpr);
			} else {
				MainThreader.RunDividedList<VarScaffPair> (SendVarsScaffList, vpr);
			}
		}

		protected void SendVarsScaffList(List<VarScaffPair> vpr)
		{
			foreach (VarScaffPair vsp in vpr) {
				Scaffolds [vsp.Scaffold].SendVariant (vsp.Variant);
			}
		}

		public void SendFKPMS(List<FKPMGene> fkls)
		{
			foreach (FKPMGene fkp in fkls) {
				if (MainData.GeneToScaffDict.ContainsKey (fkp.gene)) {
					Scaffolds[MainData.GeneToScaffDict [fkp.gene]].SendFKPM(fkp);
				}
				if (MainData.RegionToScaffDict.ContainsKey (fkp.gene)) {
					Scaffolds [MainData.RegionToScaffDict [fkp.gene]].SendFKPM (fkp);
				}
			}
		}

		public void SendReads(List<SeqScaffPair> sql)
		{
			if (!AppSettings.Processing.USE_THREADS.Item) {
				SendReadScaffList (sql);
			} else {
				MainThreader.RunDividedList<SeqScaffPair> (SendReadScaffList, sql);
			}
		}

		protected void SendReadScaffList(List<SeqScaffPair> sql)
		{
			foreach (SeqScaffPair s in sql) {
				Scaffolds [s.Scaffold].SendRead (s.Read);
			}
		}

		public void RunElementFilter(List<ElementFilterInstruction> ins)
		{
			foreach (KeyValuePair<string, Scaffold> kvp in Scaffolds) {
				kvp.Value.SendElementFilter (ins);
			}
		}
	}
}

