using System;

namespace GeneToAnno
{
	public class UTR3 : GeneElement
	{
		public UTR3 (int _start, int _end, Sense _sense) : base(_start, _end, _sense)
		{
			Component = "three_prime_UTR";
		}
		public UTR3 () : base()
		{
			Component = "three_prime_UTR";
		}
	}
}

