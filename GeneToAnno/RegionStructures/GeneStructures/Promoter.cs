using System;

namespace GeneToAnno
{
	public class Promoter : GeneElement
	{
		public Promoter (int buffer, int _start, int _end, Sense _sense) : base(_start, _end, _sense)
		{
			int finish = (_end - _start) + buffer;
			Component = "Promoter_-(" + buffer.ToString () + "-" + finish.ToString() + ")";
		}
		public Promoter (string _desc, int _start, int _end, Sense _sense) : base(_start, _end, _sense)
		{
			Component = _desc;
		}

		public Promoter (string _desc) : base()
		{
			Component = _desc;
		}

		public override string ToString ()
		{
			return Component;
		}
	}
}

