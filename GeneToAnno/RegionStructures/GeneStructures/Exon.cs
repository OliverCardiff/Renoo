using System;

namespace GeneToAnno
{
	public class Exon : GeneElement
	{
		public int NumberInGene { get; set; }

		public Exon (int _number, int _start, int _end, Sense _sense) : base(_start, _end, _sense)
		{
			NumberInGene = _number;
			Component = "exon";
		}
		public Exon (int _number) : base()
		{
			NumberInGene = _number;
			Component = "exon";
		}
			
		public override string ToStringPlusNumber ()
		{
			return base.ToString () + "_" + NumberInGene.ToString ();
		}
	}
}

