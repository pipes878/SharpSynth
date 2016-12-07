using SharpSynth.Input;

namespace SharpSynth
{
	/// <summary>
	/// Generates a gate value that is 0 when closed and 1 when open.
	/// </summary>
	public class GateGenerator : SynthComponent
	{
		public ControlValue TriggerThreshold { get; } = new ControlValue();

		/// <summary>
		/// Is the generator inverted (trigger below the threshold) or normal (trigger above the threshold).
		/// </summary>
		public bool IsInverted { get; set; }

		public SynthComponent Input { get; set; }

		#region Overrides of SynthComponent

		/// <summary>
		/// Generate synthesizer samples.
		/// </summary>
		/// <param name="buffer">The buffer to generate into.</param>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples, which is measured at 44100 samples per second.</param>
		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			if (Input == null)
			{
				for (var i = 0; i < count; i++)
					buffer[i++] = 0;
				return;
			}

			var threshold = TriggerThreshold.GenerateSamples(count, timeBase);
			var input = Input.GenerateSamples(count, timeBase);

			for (var i =0; i < count; i++)
			{
				buffer[i] = (input[i] > threshold[i]) ^ IsInverted ? 0 : 1;
			}
		}

		#endregion
	}
}