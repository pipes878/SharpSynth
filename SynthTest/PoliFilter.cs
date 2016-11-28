using SharpSynth;

namespace SynthTest
{
	public class PoliFilter
	{
		private ControlInput lfoInput;
		private ControlInput triggerInput;
		private ControlInput envelopeOutput;
		private ControlInput filterControlInput;
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
			get { return filterControlInput.BaseValue; }
			set { filterControlInput.BaseValue = value; }
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

		public FilterType FilterType
		{
			get { return filter.FilterType; }
			set { filter.FilterType = value; }
		}

		public ISynthComponent Output => filter;

		public PoliFilter()
		{
			triggerInput = new ControlInput();

			lfoInput = new ControlInput();
			envelope = new EnvelopeGenerator();
			envelope.Input = triggerInput;
			envelopeOutput = new ControlInput();
			envelopeOutput.Control = envelope;
			filterControlInput = new ControlInput();
			mixer = new Mixer();
			mixer.Inputs.Add(envelopeOutput);
			mixer.Inputs.Add(lfoInput);
			mixer.Inputs.Add(filterControlInput);

			filter = new Filter();
			// Don't know about this.
			filter.CornerFrequency.Control = new LinearFrequencyConverter(440) { Input = mixer };
		}
	}
}