using System;

namespace SharpSynth
{
	public class CombFilter : SynthComponent
	{
		private const float ReverbTimeDesired = 1;

		private float g;
		private float[] delayBuffer;
		private int pos;

		public CombFilter(float delayTime)
		{
			g = (float)Math.Pow(0.001, delayTime / ReverbTimeDesired);
			delayBuffer = new float[(int)(delayTime * 44100)];
		}

		/// <summary>
		/// The input to the reverb.
		/// </summary>
		public ISynthComponent Input { get; set; }

		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			if (Input == null)
			{
				for (var i = 0; i < buffer.Length; i++)
					buffer[i] = 0;
				return;
			}

			var input = Input.GenerateSamples(count, timeBase);
			for (var i = 0; i < count; i++)
			{
				var output = delayBuffer[pos];
				delayBuffer[pos] = input[i] + output * g;
				buffer[i] = output;
				pos = (pos + 1) % delayBuffer.Length;
			}
		}
	}
}