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
		private static readonly Dictionary<OscillatorShape, float[]> ShapeTables = new Dictionary<OscillatorShape, float[]>();

		static Oscillator()
		{
			var sawTable = new float[Tables.TableSize];

			for (var i = 0; i < Tables.TableSize; i++)
			{
				sawTable[i] = -Tables.Ramp[i];
			}

			ShapeTables[OscillatorShape.Sin] = Tables.Sin;
			ShapeTables[OscillatorShape.Ramp] = Tables.Ramp;
			ShapeTables[OscillatorShape.Saw] = sawTable;
			ShapeTables[OscillatorShape.Triangle] = Tables.Triangle;
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

		/// <summary>
		/// PWM control for the square wave.
		/// </summary>
		public ControlInput SquarePwm { get; } = new ControlInput { BaseValue = 0.5f };

		#region Implementation of ISynthComponent

		/// <summary>
		/// Generate synthesizer samples
		/// </summary>
		/// <param name="buffer">The buffer to generate into.</param>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples, which is measured at 44100 samples per second.</param>
		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			const double AngleInc = Tables.TableSize / 44100.0;

			float[] table;
			if (!ShapeTables.TryGetValue(Shape, out table) && Shape != OscillatorShape.Square)
			{
				for (var i = 0; i < count; i++)
					buffer[i] = 0;
				return;
			}

			var frequency = Frequency.GenerateSamples(count, timeBase);
			var level = Level.GenerateSamples(count, timeBase);
			var scale = Scale.GenerateSamples(count, timeBase);
			var phaseOffset = PhaseOffset.GenerateSamples(count, timeBase);

			if (Shape == OscillatorShape.Square)
			{
				var pwm = SquarePwm.GenerateSamples(count, timeBase);
				for (var i = 0; i < count; i++)
				{
					var a = (int)(phaseOffset[i] * Tables.TableSize + angle) & Tables.TableMask;
					var pw = pwm[i] * Tables.TableSize;
					buffer[i] = a < pw ? 1 : 0;

					angle += frequency[i] * AngleInc;
					if (angle > Tables.TableSize)
						angle -= Tables.TableSize;
				}
			}
			else
			{
				for (var i = 0; i < count; i++)
				{
					buffer[i] = level[i] + table[(int)(phaseOffset[i] * Tables.TableSize + angle) & Tables.TableMask] * scale[i];
					angle += frequency[i] * AngleInc;
					if (angle > Tables.TableSize)
						angle -= Tables.TableSize;
				}
			}

			while (angle > Tables.TableSize)
				angle -= Tables.TableSize;
		}

		#endregion
	}
}