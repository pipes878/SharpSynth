using SharpSynth;

namespace SynthTest
{
	public class PoliFilter
	{
		private Filter filter;
		private EnvelopeGenerator envelope;
		private Cutoff cutoff;
		private Mixer mixer;

		public ISynthComponent Input { get; set; }

		public ISynthComponent Output { get; private set; }

		public PoliFilter()
		{
			filter = new Filter();
		}
	}
}