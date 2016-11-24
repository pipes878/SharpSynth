using System;
using System.Collections.Generic;

namespace SharpSynth
{
	public class Mixer : SynthComponent
	{
		public List<ISynthComponent> Inputs { get; }

		public Mixer()
		{
			Inputs = new List<ISynthComponent>();
		}

		#region Implementation of ISynthComponent

		/// <summary>
		/// Generate synthesizer samples
		/// </summary>
		/// <param name="buffer">The buffer to generate into.</param>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples, which is measured at 44100 samples per second.</param>
		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			if (Inputs.Count == 0)
			{
				for (var i = 0; i < count; i++)
					buffer[i++] = 0;
				return;
			}

			Array.Copy(Inputs[0].GenerateSamples(count, timeBase), buffer, count);

			if (Inputs.Count == 1)
				return;

			for (int i = 1; i < Inputs.Count; i++)
			{
				var samples = Inputs[i].GenerateSamples(count, timeBase);

				for (var j = 0; j < count; j++)
				{
					buffer[j] += samples[j];
				}
			}
		}

		#endregion
	}
}