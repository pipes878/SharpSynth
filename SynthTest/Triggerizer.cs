using SharpSynth;

namespace SynthTest
{
	public class Triggerizer : SynthComponent
	{
		public bool IsTriggered { get; set; }

		#region Overrides of SynthComponent

		/// <summary>
		/// Generate synthesizer samples.
		/// </summary>
		/// <param name="buffer">The buffer to generate into.</param>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples.</param>
		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			var value = IsTriggered ? 1f : 0f;

			for (var i = 0; i < count; i++)
				buffer[i] = value;
		}

		#endregion
	}
}