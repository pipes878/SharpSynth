using System;

namespace SharpSynth.Input
{
	/// <summary>
	/// Creates a controllable range input, taking input from 0 to 1 and converting it to an output between min and max.
	/// </summary>
	public class RangeValue : ISynthComponent
	{
		#region Fields

		private readonly float min;
		private readonly float max;
		private float? outputValue;
		private float[] buffer;
		private float value;

		#endregion

		#region Properties

		/// <summary>
		/// The value of the range input. This value is clamped between 0 and 1.
		/// </summary>
		public float Value
		{
			get { return value; }
			set
			{
				this.value = Math.Max(0, Math.Min(value, 1));
				outputValue = null;
			}
		}

		#endregion

		#region Construction

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min">The minimum value of the range output.</param>
		/// <param name="max">The maximum value of the range output.</param>
		public RangeValue(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		#endregion

		#region ISynthComponent Members

		/// <summary>
		/// Generate samples from the synth component.
		/// </summary>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples, which is measured at 44100 samples per second.</param>
		/// <returns>The generated samples.</returns>
		public float[] GenerateSamples(int count, long timeBase)
		{
			if (buffer == null || buffer.Length < count)
			{
				buffer = new float[count];
				outputValue = null;
			}

			if (!outputValue.HasValue)
			{
				var v = min + value * (max - min);
				for (var i = 0; i < count; i++)
					buffer[i] = v;
				outputValue = v;
			}

			return buffer;
		}

		#endregion
	}
}