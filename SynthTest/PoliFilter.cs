using SharpSynth;
using SharpSynth.Input;

namespace SynthTest
{
	public class PoliFilter
	{
		private RangeValue attack;
		private RangeValue decay;
		private RangeValue sustain;
		private RangeValue release;

		private ControlValue lfoInput;
		private ControlValue triggerInput;
		private ControlValue envelopeOutput;
		private ControlValue filterControlValue;
		private Filter filter;
		private EnvelopeGenerator envelope;
		private Mixer mixer;

		public ISynthComponent Input
		{
			get { return filter.Input; }
			set { filter.Input = value; }
		}

		public ISynthComponent TriggerInput
		{
			get { return triggerInput.Control; }
			set { triggerInput.Control = value; }
		}

		public ISynthComponent LfoInput
		{
			get { return lfoInput.Control; }
			set { lfoInput.Control = value; }
		}

		public float Cutoff
		{
			get { return filterControlValue.BaseValue; }
			set { filterControlValue.BaseValue = value; }
		}

		public float LfoLevel
		{
			get { return lfoInput.Gain; }
			set { lfoInput.Gain = value; }
		}

		public float AdsrLevel
		{
			get { return envelopeOutput.Gain; }
			set { envelopeOutput.Gain = value; }
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

		public float Resonance
		{
			get { return filter.Q.BaseValue; }
			set { filter.Q.BaseValue = value; }
		}

		public FilterType FilterType
		{
			get { return filter.FilterType; }
			set { filter.FilterType = value; }
		}

		public ISynthComponent Output => filter;

		public PoliFilter()
		{
			triggerInput = new ControlValue();

			lfoInput = new ControlValue();
			envelope = new EnvelopeGenerator
			{
				Attack = attack = new RangeValue(0.0005f, 5),
				Decay = decay = new RangeValue(0.0005f, 5),
				Sustain = sustain = new RangeValue(0, 1),
				Release = release = new RangeValue(0.0005f, 5),
				Input = triggerInput
			};
			envelopeOutput = new ControlValue();
			envelopeOutput.Control = envelope;
			filterControlValue = new ControlValue();
			mixer = new Mixer();
			mixer.Inputs.Add(envelopeOutput);
			mixer.Inputs.Add(lfoInput);
			mixer.Inputs.Add(filterControlValue);

			filter = new Filter();
			// Don't know about this.
			filter.CornerFrequency.Control = new LinearFrequencyConverter(440) { Input = mixer };
		}
	}
}