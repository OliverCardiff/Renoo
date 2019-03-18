using System;

namespace GeneToAnno
{
	public class Flank3 : GeneElement
	{
		public Flank3 (int _start, int _end, Sense _sense) : base(_start, _end, _sense)
		{
			Component = "flank3";
		}
		public Flank3() : base()
		{
			Component = "flank3";
		}
	}
}

