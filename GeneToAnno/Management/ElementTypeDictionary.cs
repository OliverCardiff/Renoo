using System;
using Gtk;
using System.Collections.Generic;

namespace GeneToAnno
{
	public enum TypeDictCase { Genome, Genes, Methylation, Global, Comparative, Cluster };

	public class ElementTypeDictionary
	{
		protected List<string> allTypes;
		protected Dictionary<TypeDictCase, List<string>> typeCases;

		public ElementTypeDictionary ()
		{
			typeCases = new Dictionary<TypeDictCase, List<string>> ();
			allTypes = new List<string> ();
			GenerateBlankCases ();
		}

		protected void GenerateBlankCases()
		{
			typeCases.Clear ();
			for (int i = 0; i < 6; i++) {
				typeCases.Add((TypeDictCase)i, new List<string>());
				foreach (string st in allTypes) {
					typeCases [(TypeDictCase)i].Add (st);
				}
			}
		}

		public List<string> GetTypesforCase(TypeDictCase useCase)
		{
			return typeCases [useCase];
		}

		public List<string> GetAllTypes()
		{
			return allTypes;
		}

		public void RecieveTypesForCase(TypeDictCase useCase, List<string> lstr)
		{
			typeCases [useCase] = lstr;
		}

		public int RunSetTypeDictCaseWindow(TypeDictCase useCase)
		{
			SetEleDictWindow tdw = new SetEleDictWindow (this, useCase);
			tdw.Modal = true;
			tdw.Show ();
			return tdw.Run ();
		}

		public Dictionary<string, List<T>> MergeDownDicts<T>(List<Dictionary<string, List<T>>> dicts)
		{
			Dictionary<string, List<T>> dicto = new Dictionary<string, List<T>> ();

			foreach (string st in dicts[0].Keys) {
				dicto.Add (st, new List<T> ());
			}

			foreach (Dictionary<string, List<T>> di in dicts) {
				foreach (KeyValuePair<string, List<T>> kvp in di) {
					dicto [kvp.Key].AddRange (kvp.Value);
				}
			}
			return dicto;
		}

		public Dictionary<string, List<T>> GetEmptyTypeDict<T>(TypeDictCase useCase)
		{
			Dictionary <string, List<T>> dbl = new Dictionary<string, List<T>> ();

			foreach (string st in typeCases[useCase]) {
				dbl.Add (st, new List<T> ());
			}

			return dbl;
		}

		public Dictionary<string, T> MergeDownDictsSingular<T>(List<Dictionary<string, T>> dicts)
		{
			Dictionary<string, T> reDict = new Dictionary<string, T>();

			foreach (Dictionary<string, T> dictI in dicts) {
				if (dictI != null) {
					foreach (KeyValuePair<string, T> kvp in dictI) {
						if (!reDict.ContainsKey (kvp.Key)) {
							reDict.Add (kvp.Key, kvp.Value);
						}
					}
				}
			}

			return reDict;
		}

		public Dictionary<string, List<List<T>>> MergeDownDicts2D<T>(List<Dictionary<string, List<List<T>>>> dicts)
		{
			int innerCount = 1;
			foreach (KeyValuePair<string, List<List<T>>> kvp in dicts[0]) {
				innerCount = kvp.Value.Count;
				break;
			}

			Dictionary<string, List<List<T>>> dicto = new Dictionary<string, List<List<T>>> ();

			foreach (string st in dicts[0].Keys) {
				dicto.Add (st, new List<List<T>> ());

				for (int i = 0; i < innerCount; i++) {
					dicto [st].Add (new List<T> ());
				}
			}

			foreach (Dictionary<string, List<List<T>>> di in dicts) {
				foreach (KeyValuePair<string, List<List<T>>> kvp in di) {
					int lcnt = 0;
					foreach (List<T> innerL in kvp.Value) {
						dicto [kvp.Key][lcnt].AddRange(innerL);
						lcnt++;
					}
				}
			}
			return dicto;
		}

		public Dictionary<string, List<List<T>>> GetEmptyTypeDict2D<T>(TypeDictCase useCase, int innerEntCount)
		{
			Dictionary <string, List<List<T>>> dbl = new Dictionary<string, List<List<T>>> ();

			foreach (string st in typeCases[useCase]) {
				dbl.Add (st, new List<List<T>> ());

				for (int i = 0; i < innerEntCount; i++) {
					dbl [st].Add (new List<T> ());
				}
			}

			return dbl;
		}

		public Dictionary<string, T> GetEmptyTypeDictSingular<T>(T val)
		{
			T valuetype = val;

			Dictionary <string, T> dbl = new Dictionary<string, T> ();

			foreach (string st in allTypes) {
				dbl.Add (st, valuetype);
			}

			return dbl;
		}

		public void FindAllEntries(Genome gen)
		{
			List<Scaffold> scaffList = gen.GetScaffoldList ();

			foreach (Scaffold scf in scaffList) {
				foreach (KeyValuePair<string, Gene> ge in scf.Genes) {
					if (!allTypes.Contains (ge.Value.RegionTypeName)) {
						allTypes.Add (ge.Value.RegionTypeName);
					}
					foreach (KeyValuePair<string, List<GeneElement>> kvp in ge.Value.Elements) {
						foreach (GeneElement genEle in kvp.Value) {
							if (!allTypes.Contains (genEle.ToString())) {
								allTypes.Add (genEle.ToString());
							}
						}

					}
				}
				foreach (KeyValuePair<string, Region> re in scf.Regions) {
					if (!allTypes.Contains (re.Value.RegionTypeName)) {
						allTypes.Add (re.Value.RegionTypeName);
					}
					foreach (KeyValuePair<string, List<MiscElement>> kvp in re.Value.RegElements) {
						foreach (MiscElement mscEle in kvp.Value) {
							if (!allTypes.Contains (mscEle.ToString())) {
								allTypes.Add (mscEle.ToString());
							}
						}
					}
				}
			}

			GenerateBlankCases ();
		}
	}
}

