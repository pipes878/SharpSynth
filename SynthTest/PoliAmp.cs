using System;
using SharpSynth;
using SharpSynth.Input;

namespace SynthTest
{
	public class PoliAmp
	{
		private Amplifier lfoAmp;
		private Amplifier envelopeAmp;
		private Amplifier finalAmp;
		private EnvelopeGenerator envelope;
		private ControlValue lfoInput;

		public ISynthComponent Input
		{
			get { return finalAmp.Input; }
			set { finalAmp.Input = value; }
		}

		public ISynthComponent TriggerInput
		{
			get { return envelope.Input; }
			set { envelope.Input = value; }
		}

		public float Attack
		{
			get { return envelope.Attack.BaseValue; }
			set { envelope.Attack.BaseValue = value; }
		}

		public float Decay
		{
			get { return envelope.Decay.BaseValue; }
			set { envelope.Decay.BaseValue = value; }
		}

		public float Sustain
		{
			get { return envelope.Sustain.BaseValue; }
			set { envelope.Sustain.BaseValue = value; }
		}

		public float Release
		{
			get { return envelope.Release.BaseValue; }
			set { envelope.Release.BaseValue = value; }
		}

		public float LfoLevel
		{
			get { return ((RangeValue)lfoAmp.Gain).Value; }
			set { ((RangeValue)lfoAmp.Gain).Value = value; }
		}

		public ISynthComponent LfoInput
		{
			get { return lfoAmp.Input; }
			set { lfoAmp.Input = value; }
		}

		public ISynthComponent Output => finalAmp;

		public float Gain
		{
			get { return ((ControlValue)finalAmp.Gain).Gain; }
			set { ((ControlValue)finalAmp.Gain).Gain = value; }
		}

		public PoliAmp()
		{
			lfoAmp = new Amplifier();
			lfoAmp.Gain = new RangeValue(0, 2);

			lfoInput = new ControlValue { BaseValue = 1f };
			lfoInput.Control = lfoAmp;

			envelope = new EnvelopeGenerator();
			envelopeAmp = new Amplifier();
			envelopeAmp.Gain = envelope;
			envelopeAmp.Input = lfoInput;

			finalAmp = new Amplifier();
			finalAmp.Gain = new ControlValue { Control = envelopeAmp };
		}
	}
}