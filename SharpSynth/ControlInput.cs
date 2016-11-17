namespace SharpSynth
{
	/// <summary>
	/// 
	/// </summary>
	public class ControlInput : ISynthComponent
	{
		private long generatedTimeBase;
		private float[] generatedBuffer;
		private int generatedCount;
		private float? generatedBaseValue;

		public float BaseValue { get; set; }

		public ISynthComponent Control { get; set; }

		public float[] GenerateSamples(int count, long timeBase)
		{
			if (generatedTimeBase != timeBase || generatedBuffer == null || generatedCount < count || generatedBaseValue != BaseValue)
			{
				if (generatedBuffer == null || generatedBuffer.Length < count)
				{
					generatedBuffer = new float[count];
					generatedBaseValue = null;
				}

				if (Control == null)
				{
					if (generatedBaseValue != BaseValue)
					{
						for (var i = 0; i < generatedBuffer.Length; i++)
							generatedBuffer[i] = BaseValue;

						generatedBaseValue = BaseValue;
						generatedCount = generatedBuffer.Length;
					}
				}
				else
				{
					generatedBuffer = Control.GenerateSamples(count, timeBase);
					for (var i = 0; i < generatedBuffer.Length; i++)
						generatedBuffer[i] += BaseValue;
					generatedTimeBase = timeBase;
					generatedBaseValue = null;
					generatedCount = count;
				}
			}

			return generatedBuffer;
		}
	}
}