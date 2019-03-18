using System;

namespace GeneToAnno
{
	public class Intron : GeneElement
	{
		public int NumberInGene { get; set; }

		public Intron (int _number, int _start, int _end, Sense _sense) : base(_start, _end, _sense)
		{
			NumberInGene = _number;
			Component = "intron";
		}
		public Intron (int _number) : base ()
		{
			NumberInGene = _number;
			Component = "intron";
		}

		public override string ToStringPlusNumber ()
		{
			return base.ToString () + "_" + NumberInGene.ToString ();
		}
	}
}

