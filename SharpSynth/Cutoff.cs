using System;

namespace SharpSynth
{
	public class Cutoff : SynthComponent
	{
		public ControlInput CutoffThreshold { get; } = new ControlInput();

		public bool CutBelow { get; set; }

		public ISynthComponent Input { get; set; }

		#region Overrides of SynthComponent

		/// <summary>
		/// Generate synthesizer samples.
		/// </summary>
		/// <param name="buffer">The buffer to generate into.</param>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples, which is measured at 44100 samples per second.</param>
		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			if (Input == null)
			{
				for (var i = 0; i < count; i++)
					buffer[i] = 0;
				return;
			}

			var cutoff = CutoffThreshold.GenerateSamples(count, timeBase);
			var input = Input.GenerateSamples(count, timeBase);

			if (CutBelow)
			{
				for (var i = 0; i < count; i++)
					buffer[i] = Math.Max(cutoff[i], input[i]);
			}
			else
			{
				for (var i = 0; i < count; i++)
					buffer[i] = Math.Min(cutoff[i], input[i]);
			}
		}

		#endregion
	}
}