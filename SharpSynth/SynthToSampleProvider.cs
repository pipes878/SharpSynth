using System;
using NAudio.Wave;

namespace SharpSynth
{
	public class SynthToSampleProvider : ISampleProvider
	{
		private long timeBase;

		public SynthComponent SynthInput { get; set; }

		public SynthToSampleProvider()
		{
			WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
		}

		public SynthToSampleProvider(SynthComponent synthInput) : this()
		{
			SynthInput = synthInput;
		}

		public void ResetTime()
		{
			timeBase = 0;
		}

		#region Implementation of ISampleProvider

		/// <summary>
		/// Fill the specified buffer with 32 bit floating point samples
		/// </summary>
		/// <param name="buffer">The buffer to fill with samples.</param>
		/// <param name="offset">Offset into buffer</param>
		/// <param name="count">The number of samples to read</param>
		/// <returns>the number of samples written to the buffer.</returns>
		public int Read(float[] buffer, int offset, int count)
		{
			var t = timeBase;
			timeBase += count;

			if (SynthInput == null)
			{
				var c = count;
				for (var i = offset; c > 0; c--)
				{
					buffer[i] = 0;
				}
			}
			else
			{
				Array.Copy(SynthInput.GenerateSamples(count, t), 0, buffer, offset, count);
			}

			return count;
		}

		/// <summary>Gets the WaveFormat of this Sample Provider.</summary>
		/// <value>The wave format.</value>
		public WaveFormat WaveFormat { get; }

		#endregion
	}
}