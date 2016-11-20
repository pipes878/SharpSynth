using System;
using System.Net.Mail;

namespace SharpSynth
{
	/// <summary>
	/// Generates a basic linear envelope.
	/// </summary>
	/// <remarks>
	/// This is a basic envelope generator, the attack will go from 0 to 1, at the rate specified in the Attack parameter. A value of 1 will take 1 second to finish the attack phase.
	/// Once attack is hit, it will decay at the rate specified in the Decay parameter.
	/// Sustain is the level that the envelope will sit at while the input is still above the threshold.
	/// Release is the rate at which the level will head towards 0.
	/// </remarks>
	public class EnvelopeGenerator : SynthComponent
	{
		private enum Phase
		{
			Idle,
			Attack,
			Decay,
			Sustain,
			Release
		}

		private Phase phase = Phase.Idle;
		private float lastInputValue;
		private float lastOutputValue;
		private long? triggerActivationTime;


		/// <summary>
		/// The threshold that must be crossed for the envelope to activate.
		/// </summary>
		public ControlInput TriggerThreshold { get; } = new ControlInput { BaseValue = 0.5f };

		/// <summary>
		/// Should the trigger detect rising or falling edges on the input signal?
		/// </summary>
		public bool TriggerOnFallingEdge { get; set; }

		/// <summary>
		/// The time the envelope will take to go from 0 to 1 in seconds. This value will clamp at 0.
		/// </summary>
		public ControlInput Attack { get; } = new ControlInput { BaseValue = .05f };

		/// <summary>
		/// The time the envelope will take to go from 1 to sustain level in seconds. This value will clamp at 0.
		/// </summary>
		public ControlInput Decay { get; } = new ControlInput { BaseValue = .1f };

		/// <summary>
		/// The level that the envelope will sustain at. Note that this value can be above 1 to have a sustain value above attack.
		/// </summary>
		public ControlInput Sustain { get; } = new ControlInput { BaseValue = .5f };

		/// <summary>
		/// The time that the envelopw will take to release. Note that the rate of release is determined by the distance from sustain to 0.
		/// </summary>
		public ControlInput Release { get; } = new ControlInput { BaseValue = .3f };

		/// <summary>
		/// The input used to trigger the envelope.
		/// </summary>
		public SynthComponent Input { get; set; }

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
				lastInputValue = 0;
				return;
			}

			var triggerTreshold = TriggerThreshold.GenerateSamples(count, timeBase);
			var sampleData = Input.GenerateSamples(count, timeBase);
			var attack = Attack.GenerateSamples(count, timeBase);
			var decay = Decay.GenerateSamples(count, timeBase);
			var sustain = Sustain.GenerateSamples(count, timeBase);
			var release = Release.GenerateSamples(count, timeBase);

			for (var i = 0; i < count; i++)
			{
				if (phase == Phase.Idle || phase == Phase.Release)
				{
					// look for trigger.

					if (((lastInputValue < triggerTreshold[i]) ^ TriggerOnFallingEdge) && ((sampleData[i] >= triggerTreshold[i]) ^ TriggerOnFallingEdge))
						phase = Phase.Attack;
				}
				else
				{
					// Look for release.

					if (((lastInputValue > triggerTreshold[i]) ^ TriggerOnFallingEdge) && ((sampleData[i] <= triggerTreshold[i]) ^ TriggerOnFallingEdge))
						phase = Phase.Release;
				}

				switch (phase)
				{
					case Phase.Idle:
						buffer[i] = 0;
						break;
					case Phase.Attack:
					{
						var inc = attack[i] * 44100.0f;
						if (inc <= 1)
							buffer[i] = 1f;
						else
							buffer[i] = lastOutputValue + 1f / inc;

						if (buffer[i] >= 1f)
						{
							buffer[i] = 1f;
							phase = Phase.Decay;
						}
						break;
					}

					case Phase.Decay:
					{
						var inc = (1f - sustain[i]) * decay[i] * 44100.0f;
						if (Math.Abs(inc) <= 1)
							buffer[i] = sustain[i];
						else
							buffer[i] = lastOutputValue - 1f / inc;

						if (inc < 0)
						{
							if (buffer[i] > sustain[i])
							{
								buffer[i] = sustain[i];
								phase = Phase.Sustain;
							}
						}
						else
						{
							if (buffer[i] < sustain[i])
							{
								buffer[i] = sustain[i];
								phase = Phase.Sustain;
							}
						}

						break;
					}

					case Phase.Sustain:
					{
						buffer[i] = sustain[i];
						break;
					}

					case Phase.Release:
					{
						var inc = sustain[i] * release[i] * 44100.0f;
						if (inc <= 1)
							buffer[i] = 0f;
						else
							buffer[i] = lastOutputValue - 1f/inc;

						if (buffer[i] <= 0f)
						{
							buffer[i] = 0f;
							phase = Phase.Idle;
						}

						break;
					}
				}
				lastOutputValue = buffer[i];
				lastInputValue = sampleData[i];
			}

		}
	}
}