using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpSynth
{
	public class StepSequencer : SynthComponent
	{
		private int currentControl = -1;
		private readonly ControlInput[] controlValues;
		private float lastTriggerInput;
		private readonly float[][] generatedControlValues;

		public ControlInput TriggerSource { get; } = new ControlInput();

		public ControlInput TriggerThreshold { get; } = new ControlInput { BaseValue = 0.5f };

		public ReadOnlyCollection<ControlInput> ControlValues { get; }

		public StepSequencer(int stepCount)
		{
			controlValues = new ControlInput[stepCount];
			for (var i = 0; i < stepCount; i++)
				controlValues[i] = new ControlInput();

			ControlValues = new ReadOnlyCollection<ControlInput>(controlValues);
			generatedControlValues = new float[stepCount][];
		}

		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			var triggerData = TriggerSource.GenerateSamples(count, timeBase);
			var triggerThreshold = TriggerThreshold.GenerateSamples(count, timeBase);
			for (var i = 0; i < controlValues.Length; i++)
				generatedControlValues[i] = controlValues[i].GenerateSamples(count, timeBase);

			for (var i = 0; i < count; i++)
			{
				if (lastTriggerInput < triggerThreshold[i] && triggerData[i] > triggerThreshold[i])
					currentControl = (currentControl + 1) % controlValues.Length;

				if (currentControl < 0)
					buffer[i] = 0;
				else
					buffer[i] = generatedControlValues[currentControl][i];

				lastTriggerInput = triggerData[i];
			}
		}
	}
}