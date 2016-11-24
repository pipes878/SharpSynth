using System.Dynamic;
using SharpSynth;

namespace SynthTest
{
	public class Vco
	{
		#region Fields

		private ControlInput octave;
		private ControlInput detune;
		private ControlInput cvIn;
		private ControlInput lfoMod;
		private ControlInput crossMod;

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

		public ISynthComponent LfoInput
		{
			get { return lfoMod.Control; }
			set { lfoMod.Control = value; }
		}

		public float LfoLevel
		{
			get { return lfoMod.Gain; }
			set { lfoMod.Gain = value; }
		}

		public ISynthComponent XModInput
		{
			get { return crossMod.Control; }
			set { crossMod.Control = value; }
		}

		public float XModLevel
		{
			get { return crossMod.Gain; }
			set { crossMod.Gain = value; }
		}

		#endregion

		#region Construction

		public Vco()
		{
			octave = new ControlInput();
			detune = new ControlInput();
			cvIn = new ControlInput();
			lfoMod = new ControlInput { Gain = 0 };
			crossMod = new ControlInput { Gain = 0 };

			controlInputMix = new Mixer();
			controlInputMix.Inputs.Add(octave);
			controlInputMix.Inputs.Add(detune);
			controlInputMix.Inputs.Add(cvIn);
			controlInputMix.Inputs.Add(lfoMod);
			controlInputMix.Inputs.Add(crossMod);

			frequencyConverter = new LinearFrequencyConverter(440) { Input = controlInputMix };
			oscillator = new Oscillator();
			oscillator.Frequency.Control = frequencyConverter;
		}

		#endregion
	}

	public class Lfo
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


		public Lfo()
		{
			oscillator = new Oscillator();
		}
	}
}