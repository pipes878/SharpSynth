using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SharpSynth.Input;

namespace SharpSynth
{
	public class StepSequencer : SynthComponent
	{
		private int currentControl = -1;
		private readonly ControlValue[] controlValues;
		private float lastTriggerInput;
		private readonly float[][] generatedControlValues;

		public ControlValue TriggerSource { get; } = new ControlValue();

		public ControlValue TriggerThreshold { get; } = new ControlValue { BaseValue = 0.5f };

		public ReadOnlyCollection<ControlValue> ControlValues { get; }

		public StepSequencer(int stepCount)
		{
			controlValues = new ControlValue[stepCount];
			for (var i = 0; i < stepCount; i++)
				controlValues[i] = new ControlValue();

			ControlValues = new ReadOnlyCollection<ControlValue>(controlValues);
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