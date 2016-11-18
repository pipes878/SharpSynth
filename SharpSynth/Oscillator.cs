using System;

namespace SharpSynth
{
	public enum OscillatorShape
	{
		Sin,
		Square,
		Ramp,
		Saw,
		Triangle
	}

	public class Oscillator : SynthComponent
	{
		private const int TableBits = 18;
		private const int TableSize = 1 << TableBits;
		private static readonly float[] sinTable;

		static Oscillator()
		{
			sinTable = new float[TableSize];
			for (int i = 0; i < TableSize; i++)
			{
				sinTable[i] = (float)Math.Sin(2 * Math.PI * i / TableSize);
			}
		}

		private double angle;

		/// <summary>
		/// The shape of the oscillator.
		/// </summary>
		public OscillatorShape Shape { get; set; } = OscillatorShape.Sin;

		/// <summary>
		/// Frequency value in Hz.
		/// </summary>
		public ControlInput Frequency { get; } = new ControlInput { BaseValue = 440 };

		/// <summary>
		/// The signal will oscillate evenly around this point. Default is 0.
		/// </summary>
		public ControlInput Level { get; } = new ControlInput { BaseValue = 0 };

		/// <summary>
		/// Scale value of the oscillator will multiply the output value after <see cref="Level"/> is applied. Default is 1.
		/// </summary>
		public ControlInput Scale { get; } = new ControlInput { BaseValue = 1 };

		/// <summary>
		/// The phase offset of the waveform. A value of 1.0 is a full phase, 0.5 is half phase.
		/// </summary>
		public ControlInput PhaseOffset { get; } = new ControlInput { BaseValue = 0 };

		#region Implementation of ISynthComponent

		/// <summary>
		/// Generate synthesizer samples
		/// </summary>
		/// <param name="buffer">The buffer to generate into.</param>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples, which is measured at 44100 samples per second.</param>
		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			var frequency = Frequency.GenerateSamples(count, timeBase);
			var level = Level.GenerateSamples(count, timeBase);
			var scale = Scale.GenerateSamples(count, timeBase);
			var phaseOffset = PhaseOffset.GenerateSamples(count, timeBase);

			for (var i = 0; i < count; i++)
			{
				buffer[i] = level[i] + GenerateSample((int)(phaseOffset[i] * TableSize + angle) & (TableSize - 1)) * scale[i];
				angle += frequency[i] * TableSize / 44100.0;
				if (angle > TableSize)
					angle -= TableSize;
			}

			while (angle > TableSize)
				angle -= TableSize;
		}

		public float GenerateSample(int a)
		{
			switch (Shape)
			{
				case OscillatorShape.Sin:
					return sinTable[a];
				case OscillatorShape.Square:
					return 2 * (1f - (a >> (TableBits - 1))) - 1;
				case OscillatorShape.Ramp:
					return (2f * a / TableSize) - 1;
				case OscillatorShape.Saw:
					return -((2f * a / TableSize) - 1);
				case OscillatorShape.Triangle:
					a *= 2;
					var result = ((2f * (a & (TableSize - 1)) / TableSize) - 1);
					return (a >= TableSize) ? -result : result;
				default:
					return 0;
			}
		}

		#endregion
	}
}