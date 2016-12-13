namespace SharpSynth.Input
{
	/// <summary>
	/// Generates samples of a fixed value. Use to hard set an input value on a component.
	/// </summary>
	public class FixedValue : ISynthComponent
	{
		/// <summary>
		/// Generates a fixed value of 1.
		/// </summary>
		public static readonly FixedValue One = new FixedValue(1);

		/// <summary>
		/// Generates a fixed value of 0.5.
		/// </summary>
		public static readonly FixedValue Half = new FixedValue(0.5f);

		/// <summary>
		/// Generates a fixed value of 0.
		/// </summary>
		public static readonly FixedValue Zero = new FixedValue(0);

		#region Fields

		private readonly float value;
		private float[] buffer;

		#endregion

		#region Construction

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="value">The value of the samples to generate.</param>
		public FixedValue(float value)
		{
			this.value = value;
		}

		#endregion

		#region ISynthComponent Members

		/// <summary>
		/// Generate samples from the synth component.
		/// </summary>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples.</param>
		/// <returns>The generated samples.</returns>
		public float[] GenerateSamples(int count, long timeBase)
		{
			if (buffer == null || buffer.Length < count)
			{
				buffer = new float[count];
				for (var i = 0; i < count; i++)
					buffer[i] = value;
			}

			return buffer;
		}

		#endregion
	}
}