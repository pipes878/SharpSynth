using System;

namespace SharpSynth
{
	public class Noise : SynthComponent
	{
		private readonly Random r = new Random();

		#region Overrides of SynthComponent

		/// <summary>
		/// Generate synthesizer samples.
		/// </summary>
		/// <param name="buffer">The buffer to generate into.</param>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples, which is measured at 44100 samples per second.</param>
		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			for (var i = 0; i < count; i++)
				buffer[i] = (float)r.NextDouble() * 2f - 1f;
		}

		#endregion
	}
}