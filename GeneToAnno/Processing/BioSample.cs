using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GeneToAnno
{
	public enum SampleType {BAM, Expr, Vcf};
	public class SeqScaffPair
	{
		public String Scaffold;
		public SeqRead Read;

		public SeqScaffPair(string scf, SeqRead sq)
		{
			Scaffold = scf;
			Read = sq;
		}
	}
	public class VarScaffPair
	{
		public String Scaffold;
		public Variant Variant;

		public VarScaffPair(string scf, Variant va)
		{
			Scaffold = scf;
			Variant = va;
		}
	}
	public class FKPMGene
	{
		public string gene;
		public FKPM fkpm;

		public FKPMGene(string gen, FKPM fk)
		{
			fkpm = fk;
			gene = gen;
		}
	}
	public class PreSampleDefinition
	{
		public SampleType SType;
		public string singleFName;
		public string singleColID;
		public int singleColIndex;
		public List<string> FileNames;
		public List<int> ColIDs;
		public string ID;
		public bool IsGenotyped;
		public int TotalColumns;

		public PreSampleDefinition(SampleType t)
		{
			FileNames = new List<string> ();
			ColIDs = new List<int> ();
			SType = t;
		}

		public PreSampleDefinition(PreSampleDefinition other, string _ID)
		{
			singleColIndex = other.singleColIndex;
			singleColID = other.singleColID;
			FileNames = other.FileNames;
			FileNames.Add (other.singleFName);
			ColIDs = other.ColIDs;
			ColIDs.Add (other.singleColIndex);
			IsGenotyped = other.IsGenotyped;
			TotalColumns = other.TotalColumns;
			SType = other.SType;
			TotalColumns = other.TotalColumns;
			ID = _ID;
		}

		public void MergeSamp(PreSampleDefinition other)
		{
			FileNames.AddRange (other.FileNames);
			FileNames.Add (other.singleFName);
			ColIDs.Add (other.singleColIndex);
			ColIDs.AddRange (other.ColIDs);
		}

		public override string ToString ()
		{
			switch (SType) {
			case SampleType.BAM:
				return singleFName;
			case SampleType.Expr:
				return singleFName + " -- " + singleColID;
			case SampleType.Vcf:
				if (IsGenotyped) {
					return singleFName + " -- " + singleColID;
				} else
					return singleFName;
			default:
				return "none";
			}
		}
	}
	public class BioSample
	{
		public string ID {get; set;}
		public List<SeqScaffPair> AllBAMPairs;
		public bool HasBAM { get; set; }
		public bool HasExprs { get; set; }
		public bool HasVars { get; set; }

		public static Dictionary<string, BioSample[]> LatestVarColIndexed;
		public static Dictionary<string, bool> IsGenotypedNames;
		public static bool hasVarSomethingToLoad;
		public static bool hasFKPMSomethingToLoad;
		public static List<VarScaffPair> AllVarPairs;
		public static List<FKPMGene> AllFKPMS;
		public static Dictionary<string, BioSample[]> LatestFKPMColIndexed;

		public BioSample (string _id)
		{
			AllBAMPairs = new List<SeqScaffPair> (10000000);
			HasBAM = false;
			HasExprs = false;
			HasVars = false;

			ID = _id;
		}

		public void RecieveDefinition(PreSampleDefinition pres)
		{
			if (pres.SType == SampleType.BAM) {
				SendBAMs (pres.FileNames);
			} else if (pres.SType == SampleType.Expr) {
				SendToStaticFKPM (pres, this);
			} else if (pres.SType == SampleType.Vcf) {
				SendToStatic (pres, this);
			}
		}
		public static void ResetStaticLoader()
		{
			LatestVarColIndexed = null;
			IsGenotypedNames = null;
			hasVarSomethingToLoad = false;
			hasFKPMSomethingToLoad = false;
			AllVarPairs = new List<VarScaffPair> ();
			AllFKPMS = new List<FKPMGene> ();
		}
		protected static void SendToStaticFKPM(PreSampleDefinition p, BioSample bios)
		{
			bios.HasExprs = true;
			if (LatestFKPMColIndexed == null) {
				LatestFKPMColIndexed = new Dictionary<string, BioSample[]> ();
			}

			foreach(string st in p.FileNames)
			{
				if (!LatestFKPMColIndexed.ContainsKey (st)) {
					LatestFKPMColIndexed.Add (st, new BioSample[p.TotalColumns]);
				}
				foreach (int ii in p.ColIDs) {
					int index = ii - 9;

					if (index != 0) {
						index /= 4;
					}

					LatestFKPMColIndexed [st] [index] = bios;
				}
			}
			hasFKPMSomethingToLoad = true;
		}
		protected static void SendToStatic(PreSampleDefinition p, BioSample bios)
		{
			bios.HasVars = true;
			if (LatestVarColIndexed == null)
			{
				LatestVarColIndexed = new Dictionary<string, BioSample[]> ();
			}
			if (IsGenotypedNames == null) {
				IsGenotypedNames = new Dictionary<string, bool> ();
			}
			if (p.IsGenotyped) {
				if (!LatestVarColIndexed.ContainsKey (p.FileNames [0])) {
					LatestVarColIndexed.Add (p.FileNames [0], new BioSample[p.TotalColumns]);
				}
				foreach (int ii in p.ColIDs) {
					LatestVarColIndexed [p.FileNames [0]] [ii - 9] = bios;
				}
				if (!IsGenotypedNames.ContainsKey (p.FileNames [0])) {
					IsGenotypedNames.Add (p.FileNames [0], p.IsGenotyped);
				}
			} else {
				foreach(string s in p.FileNames)
				{
					if (!IsGenotypedNames.ContainsKey (s)) {
						IsGenotypedNames.Add (s, p.IsGenotyped);
					}
				}
			}
			hasVarSomethingToLoad = true;
		}

		public static void LoadAllStatic()
		{
			if (hasVarSomethingToLoad) {
				LoadAllVars ();
			}
			if (hasFKPMSomethingToLoad) {
				LoadAllFKPMS ();
			}
		}

		protected static void LoadAllFKPMS()
		{
			AllFKPMS.Clear ();
			MainData.LoadedFKPM = 0;
			ProgramState.LoadedFPKM = false;
			ProgramState.FKPMLoadLock = false;

			foreach (KeyValuePair<string, BioSample[]> kvp in LatestFKPMColIndexed) {
				LoadFKPMFile (kvp.Key, kvp.Value);
			}

			MainData.UpdateLog ("Assigning FKPM scores to genes..", true);
			MainData.Genome.SendFKPMS (AllFKPMS);
			MainData.UpdateLog ("Assignment of: " + MainData.LoadedFKPM.ToString() + " fkpm scores complete", true);

			ProgramState.LoadedFPKM = true;
			ProgramState.FKPMLoadLock = true;
			hasFKPMSomethingToLoad = false;
			AllFKPMS.Clear ();
		}

		protected static void LoadAllVars()
		{
			AllVarPairs.Clear ();
			MainData.LoadedVars = 0;
			ProgramState.LoadedVariants = false;
			ProgramState.VariantLoadLock = false;

			foreach (KeyValuePair<string, BioSample[]> kvp in LatestVarColIndexed) {
				LoadVarFile (kvp.Key, kvp.Value, IsGenotypedNames [kvp.Key]);
			}

			MainData.UpdateLog ("Assigning Variants to annotation elements..", true);
			MainData.Genome.SendVars (AllVarPairs);
			MainData.UpdateLog ("Assignment of: " + MainData.LoadedVars.ToString() + " variants complete", true);

			ProgramState.LoadedVariants = true;
			ProgramState.VariantLoadLock = true;
			hasVarSomethingToLoad = false;
			AllVarPairs.Clear ();
		}

		protected static void LoadFKPMFile(string fName, BioSample[] samps)
		{
			if (!AppSettings.Loading.FKPM_USE_OTHER_THAN_DIFFOUT.Item) {
				using (StreamReader sr = new StreamReader (fName)) {
					sr.ReadLine ();
					string line;
					int idInd = AppSettings.Loading.FKPM_ID_COLUMN.Item;

					while ((line = sr.ReadLine ()) != null) {
						string[] spstr = line.Split ('\t');
						string gen = spstr [idInd];
						for (int i = 9; (i + 3) < spstr.Length; i += 4) {
							bool parseSuccess = true;
							double hi; double lo; double fkpm;
							if (!double.TryParse (spstr [i], out fkpm)) {
								parseSuccess = false;
							}
							if (!double.TryParse (spstr [i + 1], out hi)) {
								parseSuccess = false;
							}
							if (!double.TryParse (spstr [i + 2], out lo)) {
								parseSuccess = false;
							}
							if (!spstr [i + 3].Contains ("OK")) {
								parseSuccess = false;
							}
							int sampInd = i - 9;
							if (i > 0) {
								sampInd++;
								sampInd /= 4;
							}
							if (parseSuccess) {
								FKPM fk = new FKPM (fkpm, hi, lo, samps [sampInd]);
								FKPMGene fkg = new FKPMGene (gen, fk);
								AllFKPMS.Add (fkg);
							}
						}
					}
				}
			} else {

				int colsPerItem = 1;
				if (AppSettings.Loading.FKPM_FILE_HAS_HILO.Item)
					colsPerItem += 2;
				if (AppSettings.Loading.FKPM_FILE_HAS_TESTOK.Item)
					colsPerItem += 1;

				char[] spch = AppSettings.Loading.FKPM_FILE_DELIM.Item;
				int firstScore = AppSettings.Loading.FKPM_SCORE_COLUMN.Item;
				int idInd = AppSettings.Loading.FKPM_ID_COLUMN.Item;

				using (StreamReader sr = new StreamReader (fName)) {
					sr.ReadLine ();
					string line;

					while ((line = sr.ReadLine ()) != null) {
						
						string[] spstr = line.Split (spch);
						string gen = spstr [idInd];

						for (int i = firstScore; (i + colsPerItem - 1) < spstr.Length; i += colsPerItem) {
							bool parseSuccess = true;
							double hi = 0; double lo = 0; double fkpm;

							if (!double.TryParse (spstr [i], out fkpm)) {
								parseSuccess = false;
							}
							if (AppSettings.Loading.FKPM_FILE_HAS_HILO.Item) {
								int loIn = AppSettings.Loading.FKPM_TESTLO_COLUMN.Item;
								int hiIn = AppSettings.Loading.FKPM_TESTHI_COLUMN.Item;

								loIn = i + loIn - firstScore;
								hiIn = i + hiIn - firstScore;

								if (!double.TryParse (spstr [hiIn], out hi)) {
									parseSuccess = false;
								}
								if (!double.TryParse (spstr [loIn], out lo)) {
									parseSuccess = false;
								}
							}
							if (AppSettings.Loading.FKPM_FILE_HAS_TESTOK.Item) {
								int textIn = AppSettings.Loading.FKPM_TESTOK_COLUMN.Item;
								textIn = i + textIn - firstScore;

								if (!spstr [textIn].Contains (AppSettings.Loading.FKPM_TESTOK_TEXT.Item)) {
									parseSuccess = false;
								}
							}
							int sampInd = i - 9;
							if (i > 0) {
								sampInd++;
								sampInd /= 4;
							}
							if (parseSuccess) {
								FKPM fk = new FKPM (fkpm, hi, lo, samps [sampInd]);
								FKPMGene fkg = new FKPMGene (gen, fk);
								AllFKPMS.Add (fkg);
							}
						}
					}
				}
			}
		}

		protected static void LoadVarFile(string fName, BioSample [] samps, bool genotyped)
		{
			char[] seps = { '/', '|' };
			int count = 0;
			using (StreamReader sr = new StreamReader (fName)) {

				while ((int)'#' == sr.Peek ()) {
					sr.ReadLine ();
				}

				string[] spstrs;
				string line;

				while ((line = sr.ReadLine ()) != null) {
					spstrs = line.Split ('\t');

					if (spstrs [6] == "PASS") {
						Variant v;

						string[] sps = spstrs [4].Split (',');
						if (sps.Length == 1) {
							v = new Variant (int.Parse (spstrs [1]), spstrs [3], spstrs [4]);
						} else if (sps.Length == 2) {
							v = new Variant (int.Parse (spstrs [1]), spstrs [3], sps [0], sps [1]);
						} else {
							v = new Variant (int.Parse (spstrs [1]), spstrs [3], sps [0], sps [1], sps [2]);
						}

						if (genotyped) {
							int genLen = spstrs.Length;
							int stInd = 9;

							for (int i = stInd; i < genLen; i++) {
								bool parsed = true;

								string[] sps2 = spstrs [i].Split (':');
								string[] sps3 = sps2 [0].Split (seps);
								int ma = 0;
								int al = 0;
								if (!int.TryParse (sps3 [0], out ma)) {
									parsed = false;
								}
								if (sps3.Length > 1) {
									if (!int.TryParse (sps3 [1], out al)) {
										parsed = false;
									}
								}
								if (parsed) {
									SampleVariant s = new SampleVariant (ma, al, samps [i - stInd]);
									v.AddSampleVars (s);
								}
							}
						}

						AllVarPairs.Add (new VarScaffPair (spstrs [0], v));

						count++;
						if (count % 100000 == 0) {
							int tot = count / 100000;
							MainData.UpdateLog ("Read " + tot + "00K variants from " + MainData.MainWindow.MaxLenString (fName, 20), true);
						}
					}
				}
			}
		}

		protected void SendBAMs(List<string> fNames)
		{
			AllBAMPairs.Clear ();
			MainData.LoadedSeqs = 0;
			ProgramState.LoadedSamples = false;
			ProgramState.BAMLoadLock = false;

			foreach (string st in fNames) {
				MainData.UpdateLog ("Reading " + MainData.MainWindow.MaxLenString (st, 20) + " to Sample ID -- \"" + ID + "\" -- ", true);
				if (AppSettings.Loading.LOAD_SEQ_NAMES.Item) {
					LoadBAMNameFile (st);
				} else
					LoadBAMFile (st);
			}
			if (AllBAMPairs.Count > 0) {
				HasBAM = true;
			}

			MainData.UpdateLog ("Assigning -- \"" + ID + "\" -- sample reads to all annotated regions..", true);
			MainData.Genome.SendReads (AllBAMPairs);
			MainData.UpdateLog ("Assignment of: " + MainData.LoadedSeqs.ToString() + " seqs complete", true);
			ProgramState.LoadedSamples = true;
			ProgramState.BAMLoadLock = true;
			AllBAMPairs.Clear ();
		}

		protected void LoadBAMFile(string fileName)
		{
			int count = 0;

			using (StreamReader st = new StreamReader (fileName)) {

				string line = "";
				while ((int)'@' == st.Peek()) {
					st.ReadLine ();
				}
				while ((line = st.ReadLine ()) != null) {
					string[] spstrs = line.Split ('\t');
					if (spstrs [2].Length > 2 && spstrs.Length >= 10) {
						int start = int.Parse (spstrs [3]);
						SeqRead sq = new SeqRead (this, start, spstrs [9].Length + start);
						AllBAMPairs.Add (new SeqScaffPair (spstrs [2], sq));

						count++;
						if (count % 1000000 == 0) {
							int tot = count / 1000000;
							MainData.UpdateLog ("Read " + tot + "M seqs from " + MainData.MainWindow.MaxLenString (fileName, 20), true);
						}
					}
				}
			}
			//Parallel.ForEach(lines, x => 
			//	{
			//		ProcessLines(x);
			//	});

		}

		protected void LoadBAMNameFile(string fileName)
		{
			int count = 0;

			using (StreamReader st = new StreamReader (fileName)) {

				string line = "";
				while ((int)'@' == st.Peek()) {
					st.ReadLine ();
				}
				while ((line = st.ReadLine ()) != null) {
					string[] spstrs = line.Split ('\t');
					if (spstrs [2].Length > 2 && spstrs.Length >= 10) {
						int start = int.Parse (spstrs [3]);
						NameRead sq = new NameRead (this, start, spstrs [9].Length + start, spstrs[0]);
						AllBAMPairs.Add (new SeqScaffPair (spstrs [2], sq));

						count++;
						if (count % 1000000 == 0) {
							int tot = count / 1000000;
							MainData.UpdateLog ("Read " + tot + "M seqs from " + MainData.MainWindow.MaxLenString (fileName, 20), true);
						}
					}
				}
			}
			//Parallel.ForEach(lines, x => 
			//	{
			//		ProcessLines(x);
			//	});

		}

		protected void ProcessLines(string line)
		{
			string[] spstrs = line.Split ('\t');
			if (spstrs [2].Length > 2) {
				int start = int.Parse (spstrs [3]);
				SeqRead sq = new SeqRead (this, start, spstrs [9].Length + start);
				AllBAMPairs.Add (new SeqScaffPair (spstrs [2], sq));
			}
		}
	}
}

