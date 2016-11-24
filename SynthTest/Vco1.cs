using SharpSynth;

namespace SynthTest
{
	public class Vco1
	{
		#region Fields

		private ControlInput octave;
		private ControlInput detune;
		private ControlInput cvIn;
		private Mixer controlInputMix;
		private LinearFrequencyConverter frequencyConverter;
		private Oscillator oscillator;

		#endregion

		#region Properties

		public ISynthComponent ControlInput
		{
			get { return cvIn.Control; }
			set { cvIn.Control = value; }
		}

		public ISynthComponent Output => oscillator;

		public int Octave
		{
			get { return (int)octave.BaseValue; }
			set { octave.BaseValue = value; }
		}

		public float Detune
		{
			get { return detune.BaseValue; }
			set { detune.BaseValue = value; }
		}

		public OscillatorShape Shape
		{
			get { return oscillator.Shape; }
			set { oscillator.Shape = value; }
		}

		#endregion

		#region Construction

		public Vco1()
		{
			octave = new ControlInput() { BaseValue = 0 };
			detune = new ControlInput() { BaseValue = 0, Gain = 1 / 12f };
			cvIn = new ControlInput();
			controlInputMix = new Mixer();
			controlInputMix.Inputs.Add(octave);
			controlInputMix.Inputs.Add(detune);
			controlInputMix.Inputs.Add(cvIn);
			frequencyConverter = new LinearFrequencyConverter(440) { Input = controlInputMix };
			oscillator = new Oscillator();
			oscillator.Frequency.Control = frequencyConverter;
		}

		#endregion
	}
}