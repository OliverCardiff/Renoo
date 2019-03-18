using System;
using System.IO;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class FKPM
	{
		public BioSample Sample;
		public double FKPMCount;
		public double HI;
		public double LO;

		public FKPM (double fkpm, double hi, double lo, BioSample samp)
		{
			Sample = samp;
			FKPMCount = fkpm;
			HI = hi;
			LO = lo;
		}

		public static void GetSampleNames(string fName, out List<string> sms)
		{
			if (!AppSettings.Loading.FKPM_USE_OTHER_THAN_DIFFOUT.Item) {
				sms = new List<string> ();
				using (StreamReader sr = new StreamReader (fName)) {
					string[] spstr = sr.ReadLine ().Split ('\t');

					for (int i = 9; i < spstr.Length; i += 4) {
						string[] sps = spstr [i].Split ('_');
						sms.Add (sps [0]);
					}
				}
			} else {
				sms = new List<string> ();

				int colsPerItem = 1;
				if (AppSettings.Loading.FKPM_FILE_HAS_HILO.Item)
					colsPerItem += 2;
				if (AppSettings.Loading.FKPM_FILE_HAS_TESTOK.Item)
					colsPerItem += 1;

				char[] spch = AppSettings.Loading.FKPM_FILE_DELIM.Item;
				int firstScore = AppSettings.Loading.FKPM_SCORE_COLUMN.Item;

				using (StreamReader sr = new StreamReader (fName)) {
					string[] spstr = sr.ReadLine ().Split (spch);

					for (int i = firstScore; i < spstr.Length; i +=colsPerItem) {
						sms.Add (spstr [i]);
					}
				}
			}
		}
	}
}

