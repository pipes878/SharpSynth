using System;
using SharpSynth;
using SharpSynth.Input;

namespace SynthTest
{
	public class PoliAmp
	{
		private RangeValue attack;
		private RangeValue decay;
		private RangeValue sustain;
		private RangeValue release;

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
			get { return attack.Value; }
			set { attack.Value = value; }
		}

		public float Decay
		{
			get { return decay.Value; }
			set { decay.Value = value; }
		}

		public float Sustain
		{
			get { return sustain.Value; }
			set { sustain.Value = value; }
		}

		public float Release
		{
			get { return release.Value; }
			set { release.Value = value; }
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

			envelope = new EnvelopeGenerator
			{
				Attack = attack = new RangeValue(0.0005f, 5),
				Decay = decay = new RangeValue(0.0005f, 5),
				Sustain = sustain = new RangeValue(0, 1),
				Release = release = new RangeValue(0.0005f, 5)
			};

			envelopeAmp = new Amplifier();
			envelopeAmp.Gain = envelope;
			envelopeAmp.Input = lfoInput;

			finalAmp = new Amplifier();
			finalAmp.Gain = new ControlValue { Control = envelopeAmp };
		}
	}
}