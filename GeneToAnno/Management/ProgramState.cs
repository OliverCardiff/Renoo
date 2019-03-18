using System;
using System.Collections.Generic;

namespace GeneToAnno
{
	public enum SensitiType{Genome, GFF3, Outfmt6, FPKM, Variants, BAM, GenomeGraph, MethGraph, GeneGraph, GlobalGraph, ComparativeGraph, ClusterGraph, ImgAll, GenLoadLock, GffLoadLock, Outfmt6LoadLock, BAMLoadLock, VariantLoadLock, FKPMLoadLock};

	public static class ProgramState
	{
		public static int SENSITYPE_COUNT = 19;

		private static bool _loadedGenome;
		private static bool _loadedGFF3;
		private static bool _loadedSamples;
		private static bool _loadedBlast;
		private static bool _loadedFPKM;
		private static bool _loadedVariants;

		private static bool _genomeMadeGraph;
		private static bool _geneMadeGraph;
		private static bool _methyMadeGraph;
		private static bool _globalMadeGraph;
		private static bool _comparativeMadeGraph;
		private static bool _clusterMadeGraph;
		private static bool _imgHasBitmaps;

		private static bool _genomeLoadLock;
		private static bool _gff3LoadLock;
		private static bool _Outfmt6LoadLock;
		private static bool _BAMLoadLock;
		private static bool _VariantLoadLock;
		private static bool _FKPMLoadLock;

		//Use to get locks on ui elements while loading is happening
		public static bool GenomeLoadLock { get { return _genomeLoadLock; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.GenLoadLock, value);
				_genomeLoadLock = value;
			});
			} }
		public static bool GFF3LoadLock { get { return _gff3LoadLock; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.GffLoadLock, value);
				_gff3LoadLock = value;
			});
			} }
		public static bool OutFmt6LoadLock { get { return _Outfmt6LoadLock; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.Outfmt6LoadLock, value);
				_Outfmt6LoadLock = value;
			});
			} }
		public static bool BAMLoadLock { get { return _BAMLoadLock; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.BAMLoadLock, value);
				_BAMLoadLock = value;
			});
			} }
		public static bool VariantLoadLock { get { return _VariantLoadLock; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.VariantLoadLock, value);
				_VariantLoadLock = value;
			});
			} }
		public static bool FKPMLoadLock { get { return _FKPMLoadLock; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.FKPMLoadLock, value);
				_FKPMLoadLock = value;
			});
			} }
		//end of locks

		//Use to determine when graph output window components should become sensitive
		public static bool ImageHasBitmap{ get { return _imgHasBitmaps; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.ImgAll, value);
			}); } }
		public static bool MadeGenomeGraph{ get { return _genomeMadeGraph; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.GenomeGraph, value);
				_genomeMadeGraph = value;
			}); } }
		public static bool MadeGeneGraph{ get { return _geneMadeGraph; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.GeneGraph, value);
				_geneMadeGraph = value;
			}); } }
		public static bool MadeMethGraph{ get { return _methyMadeGraph; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.MethGraph, value);
				_methyMadeGraph = value;
			}); } }
		public static bool MadeGlobalGraph{ get { return _globalMadeGraph; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.GlobalGraph, value);
				_globalMadeGraph = value;
			}); } }
		public static bool MadeComparativeGraph{ get { return _comparativeMadeGraph; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.ComparativeGraph, value);
				_comparativeMadeGraph = value;
			}); } }
		public static bool MadeClusterGraph{ get { return _clusterMadeGraph; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.ClusterGraph, value);
				_clusterMadeGraph = value;
			}); } }
		//end of output windows

		//Use to determine whether user has loaded data a certain type
		public static bool LoadedGenome { get { return _loadedGenome; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.Genome, value);
				_loadedGenome = value;
			}); } }
		public static bool LoadedGFF3 { get { return _loadedGFF3; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.GFF3, value);
				_loadedGFF3 = value;
			}); } }
		public static bool LoadedSamples { get { return _loadedSamples; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.BAM, value);
				_loadedSamples = value;
				MainData.MainWindow.UpdateRankSplit ();
			}); } }
		public static bool LoadedBlast { get { return _loadedBlast; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.Outfmt6, value);
				_loadedBlast = value;
				MainData.MainWindow.UpdateRankSplit ();
			}); } }
		public static bool LoadedFPKM { get { return _loadedFPKM; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.FPKM, value);
				_loadedFPKM = value;
				MainData.MainWindow.UpdateRankSplit ();
			}); } }
		public static bool LoadedVariants { get { return _loadedVariants; } set { Gtk.Application.Invoke (delegate {
				MainData.MainWindow.Sensitize (SensitiType.Variants, value);
				_loadedVariants = value;
				MainData.MainWindow.UpdateRankSplit ();
			}); } }
		//end of data loading

		public static int VariantSampleCount { get; set; }
		public static int FPKMSampleCount { get; set; }
		public static int SampleCount { get; set; }

		public static void ResetAll()
		{
			LoadedGenome = false;
			LoadedGFF3 = false;
			LoadedSamples = false;
			SampleCount = 0;
			LoadedBlast = false;
			LoadedFPKM = false;
			FPKMSampleCount = 0;
			LoadedVariants = false;
			VariantSampleCount = 0;

			MadeGeneGraph = false;
			MadeGenomeGraph = false;
			MadeMethGraph = false;
			MadeComparativeGraph = false;
			MadeGlobalGraph = false;
			MadeClusterGraph = false;
		}
		public static void Init()
		{
			_loadedGenome = false;
			_loadedGFF3 = false;
			_loadedSamples = false;
			SampleCount = 0;
			_loadedBlast = false;
			_loadedFPKM = false;
			FPKMSampleCount = 0;
			_loadedVariants = false;
			VariantSampleCount = 0;

			_genomeMadeGraph = false;
			_geneMadeGraph = false;
			_methyMadeGraph = false;
			_globalMadeGraph = false;
			_comparativeMadeGraph = false;
			_clusterMadeGraph = false;

			_genomeLoadLock = true;
			_gff3LoadLock = true;
			_Outfmt6LoadLock = true;
			_BAMLoadLock = true;
			_VariantLoadLock = true;
			_FKPMLoadLock = true;
		}
	}
}