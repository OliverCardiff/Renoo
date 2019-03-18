using System;

namespace GeneToAnno
{
	public class SeqRead
	{
		public BioSample Sample { get; set; }
		public int Start;
		public int End;
		public int Length { get { return End - Start; } }

		public SeqRead (BioSample samp)
		{
			Sample = samp;
		}
		public SeqRead(BioSample samp, int _start, int _end) 
		{
			Sample = samp;
			Start = _start;
			End = _end;
		}

		public bool Contains(int item)
		{
			if (item <= End && item >= Start) {
				return true;
			} else
				return false;
		}
	}

	public class NameRead : SeqRead
	{
		public string Name;

		public NameRead(BioSample samp, string name) : base(samp)
		{
			Name = name;
		}
		public NameRead(BioSample samp, int _start, int _end, string name)
			:base(samp, _start, _end)
		{
			Name = name;
		}
	}
}

