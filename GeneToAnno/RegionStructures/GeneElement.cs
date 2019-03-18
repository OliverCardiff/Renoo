using System;
using System.Collections.Generic;

namespace GeneToAnno
{
	public class ElementFilterInstruction
	{
		public string ElementType;
		public int MinLength;
		public int MaxLength;

		public ElementFilterInstruction(string ty, int min, int max)
		{
			ElementType = ty;
			MinLength = min;
			MaxLength = max;
		}
	}
	public class GeneElement : Element
	{
		public string Component { get; set; }

		public GeneElement () : base()
		{
			Reads = null;
		}
		public GeneElement(int _start, int _end, Sense sense)
			:base(_start, _end, sense)
		{
			Reads = null;
		}

		public override string ToString ()
		{
			return Component;
		}

		public virtual string ToStringPlusNumber()
		{
			return Component;
		}
	}
}

