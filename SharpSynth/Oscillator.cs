using System;
using System.Collections.Generic;
using SharpSynth.Input;

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
		public ControlValue Frequency { get; } = new ControlValue { BaseValue = 0 };

		/// <summary>
		/// PWM control for the square wave.
		/// </summary>
		public ControlValue SquarePwm { get; } = new ControlValue { BaseValue = 0.5f };

		#region Implementation of ISynthComponent

		/// <summary>
		/// Generate synthesizer samples
		/// </summary>
		/// <param name="buffer">The buffer to generate into.</param>
		/// <param name="count">The number of samples to generate.</param>
		/// <param name="timeBase">The time base for the new samples. This value is in samples.</param>
		protected override void GenerateSamples(float[] buffer, int count, long timeBase)
		{
			double angleInc = Tables.TableSize / (float)SampleRate;

			float[] table;
			if (!ShapeTables.TryGetValue(Shape, out table) && Shape != OscillatorShape.Square)
			{
				for (var i = 0; i < count; i++)
					buffer[i] = 0;
				return;
			}

			var frequency = Frequency.GenerateSamples(count, timeBase);

			if (Shape == OscillatorShape.Square)
			{
				var pwm = SquarePwm.GenerateSamples(count, timeBase);
				for (var i = 0; i < count; i++)
				{
					var a = (int)(Tables.TableSize + angle) & Tables.TableMask;
					var pw = pwm[i] * Tables.TableSize;
					buffer[i] = a < pw ? 1 : 0;

					angle += frequency[i] * angleInc;
					if (angle > Tables.TableSize)
						angle -= Tables.TableSize;
				}
			}
			else
			{
				for (var i = 0; i < count; i++)
				{
					buffer[i] = table[(int)(Tables.TableSize + angle) & Tables.TableMask];
					angle += frequency[i] * angleInc;
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