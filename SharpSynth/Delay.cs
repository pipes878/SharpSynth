using System;
using SharpSynth.Input;

namespace SharpSynth
{
	public class BbdDelay : SynthComponent
	{
		private const float minimumDelay = 0.2f;
		private const float maximumDelay = 1f;
		private float lastBufferedSample;
		private float head;
		private float[] bbdBuffer;

		public ISynthComponent DelayTime { get; set; } = FixedValue.One;

		public ISynthComponent Mix { get; set; } = FixedValue.Half;

		public ISynthComponent Feedback { get; set; } = FixedValue.Half;

		public ISynthComponent Input { get; set; }

		public BbdDelay()
		{
			// This gives 1 second of day with the slowest clock.

			bbdBuffer = new float[SampleRate];
		}

		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			if (Input == null)
			{
				for (var i = 0; i < buffer.Length; i++)
					buffer[i] = 0;
				return;
			}

			var delay = DelayTime.GenerateSamples(count, timeBase);
			var mix = Mix.GenerateSamples(count, timeBase);
			var feedback = Feedback.GenerateSamples(count, timeBase);
			var input = Input.GenerateSamples(count, timeBase);

			for (var i = 0; i < count; i++)
			{
				var d = Math.Min(maximumDelay, Math.Max(minimumDelay, delay[i]));
				var m = Math.Min(1, Math.Max(0, mix[i]));
				var f = Math.Min(1, Math.Max(0, feedback[i]));

				var s = ReplaceDelayedSample(input[i], d, f);
			}
		}

		private float ReplaceDelayedSample(float input, float delay, float feedback)
		{
			// This works because the BBD is 1 second long, in samples.

			var ticksPerSample = 1 / delay;

			// First read the output of the BBD.

			//bbdBuffer[head]

			return 0;
		}
	}

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
		public ISynthComponent FeedbackAmount { get; set; } = FixedValue.Half;

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
		/// <param name="timeBase">The time base for the new samples. This value is in samples.</param>
		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			if (Input == null)
			{
				for (var i = 0; i < count; i++)
					buffer[i] = 0;
				return;
			}

			var delayAmount = (int)(DelayAmount * SampleRate);
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