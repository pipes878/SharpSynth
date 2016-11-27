using System;

namespace SharpSynth
{
	internal static class Tables
	{
		public const int TableBits = 20;
		public const int TableSize = 1 << TableBits;
		public const int TableMask = TableSize - 1;

		public static float[] Sin;
		public static float[] Ramp;
		public static float[] Triangle;

		static Tables()
		{
			Sin = new float[TableSize];
			Ramp = new float[TableSize];
			Triangle = new float[TableSize];

			for (var i = 0; i < TableSize; i++)
			{
				Sin[i] = (float)Math.Sin(2 * Math.PI * i / TableSize);
				Ramp[i] = 2f * i / (TableSize - 1) - 1;
				var result = 2f * ((i * 2) & TableMask) / TableSize - 1;
				Triangle[i] = i * 2 >= TableSize ? -result : result;
			}
		}
	}
}