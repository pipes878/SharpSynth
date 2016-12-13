using System;
using NAudio.Wave;

namespace SharpSynth
{
	public class SynthToSampleProvider : ISampleProvider
	{
		public static readonly int SampleRate = 48000;

		private long timeBase;

		public ISynthComponent SynthInput { get; set; }

		public SynthToSampleProvider()
		{
			WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 2);
		}

		public SynthToSampleProvider(ISynthComponent synthInput) : this()
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
			timeBase += count / 2;

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
				var synthSamples = SynthInput.GenerateSamples(count / 2, t);
				for (var i = 0; i < count; i += 2)
				{
					buffer[offset + i] = buffer[offset + i + 1] = synthSamples[i / 2];
				}
			}

			return count;
		}

		/// <summary>Gets the WaveFormat of this Sample Provider.</summary>
		/// <value>The wave format.</value>
		public WaveFormat WaveFormat { get; }

		#endregion
	}
}