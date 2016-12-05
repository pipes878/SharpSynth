using System;
using SharpSynth;

namespace SynthTest
{
	public class PoliAmp
	{
		private Amplifier lfoAmp;
		private Amplifier envelopeAmp;
		private Amplifier finalAmp;
		private EnvelopeGenerator envelope;
		private ControlInput lfoInput;

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
			get { return lfoAmp.Gain.BaseValue; }
			set { lfoAmp.Gain.BaseValue = value; }
		}

		public ISynthComponent LfoInput
		{
			get { return lfoAmp.Input; }
			set { lfoAmp.Input = value; }
		}

		public ISynthComponent Output => finalAmp;

		public PoliAmp()
		{
			lfoAmp = new Amplifier();
			lfoAmp.Gain.BaseValue = 0;

			lfoInput = new ControlInput { BaseValue = 1f };
			lfoInput.Control = lfoAmp;

			envelope = new EnvelopeGenerator();
			envelopeAmp = new Amplifier();
			envelopeAmp.Gain.BaseValue = 0;
			envelopeAmp.Gain.Control = envelope;
			envelopeAmp.Input = lfoInput;

			finalAmp = new Amplifier();
			finalAmp.Gain.BaseValue = 0;
			finalAmp.Gain.Control = envelopeAmp;
		}
	}
}