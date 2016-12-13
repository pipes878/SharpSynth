namespace SharpSynth
{
	public interface ISynthComponent
	{
		/// <summary>
		/// Generate samples from the synth component.
		/// </summary>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples.</param>
		/// <returns>The generated samples.</returns>
		float[] GenerateSamples(int count, long timeBase);
	}
}