using System;

namespace SharpSynth
{
	public enum FilterType
	{
		HighPass,
		LowPass,
		BandPass,
		Notch
	}

	public class Filter : SynthComponent
	{
		private const float Dt = 1f / 44100.0f;

		private float damp;
		private float lowPass;
		private float bandPass;
		private float highPass;
		private float notch;

		private float lastOutput;
		private float lastInput;

		public FilterType FilterType { get; set; }

		public ControlInput CornerFrequency { get; } = new ControlInput { BaseValue = 440 };

		public ISynthComponent Input { get; set; }

		public Filter(float q = 5)
		{
			damp = 1 / q;
		}

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
					buffer[i++] = 0;

				lastOutput = 0;
				return;
			}

			var input = Input.GenerateSamples(count, timeBase);
			var cutoff = CornerFrequency.GenerateSamples(count, timeBase);

			for (var i = 0; i < count; i++)
			{
				var index = (int)((Tables.TableSize / 2f) * (cutoff[i] / 44100f));
				if (index < 0)
					index = 0;
				else if (index > Tables.TableSize / 2)
					index = Tables.TableSize / 2;
				var fc = Tables.Sin[index];

				lowPass = lowPass + fc * bandPass;
				highPass = (input[i] - lowPass) - (damp * bandPass);
				bandPass = fc * highPass + bandPass;
				notch = highPass + lowPass;

				switch (FilterType)
				{
					case FilterType.LowPass:
						buffer[i] = lowPass;
						break;
					case FilterType.HighPass:
						buffer[i] = highPass;
						break;
					case FilterType.BandPass:
						buffer[i] = bandPass;
						break;
					case FilterType.Notch:
						buffer[i] = notch;
						break;

					default:
						buffer[i] = 0;
						break;
				}
			}

			//if (FilterType == FilterType.HighPass)
			//{
			//	for (var i = 0; i < count; i++)
			//	{
			//		var rc = 1f / (float)(cutoff[i] * 2 * Math.PI);
			//		var alpha = Dt / (rc + Dt);

			//		// High pass filter.
			//		lastOutput = alpha * (lastOutput + input[i] - lastInput);
			//		buffer[i] = lastOutput;
			//		lastInput = input[i];
			//	}
			//}
			//else
			//{
			//	for (var i = 0; i < count; i++)
			//	{
			//		var rc = 1f / (float)(cutoff[i] * 2 * Math.PI);
			//		var alpha = Dt / (rc + Dt);

			//		// Low pass filter.
			//		lastOutput = lastOutput + alpha * (input[i] - lastOutput);
			//		buffer[i] = lastOutput;
			//		lastInput = input[i];
			//	}
			//}
		}

		#endregion
	}
}