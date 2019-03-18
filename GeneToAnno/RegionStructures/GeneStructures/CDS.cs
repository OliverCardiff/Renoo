using System;

namespace GeneToAnno
{
	public class CDS : GeneElement
	{
		public int NumberInGene { get; set; }

		public CDS (int _number, int _start, int _end, Sense _sense) : base(_start, _end, _sense)
		{
			NumberInGene = _number;
			Component = "CDS";
		}
		public CDS (int _number) : base()
		{
			NumberInGene = _number;
			Component = "CDS";
		}

		public override string ToStringPlusNumber ()
		{
			return base.ToString () + "_" + NumberInGene.ToString ();
		}
	}
}

