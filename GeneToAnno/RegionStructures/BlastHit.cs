using System;

namespace GeneToAnno
{
	public class BlastHit
	{
		public string HitID;
		public double PercIdentity;
		public double EValue;

		public BlastHit (string id, double pid, double ev)
		{
			HitID = id;
			PercIdentity = pid;
			EValue = ev;
		}
	}
}

