using System;

namespace GeneToAnno
{
	public class UTR5 : GeneElement
	{
		public UTR5 (int _start, int _end, Sense _sense) : base(_start, _end, _sense)
		{
			Component = "five_prime_UTR";
		}
		public UTR5 () : base()
		{
			Component = "five_prime_UTR";
		}
	}
}

