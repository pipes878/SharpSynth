using SharpSynth;

namespace SynthTest
{
	public class PoliMixer
	{
		private readonly Mixer mixer;
		private readonly ControlInput[] controls;

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

		public PoliMixer()
		{
			mixer = new Mixer();
			controls = new ControlInput[2];
			for (var i = 0; i < 2; i++)
				controls[i] = new ControlInput();
			mixer.Inputs.AddRange(controls);
		}
	}
}