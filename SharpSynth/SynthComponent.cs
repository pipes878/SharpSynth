namespace SharpSynth
{
	public abstract class SynthComponent
	{
		private long generatedTimeBase;
		private float[] generatedBuffer;

		/// <summary>
		/// Process the synth component
		/// </summary>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples, which is measured at 44100 samples per second.</param>
		/// <returns>The generated samples.</returns>
		public float[] GenerateSamples(int count, long timeBase)
		{
			if (generatedTimeBase != timeBase || generatedBuffer == null || generatedBuffer.Length < count)
			{
				if (generatedBuffer == null || generatedBuffer.Length < count)
					generatedBuffer = new float[count];

				GenerateSamples(generatedBuffer, count, timeBase);
				generatedTimeBase = timeBase;
			}

			return generatedBuffer;
		}

		/// <summary>
		/// Generate synthesizer samples.
		/// </summary>
		/// <param name="buffer">The buffer to generate into.</param>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples, which is measured at 44100 samples per second.</param>
		protected abstract void GenerateSamples(float[] buffer, int count, long timeBase);
	}
}