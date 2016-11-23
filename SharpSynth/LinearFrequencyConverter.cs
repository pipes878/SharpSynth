using System;

namespace SharpSynth
{
	/// <summary>
	/// Converts a linear value to a frequency value, where 1 unit = 1 octave.
	/// </summary>
	public class LinearFrequencyConverter : SynthComponent
	{
		public ControlInput BaseFrequency { get; } = new ControlInput();

		public ISynthComponent Input { get; set; }

		public LinearFrequencyConverter()
		{
		}

		public LinearFrequencyConverter(float baseFrequencyValue)
		{
			BaseFrequency.BaseValue = baseFrequencyValue;
		}

		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			if (Input == null)
			{
				for (var i = 0; i < count; i++)
					buffer[i] = 0;
				return;
			}

			var frequencies = BaseFrequency.GenerateSamples(count, timeBase);
			var input = Input.GenerateSamples(count, timeBase);

			for (var i = 0; i < count; i++)
			{
				buffer[i] = (float)(frequencies[i] * Math.Pow(2, input[i]));
			}
		}
	}
}