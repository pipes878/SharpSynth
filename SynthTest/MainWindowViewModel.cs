using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SharpSynth;

namespace SynthTest
{
	public class MainWindowViewModel : IDisposable
	{
		private readonly Oscillator LFO = new Oscillator {Shape = OscillatorShape.Ramp, Offset = 1, Scale = .5f};
		private readonly Oscillator Osc1 = new Oscillator { Shape = OscillatorShape.Square, Scale = .3f };
		private readonly Oscillator Osc2 = new Oscillator { Shape = OscillatorShape.Triangle, Scale = .2f, PhaseOffset = 0f };
		private readonly Oscillator Osc3 = new Oscillator { Shape = OscillatorShape.Sin, Scale = .5f, PhaseOffset = 0f };
		private WaveOut waveOut;

		public float Frequency
		{
			get { return LFO.Frequency.BaseValue; }
			set
			{
				LFO.Frequency.BaseValue = value;
			}
		}

		public float Scale1
		{
			get { return Osc1.Scale; }
			set { Osc1.Scale = value; }
		}

		public float Scale2
		{
			get { return Osc2.Scale; }
			set { Osc2.Scale = value; }
		}

		public float Scale3
		{
			get { return Osc3.Scale; }
			set { Osc3.Scale = value; }
		}

		public void Play()
		{
			if (waveOut != null)
				return;

			waveOut = new WaveOut();
			waveOut.DesiredLatency = 80;
			waveOut.NumberOfBuffers = 4;
			LFO.Frequency.BaseValue = .5f;
			LFO.Scale = .5f;

			Osc1.Frequency.Control = new LinearFrequencyConverter(440) { Input = LFO };
			Osc2.Frequency.Control = new LinearFrequencyConverter(220) { Input = LFO };
			Osc3.Frequency.Control = new LinearFrequencyConverter(110) { Input = LFO };

			var mixer = new Mixer();
			mixer.Inputs.Add(Osc1);
			mixer.Inputs.Add(Osc2);
			mixer.Inputs.Add(Osc3);
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