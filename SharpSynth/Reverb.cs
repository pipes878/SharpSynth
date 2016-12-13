using SharpSynth.Input;

namespace SharpSynth
{
	public class Reverb : SynthComponent
	{
		private CombFilter[] combFilters = new CombFilter[4];
		private AllPassFilter[] allPassFilters = new AllPassFilter[2];

		private Mixer combMixer;

		#region Properties

		/// <summary>
		/// The mix of the dry signal and the wet signal, with 0 being completely dry, and 1 being completely wet.
		/// </summary>
		public ISynthComponent Mix { get; set; } = FixedValue.Half;

		/// <summary>
		/// The input to the reverb.
		/// </summary>
		public ISynthComponent Input
		{
			get { return combFilters[0].Input; }
			set
			{
				foreach (var c in combFilters)
					c.Input = value;
			}
		}

		#endregion

		public Reverb()
		{
			combFilters[0] = new CombFilter(0.0297f);
			combFilters[1] = new CombFilter(0.0371f);
			combFilters[2] = new CombFilter(0.0411f);
			combFilters[3] = new CombFilter(0.0437f);
			allPassFilters[0] = new AllPassFilter(0.005f);
			allPassFilters[1] = new AllPassFilter(0.0017f);

			combMixer = new Mixer();
			combMixer.Inputs.AddRange(combFilters);
			allPassFilters[0].Input = combMixer;
			allPassFilters[1].Input = allPassFilters[0];
		}

		#region SynthComponent Overrides

		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			if (Input == null)
			{
				for (var i = 0; i < buffer.Length; i++)
					buffer[i] = 0;
				return;
			}

			var input = Input.GenerateSamples(count, timeBase);
			var mix = Mix.GenerateSamples(count, timeBase);
			var reverb = allPassFilters[1].GenerateSamples(count, timeBase);

			for (var i = 0; i < buffer.Length; i++)
			{
				buffer[i] = reverb[i] * mix[i] + input[i] * (1 - mix[i]);
			}
		}

		#endregion
	}
}