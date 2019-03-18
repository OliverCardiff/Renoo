using System;

namespace GeneToAnno
{
	public class MiscElement : GeneElement
	{
		public MiscElement (string typename, int _start, int _end, Sense _sense) : base(_start, _end, _sense)
		{
			Component = typename;
		}

		public MiscElement (string typename) : base()
		{
			Component = typename;
		}

		public override string ToString ()
		{
			return Component;
		}
	}
}

