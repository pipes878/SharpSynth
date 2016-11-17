using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SharpSynth;

namespace SynthTest
{
	public class MainWindowViewModel : IDisposable
	{
		private readonly Oscillator sinWave = new Oscillator { Shape = OscillatorShape.Square, Scale = .3f };
		private readonly Oscillator rampWave = new Oscillator { Shape = OscillatorShape.Triangle, Frequency = 220, Scale = .2f, PhaseOffset = 0f };
		private readonly Oscillator rampWave2 = new Oscillator { Shape = OscillatorShape.Sin, Frequency = 110, Scale = .5f, PhaseOffset = 0f };
		private WaveOut waveOut;
		private float freq = 20;

		public float Frequency
		{
			get { return freq; }
			set
			{
				freq = value;
				sinWave.Frequency = (float)(440 * Math.Pow(2, freq));
				rampWave.Frequency = sinWave.Frequency / 2;
				rampWave2.Frequency = rampWave.Frequency / 2;
			}
		}

		public float Scale1
		{
			get { return sinWave.Scale; }
			set { sinWave.Scale = value; }
		}

		public float Scale2
		{
			get { return rampWave.Scale; }
			set { rampWave.Scale = value; }
		}

		public float Scale3
		{
			get { return rampWave2.Scale; }
			set { rampWave2.Scale = value; }
		}

		public void Play()
		{
			if (waveOut != null)
				return;

			waveOut = new WaveOut();
			waveOut.DesiredLatency = 80;
			waveOut.NumberOfBuffers = 4;

			var mixer = new Mixer();
			mixer.Inputs.Add(sinWave);
			mixer.Inputs.Add(rampWave);
			mixer.Inputs.Add(rampWave2);
			Frequency = 0;
			waveOut.Init(new SampleToWaveProvider16(new SynthToSampleProvider(mixer)));
			waveOut.Play();
		}

		public void Dispose()
		{
			waveOut.Dispose();
		}
	}
}