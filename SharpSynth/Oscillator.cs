using System;
using System.Collections.Generic;

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
		private const int TableBits = 20;
		private const int TableSize = 1 << TableBits;
		private const int TableMask = TableSize - 1;

		private static readonly Dictionary<OscillatorShape, float[]> tables = new Dictionary<OscillatorShape, float[]>();

		static Oscillator()
		{
			var sinTable = new float[TableSize];
			var squareTable = new float[TableSize];
			var rampTable = new float[TableSize];
			var sawTable = new float[TableSize];
			var triangleTable = new float[TableSize];

			for (var i = 0; i < TableSize; i++)
			{
				sinTable[i] = (float)Math.Sin(2 * Math.PI * i / TableSize);
				squareTable[i] = 2 * (1f - (i >> (TableBits - 1))) - 1;
				rampTable[i] = 2f * i / (TableSize-1) - 1;
				sawTable[i] = -rampTable[i];
				var result = 2f * ((i * 2) & TableMask) / TableSize - 1;
				triangleTable[i] = i * 2 >= TableSize ? -result : result;
			}

			tables[OscillatorShape.Sin] = sinTable;
			tables[OscillatorShape.Square] = squareTable;
			tables[OscillatorShape.Ramp] = rampTable;
			tables[OscillatorShape.Saw] = sawTable;
			tables[OscillatorShape.Triangle] = triangleTable;
		}

		private double angle;

		/// <summary>
		/// The shape of the oscillator.
		/// </summary>
		public OscillatorShape Shape { get; set; } = OscillatorShape.Sin;

		/// <summary>
		/// Frequency value in Hz.
		/// </summary>
		public ControlInput Frequency { get; } = new ControlInput { BaseValue = 0 };

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
			const double angleInc = TableSize / 44100.0;

			float[] table;
			if (!tables.TryGetValue(Shape, out table))
			{
				for (var i = 0; i < count; i++)
					buffer[i] = 0;
				return;
			}

			var frequency = Frequency.GenerateSamples(count, timeBase);
			var level = Level.GenerateSamples(count, timeBase);
			var scale = Scale.GenerateSamples(count, timeBase);
			var phaseOffset = PhaseOffset.GenerateSamples(count, timeBase);

			for (var i = 0; i < count; i++)
			{
				buffer[i] = level[i] + table[(int)(phaseOffset[i] * TableSize + angle) & TableMask] * scale[i];
				angle += frequency[i] * angleInc;
				if (angle > TableSize)
					angle -= TableSize;
			}

			while (angle > TableSize)
				angle -= TableSize;
		}

		#endregion
	}
}