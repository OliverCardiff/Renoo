using System;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class SettingsItem<T>
	{
		private T theItem;
		public T Item { get { return theItem; } set { theItem = value; } }
		public Type ThisType { get; set; }

		public SettingsItem(T item)
		{
			ThisType = item.GetType();
			theItem = item;
		}
	}
	public struct LoadingSettings
	{
		public SettingsItem<string> GFF3_ID_TAG;
		public SettingsItem<bool> GFF3_SPLIT_ID;
		public SettingsItem<bool> GFF3_REMOVE_SUFF;
		public SettingsItem<List<string>> GFF3_PRESUFFIX_TO_TRIM;
		public SettingsItem<char[]> GFF3_ID_SPLIT_CHARS;
		public SettingsItem<int> GFF3_SPLIT_SUBSTR;

		public SettingsItem<bool> GEN_LITE_LOAD;
		public SettingsItem<bool> GEN_LINE_BREAK;
		public SettingsItem<bool> GEN_REMOVE_SUFF;
		public SettingsItem<bool> GEN_SPLIT_ID;
		public SettingsItem<List<string>> GEN_PRESUFFIX_TO_TRIM;
		public SettingsItem<char[]> GEN_ID_SPLIT_CHARS;
		public SettingsItem<int> GEN_SPLIT_SUBSTR;

		public SettingsItem<bool> FKPM_USE_OTHER_THAN_DIFFOUT;
		public SettingsItem<bool> FKPM_FILE_HAS_HILO;
		public SettingsItem<bool> FKPM_FILE_HAS_TESTOK;
		public SettingsItem<bool> FKPM_FILE_HAS_HEADER;
		public SettingsItem<int> FKPM_ID_COLUMN;
		public SettingsItem<int> FKPM_SCORE_COLUMN;
		public SettingsItem<int> FKPM_TESTOK_COLUMN;
		public SettingsItem<int> FKPM_TESTHI_COLUMN;
		public SettingsItem<int> FKPM_TESTLO_COLUMN;
		public SettingsItem<char[]> FKPM_FILE_DELIM;
		public SettingsItem<string> FKPM_TESTOK_TEXT;
		public SettingsItem<bool> LOAD_SEQ_NAMES;
	}

	public struct GeneProcessSettings
	{
		public SettingsItem<bool> GENERATE_ANY_PROMO;
		public SettingsItem<bool> GENERATE_PROMO_1;
		public SettingsItem<bool> GENERATE_PROMO_2;
		public SettingsItem<bool> GENERATE_PROMO_3;

		public SettingsItem<int> PROMO_1_SIZE;
		public SettingsItem<int> PROMO_2_SIZE;
		public SettingsItem<int> PROMO_3_SIZE;

		public SettingsItem<bool> GENERATE_FLANK3;
		public SettingsItem<int> FLANK3_SIZE;
		public SettingsItem<bool> GENERATE_INTRONS;
	}

	public struct ProcessingSettings
	{
		public SettingsItem<int> MAX_THREADS;
		public SettingsItem<bool> USE_THREADS;
	}

	public struct OutputSettings
	{
		public SettingsItem<bool> OUTPUT_PROCESSED;
		public SettingsItem<int> DEFAULT_IMG_BASES;
		public SettingsItem<bool> DEFAULT_IMG_FROMEND;
		public SettingsItem<bool> OVERLAY_RANK_VAR;
	}
	public struct StatisticsSettings
	{
		public SettingsItem<bool> NORMALISE_VARIANTS;
	}
	public static class AppSettings
	{
		public static LoadingSettings Loading;
		public static GeneProcessSettings Genes;
		public static ProcessingSettings Processing;
		public static OutputSettings Output;
		public static StatisticsSettings Statistic;

		public static void Assemble()
		{
			Loading = new LoadingSettings ();
			Genes = new GeneProcessSettings ();
			Processing = new ProcessingSettings ();
			Output = new OutputSettings ();
			Statistic = new StatisticsSettings ();

			Loading.GEN_LITE_LOAD = new SettingsItem<bool>(false);
			Loading.GEN_LINE_BREAK = new SettingsItem<bool>(false);
			Loading.GEN_SPLIT_ID = new SettingsItem<bool>(false);
			Loading.GEN_REMOVE_SUFF = new SettingsItem<bool>(false);
			Loading.GEN_PRESUFFIX_TO_TRIM = new SettingsItem<List<string>>(new List<string>());
			Loading.GEN_ID_SPLIT_CHARS = new SettingsItem<char[]> (new char[]{ });
			Loading.GEN_SPLIT_SUBSTR = new SettingsItem<int> (1);

			Loading.GFF3_SPLIT_SUBSTR = new SettingsItem<int> (1);
			Loading.GFF3_ID_TAG = new SettingsItem<string>("ID=");
			Loading.GFF3_ID_SPLIT_CHARS = new SettingsItem<char[]>(new char[] {'|'});
			Loading.GFF3_PRESUFFIX_TO_TRIM = new SettingsItem<List<string>>(new List<string>());
			Loading.GFF3_PRESUFFIX_TO_TRIM.Item.Add ("cds.");
			Loading.GFF3_REMOVE_SUFF = new SettingsItem<bool>(true);
			Loading.GFF3_SPLIT_ID = new SettingsItem<bool>(true);

			Loading.FKPM_USE_OTHER_THAN_DIFFOUT = new SettingsItem<bool>(false);
			Loading.FKPM_FILE_HAS_HILO = new SettingsItem<bool>(false);
			Loading.FKPM_FILE_HAS_TESTOK = new SettingsItem<bool>(false);
			Loading.FKPM_FILE_HAS_HEADER = new SettingsItem<bool>(true);
			Loading.FKPM_ID_COLUMN = new SettingsItem<int> (0);
			Loading.FKPM_SCORE_COLUMN = new SettingsItem<int> (9);
			Loading.FKPM_TESTOK_COLUMN = new SettingsItem<int> (12);
			Loading.FKPM_TESTHI_COLUMN = new SettingsItem<int> (11);
			Loading.FKPM_TESTLO_COLUMN = new SettingsItem<int> (10);
			Loading.FKPM_FILE_DELIM = new SettingsItem<char[]> (new char[] { '\t' });
			Loading.FKPM_TESTOK_TEXT = new SettingsItem<string> ("OK");
			Loading.LOAD_SEQ_NAMES = new SettingsItem<bool> (false);

			Genes.FLANK3_SIZE = new SettingsItem<int> (300);
			Genes.GENERATE_FLANK3 = new SettingsItem<bool> (true);
			Genes.GENERATE_ANY_PROMO = new SettingsItem<bool> (true);
			Genes.GENERATE_PROMO_1 = new SettingsItem<bool> (true);
			Genes.GENERATE_PROMO_2 = new SettingsItem<bool> (true);
			Genes.GENERATE_PROMO_3 = new SettingsItem<bool> (true);
			Genes.PROMO_1_SIZE = new SettingsItem<int> (34);
			Genes.PROMO_2_SIZE = new SettingsItem<int> (200);
			Genes.PROMO_3_SIZE = new SettingsItem<int> (1000);
			Genes.GENERATE_INTRONS = new SettingsItem<bool> (true);

			Processing.MAX_THREADS = new SettingsItem<int> (4);
			Processing.USE_THREADS = new SettingsItem<bool> (true);

			Output.OUTPUT_PROCESSED = new SettingsItem<bool> (false);
			Output.DEFAULT_IMG_BASES = new SettingsItem<int> (500);
			Output.DEFAULT_IMG_FROMEND = new SettingsItem<bool> (false);
			Output.OVERLAY_RANK_VAR = new SettingsItem<bool> (false);

			Statistic.NORMALISE_VARIANTS = new SettingsItem<bool> (true);
		}
	}
}

