using SharpSynth;

namespace SynthTest
{
	public class PoliLfo
	{
		private readonly Oscillator oscillator;

		public float Frequency
		{
			get { return oscillator.Frequency.BaseValue; }
			set { oscillator.Frequency.BaseValue = value; }
		}

		public OscillatorShape Shape
		{
			get { return oscillator.Shape; }
			set { oscillator.Shape = value; }
		}

		public ISynthComponent Output => oscillator;

		public PoliLfo()
		{
			oscillator = new Oscillator();
		}
	}
}