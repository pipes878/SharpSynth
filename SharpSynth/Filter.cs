using System;

namespace SharpSynth
{
	public class Filter : SynthComponent
	{
		private const float Dt = 1f / 44100.0f;

		private float lastOutput;
		private float lastInput;

		public ControlInput CutoffFrequency { get; } = new ControlInput { BaseValue = 440 };

		public ISynthComponent Input { get; set; }

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

			var input = Input.GenerateSamples(count, timeBase);
			var cutoff = CutoffFrequency.GenerateSamples(count, timeBase);

			for (var i = 0; i < count; i++)
			{
				var rc = 1f / (float)(cutoff[i] * 2 * Math.PI);
				var alpha = Dt / (rc + Dt);

				// Low pass filter.
//				lastOutput = lastOutput + alpha * (input[i] - lastOutput);

				// High pass filter.
				lastOutput = alpha * (lastOutput + input[i] - lastInput);

				buffer[i] = lastOutput;
				lastInput = input[i];
			}
		}

		#endregion
	}
}