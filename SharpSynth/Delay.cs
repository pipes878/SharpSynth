using System;
using SharpSynth.Input;

namespace SharpSynth
{
	public class Delay : SynthComponent
	{
		private int head;
		private float[] feedbackBuffer;

		/// <summary>
		/// The delay amount in seconds.
		/// </summary>
		public float DelayAmount { get; set; } = 0.2f;

		/// <summary>
		/// The feedback amount.
		/// </summary>
		public ControlValue FeedbackAmount { get; } = new ControlValue { BaseValue = .5f };

		/// <summary>
		/// 
		/// </summary>
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

			var delayAmount = (int)(DelayAmount * 44100.0);
			if (delayAmount < 0)
				delayAmount = 0;

			var desiredBufferSize = delayAmount + count;

			if (feedbackBuffer == null || feedbackBuffer.Length < desiredBufferSize)
			{
				var newBuffer = new float[desiredBufferSize];
				if (feedbackBuffer != null)
					Array.Copy(feedbackBuffer, newBuffer, feedbackBuffer.Length);
				feedbackBuffer = newBuffer;
			}

			var tail = head - delayAmount;
			if (tail < 0)
				tail = feedbackBuffer.Length + tail;

			var input = Input.GenerateSamples(count, timeBase);
			var feedback = FeedbackAmount.GenerateSamples(count, timeBase);

			for (var i = 0; i < count; i++)
			{
				buffer[i] = input[i] + feedbackBuffer[tail];
				feedbackBuffer[head] = buffer[i] * feedback[i];

				tail = (tail + 1) % feedbackBuffer.Length;
				head = (head + 1) % feedbackBuffer.Length;
			}
		}

		#endregion
	}
}