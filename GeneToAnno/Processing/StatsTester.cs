using System;
using System.Collections.Generic;
using Meta.Numerics.Statistics;

namespace GeneToAnno
{
	public enum PairedTest { KendallTau, StudentT, SpearmanRho, PearsonR };
	public enum BetweenGroupTest { MannWhitney, Kolmogorov, Kruskal, StudentT, Anova };
	public static class StatsTester
	{
		public static Dictionary<PairedTest, string> PairedTestDesc;
		public static Dictionary<string, PairedTest> PairedTestEnum;
		public static Dictionary<BetweenGroupTest, string> BetweenTestDesc;
		public static Dictionary<string, BetweenGroupTest> BetweenTestEnum;

		public static void Assemble ()
		{
			PairedTestDesc = new Dictionary<PairedTest, string> ();
			PairedTestEnum = new Dictionary<string, PairedTest> ();
			BetweenTestDesc = new Dictionary<BetweenGroupTest, string> ();
			BetweenTestEnum = new Dictionary<string, BetweenGroupTest> ();

			PairedTestDesc.Add (PairedTest.KendallTau, "Kendall Tau Rank Correlation Coefficient");
			PairedTestDesc.Add (PairedTest.PearsonR, "Pearson Product-Moment Coefficient");
			PairedTestDesc.Add (PairedTest.SpearmanRho, "Spearman Rank Correlation Coefficient");
			PairedTestDesc.Add (PairedTest.StudentT, "Paired Student T Test");
			PairedTestEnum.Add (PairedTestDesc [PairedTest.KendallTau], PairedTest.KendallTau);
			PairedTestEnum.Add (PairedTestDesc [PairedTest.PearsonR], PairedTest.PearsonR);
			PairedTestEnum.Add (PairedTestDesc [PairedTest.SpearmanRho], PairedTest.SpearmanRho);
			PairedTestEnum.Add (PairedTestDesc [PairedTest.StudentT], PairedTest.StudentT);

			BetweenTestDesc.Add (BetweenGroupTest.Anova, "One-Way ANOVA Test");
			BetweenTestDesc.Add (BetweenGroupTest.MannWhitney, "Mann-Whitney U Test");
			BetweenTestDesc.Add (BetweenGroupTest.Kruskal, "Kruskal Wallis Analysis of Variance");
			BetweenTestDesc.Add (BetweenGroupTest.Kolmogorov, "Two-Sample Kolmogorov-Smirov Test");
			BetweenTestDesc.Add (BetweenGroupTest.StudentT, "Student's T Test");
			BetweenTestEnum.Add (BetweenTestDesc [BetweenGroupTest.Anova], BetweenGroupTest.Anova);
			BetweenTestEnum.Add (BetweenTestDesc [BetweenGroupTest.MannWhitney], BetweenGroupTest.MannWhitney);
			BetweenTestEnum.Add (BetweenTestDesc [BetweenGroupTest.Kruskal], BetweenGroupTest.Kruskal);
			BetweenTestEnum.Add (BetweenTestDesc [BetweenGroupTest.Kolmogorov], BetweenGroupTest.Kolmogorov);
			BetweenTestEnum.Add (BetweenTestDesc [BetweenGroupTest.StudentT], BetweenGroupTest.StudentT);
		}

		public static TestResult RunPairedTest(List<double> x, List<double> y, PairedTest pt)
		{
			BivariateSample s = new BivariateSample ();
			s.Add (x, y);

			switch (pt) {
			case PairedTest.KendallTau:
				return s.KendallTauTest ();
			case PairedTest.PearsonR:
				return s.PearsonRTest ();
			case PairedTest.SpearmanRho:
				return s.SpearmanRhoTest ();
			case PairedTest.StudentT:
				return s.PairedStudentTTest ();
			default:
				return null;
			}
		}

		public static TestResult RunBetweenTest(List<double> x, List<double> y, BetweenGroupTest bt)
		{
			Sample sm1 = new Sample (x);
			Sample sm2 = new Sample (y);
		    
			switch (bt) {
			case BetweenGroupTest.Anova:
				return Sample.OneWayAnovaTest (sm1, sm2).Factor.Result;
			case BetweenGroupTest.Kolmogorov:
				return Sample.KolmogorovSmirnovTest (sm1, sm2);
			case BetweenGroupTest.Kruskal:
				return Sample.KruskalWallisTest (sm1, sm2);
			case BetweenGroupTest.MannWhitney:
				return Sample.MannWhitneyTest (sm1, sm2);
			case BetweenGroupTest.StudentT:
				return Sample.StudentTTest (sm1, sm2);
			default:
				return null;
			}
		}
	}
}

