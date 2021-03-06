﻿namespace SharpSynth.Input
{
	/// <summary>
	/// Generates a controllable value with a configurable base. The value produced is <see cref="BaseValue"/> + <see cref="Gain"/> * <see cref="Control"/>.
	/// </summary>
	public class ControlValue : ISynthComponent
	{
		#region Fields

		private long generatedTimeBase;
		private float[] generatedBuffer;
		private int generatedCount;
		private float? generatedBaseValue;

		#endregion

		#region Properties

		/// <summary>
		/// Added to the input after gain is applied. If there is no control input, this is the output produced.
		/// </summary>
		public float BaseValue { get; set; } = 0f;

		/// <summary>
		/// Gain on the input, if it is connected.
		/// </summary>
		public float Gain { get; set; } = 1f;

		/// <summary>
		/// The control input data. This is optional, and if not set, the value used will be <see cref="BaseValue"/>
		/// </summary>
		public ISynthComponent Control { get; set; }

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
			if (generatedTimeBase != timeBase || generatedBuffer == null || generatedCount < count || generatedBaseValue != BaseValue)
			{
				if (generatedBuffer == null || generatedBuffer.Length < count)
				{
					generatedBuffer = new float[count];
					generatedBaseValue = null;
				}

				if (Control == null)
				{
					if (generatedBaseValue != BaseValue)
					{
						for (var i = 0; i < generatedBuffer.Length; i++)
							generatedBuffer[i] = BaseValue;

						generatedBaseValue = BaseValue;
						generatedCount = generatedBuffer.Length;
					}
				}
				else
				{
					var controlBuffer = Control.GenerateSamples(count, timeBase);
					for (var i = 0; i < generatedBuffer.Length; i++)
						generatedBuffer[i] = controlBuffer[i] * Gain + BaseValue;
					generatedTimeBase = timeBase;
					generatedBaseValue = null;
					generatedCount = count;
				}
			}

			return generatedBuffer;
		}

		#endregion
	}
}