using System;
using SharpSynth.Input;

namespace SharpSynth
{
	/// <summary>
	/// The different types of clipping supported.
	/// </summary>
	public enum ClippingType
	{
		/// <summary>
		/// Hard clipping, clamps the input to the threshold.
		/// </summary>
		Hard,
		/// <summary>
		/// Soft clipping, begins shaping the input once it goes beyond the threshold, and 
		/// </summary>
		Soft
	}

	/// <summary>
	/// Clips the input when it sits +/- the specified threshold.
	/// </summary>
	public class Clipper : SynthComponent
	{
		#region Properties

		/// <summary>
		/// The clip threshold. This value is clamped between 0 and 1.
		/// </summary>
		public ISynthComponent Threshold { get; } = FixedValue.One;

		/// <summary>
		/// The clipping type.
		/// </summary>
		public ClippingType ClippingType { get; set; }

		/// <summary>
		/// Input for the clipper.
		/// </summary>
		public ISynthComponent Input { get; set; }

		#endregion

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

			var threshold = Threshold.GenerateSamples(count, timeBase);
			var input = Input.GenerateSamples(count, timeBase);

			switch (ClippingType)
			{
				case ClippingType.Hard:
					for (var i = 0; i < count; i++)
					{
						var t = Math.Max(0, Math.Min(threshold[i], 1));
						buffer[i] = Math.Max(-t, Math.Min(t, input[i]));
					}
					break;

				case ClippingType.Soft:
					for (var i = 0; i < count; i++)
					{
						var t = Math.Max(0, Math.Min(threshold[i], 1));
						var absIn = Math.Abs(input[i]);
						if (absIn <= t)
							buffer[i] = t;
						else
						{
							buffer[i] = (t * t - t + absIn) / input[i];
						}
					}
					break;

				default:
					Array.Copy(input, buffer, count);
					break;
			}
		}

		#endregion
	}
}