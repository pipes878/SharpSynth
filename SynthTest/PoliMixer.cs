using SharpSynth;
using SharpSynth.Input;

namespace SynthTest
{
	public class PoliMixer
	{
		private ControlValue noise;
		private readonly Mixer mixer;
		private readonly ControlValue[] controls;

		public ISynthComponent Osc1
		{
			get { return controls[0].Control; }
			set { controls[0].Control = value; }
		}

		public ISynthComponent Osc2
		{
			get { return controls[1].Control; }
			set { controls[1].Control = value; }
		}

		public ISynthComponent Output => mixer;

		public float Osc1Level
		{
			get { return controls[0].Gain; }
			set { controls[0].Gain = value; }
		}

		public float Osc2Level
		{
			get { return controls[1].Gain; }
			set { controls[1].Gain = value; }
		}

		public float NoiseLevel
		{
			get { return noise.Gain; }
			set { noise.Gain = value; }
		}

		public PoliMixer()
		{
			noise = new ControlValue { Control = new Noise(), Gain = 0 };
			mixer = new Mixer();
			controls = new ControlValue[2];
			for (var i = 0; i < 2; i++)
				controls[i] = new ControlValue { Gain = .5f };
			mixer.Inputs.AddRange(controls);
			mixer.Inputs.Add(noise);
		}
	}
}