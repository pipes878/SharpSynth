namespace SharpSynth
{
	public abstract class SynthComponent : ISynthComponent
	{
		public static int SampleRate => SynthToSampleProvider.SampleRate;

		private long generatedTimeBase;
		private float[] generatedBuffer;
		private int generatedCount;

		/// <summary>
		/// Process the synth component
		/// </summary>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples.</param>
		/// <returns>The generated samples.</returns>
		public float[] GenerateSamples(int count, long timeBase)
		{
			if (generatedTimeBase != timeBase || generatedBuffer == null || generatedBuffer.Length < count || generatedCount < count)
			{
				if (generatedBuffer == null || generatedBuffer.Length < count)
					generatedBuffer = new float[count];

				GenerateSamples(generatedBuffer, count, timeBase);
				generatedTimeBase = timeBase;
				generatedCount = count;
			}

			return generatedBuffer;
		}

		/// <summary>
		/// Generate synthesizer samples.
		/// </summary>
		/// <param name="buffer">The buffer to generate into.</param>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples.</param>
		protected abstract void GenerateSamples(float[] buffer, int count, long timeBase);
	}
}