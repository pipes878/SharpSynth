using SharpSynth.Input;

namespace SharpSynth
{
	public class Amplifier : SynthComponent
	{
		/// <summary>
		/// The gain of the amplifier. If this value is a fraction then the amp will be an attenuator.
		/// </summary>
		public ISynthComponent Gain { get; set; } = FixedValue.One;

		/// <summary>
		/// The amplifier input.
		/// </summary>
		public ISynthComponent Input { get; set; }

		#region Implementation of ISynthComponent

		/// <summary>
		/// Generate synthesizer samples
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

			var gain = Gain.GenerateSamples(count, timeBase);

			var inputData = Input.GenerateSamples(count, timeBase);
			for (var i = 0; i < count; i++)
				buffer[i] = inputData[i] * gain[i];
		}

		#endregion
	}
}