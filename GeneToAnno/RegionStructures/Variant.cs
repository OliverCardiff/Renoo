using System;
using System.Collections.Generic;
using System.IO;

namespace GeneToAnno
{
	public class SampleVariant
	{
		public BioSample BioS;
		public int MajorInd;
		public int AltInd;
		public SampleVariant(int ma, int alt, BioSample bio)
		{
			MajorInd = ma;
			AltInd = alt;
			BioS = bio;
		}
	}
	public class Variant : Element
	{
		string refAllele;
		string [] altAllele;
		int altCount;
		Dictionary<string, List<SampleVariant>> varTypes;

		public int AltLength { get { return altAllele [0].Length; } }
		public string RefAllele { get { return refAllele; } }
		public string AltAllele1 { get { return altAllele [0]; } }
		public string AltAllele2 { get { return altAllele [1]; } }
		public int AltCount { get { return altCount; } }

		public Variant (int location, string r, params string [] alt) : base(location, location + (alt[0].Length - 1), Sense.None)
		{
			altCount = alt.Length;
			altAllele = alt;
			refAllele = r;
		}

		public void AddSampleVars(SampleVariant vr)
		{
			if (varTypes == null) {
				varTypes = new Dictionary<string, List<SampleVariant>> ();
			}
			if (!varTypes.ContainsKey (vr.BioS.ID)) {
				varTypes.Add (vr.BioS.ID, new List<SampleVariant> ());
			}
			varTypes[vr.BioS.ID].Add(vr);
		}

		public int HasMajor(string id)
		{
			int retVal = 0;

			foreach (SampleVariant sv in varTypes[id]) {
				if (sv.MajorInd == 0) {
					retVal++;
				}
			}

			return retVal;
		}

		public int HasMinor(string id)
		{
			int retVal = 0;

			foreach (SampleVariant sv in varTypes[id]) {
				if (sv.AltInd != 0) {
					retVal++;
				}
			}

			return retVal;
		}

		public int IsHom(string id)
		{
			int retVal = 0;

			if (varTypes.ContainsKey (id)) {
				foreach (SampleVariant sv in varTypes[id]) {
					if (sv.MajorInd == sv.AltInd) {
						retVal++;
					}
				}
			}
			return retVal;
		}

		public int IsHet(string id)
		{
			int retVal = 0;

			if (varTypes.ContainsKey (id)) {
				foreach (SampleVariant sv in varTypes[id]) {
					if (sv.MajorInd != sv.AltInd) {
						retVal++;
					}
				}
			}
			return retVal;
		}

		public static bool IsGenotyped(string fileName, ref List<string> lstr)
		{
			bool genotyped = false;
			using (StreamReader sr = new StreamReader (fileName)) {
				bool breakout = false;
				while (!breakout) {
					string line = sr.ReadLine ();
					if (line.Contains ("#CHROM")) {
						string[] spstr = line.Split ('\t');

						if (spstr.Length > 8) {
							genotyped = true;
							lstr = new List<string> ();
							for (int i = 9; i < spstr.Length; i++) {
								lstr.Add (spstr [i]);
							}
						}
						breakout = true;
					}
				}
			}

			return genotyped;
		}
	}
}

