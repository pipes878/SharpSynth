using System.Xml;
using SharpSynth.Input;

namespace SharpSynth
{
	/// <summary>
	/// Generates a trigger pulse on the rising or falling edge of the input signal, with the threshold and trigger length defined as control inputs.
	/// </summary>
	public class TriggerGenerator : SynthComponent
	{
		private float lastValue;
		private long? triggerActivationTime;

		/// <summary>
		/// Should the trigger detect rising or falling edges on the input signal?
		/// </summary>
		public bool TriggerOnFallingEdge { get; set; }

		/// <summary>
		/// The threshold that must be crossed for the trigger to activate.
		/// </summary>
		public ControlValue TriggerThreshold { get; } = new ControlValue { BaseValue = 0.5f };

		/// <summary>
		/// The length in seconds that the trigger should last.
		/// </summary>
		public ControlValue TriggerLength { get; } = new ControlValue { BaseValue = 0.1f };

		public SynthComponent Input { get; set; }

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
				lastValue = 0;
				return;
			}

			var triggerTreshold = TriggerThreshold.GenerateSamples(count, timeBase);
			var triggerLength = TriggerLength.GenerateSamples(count, timeBase);
			var sampleData = Input.GenerateSamples(count, timeBase);

			for (var i = 0; i < count;)
			{
				if (triggerActivationTime != null)
				{
					while (timeBase - triggerActivationTime < triggerLength[i] * 44100)
					{
						buffer[i] = 1;
						timeBase++;
						i++;
						if (i == count)
						{
							lastValue = sampleData[count - 1];
							return;
						}
					}

					lastValue = sampleData[i];
					triggerActivationTime = null;
				}

				while (i < count)
				{
					if (((lastValue < triggerTreshold[i]) ^ TriggerOnFallingEdge) && ((sampleData[i] >= triggerTreshold[i]) ^ TriggerOnFallingEdge))
					{
						triggerActivationTime = timeBase;
						break;
					}
					buffer[i] = 0;

					lastValue = sampleData[i];
					i++;
				}
			}

			lastValue = sampleData[count - 1];
		}

		#endregion
	}
}