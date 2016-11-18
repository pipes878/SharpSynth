using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SharpSynth;

namespace SynthTest
{
	public class MainWindowViewModel : IDisposable
	{
		private readonly Oscillator LFO = new Oscillator { Shape = OscillatorShape.Ramp };
		private readonly Oscillator Osc1 = new Oscillator { Shape = OscillatorShape.Sin };
		private readonly Oscillator Osc2 = new Oscillator { Shape = OscillatorShape.Sin };
		private readonly Oscillator Osc3 = new Oscillator { Shape = OscillatorShape.Sin };

		private readonly TriggerGenerator trigger = new TriggerGenerator();
		private readonly Amplifier Amp = new Amplifier();

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
			get { return Osc1.Scale.BaseValue; }
			set { Osc1.Scale.BaseValue = value; }
		}

		public float Scale2
		{
			get { return Osc2.Scale.BaseValue; }
			set { Osc2.Scale.BaseValue = value; }
		}

		public float Scale3
		{
			get { return Osc3.Scale.BaseValue; }
			set { Osc3.Scale.BaseValue = value; }
		}

		public void Play()
		{
			if (waveOut != null)
				return;

			waveOut = new WaveOut();
			waveOut.DesiredLatency = 80;
			waveOut.NumberOfBuffers = 4;
			LFO.Frequency.BaseValue = .5f;
			LFO.Scale.BaseValue = .5f;
			LFO.Level.BaseValue = 1;

			Osc1.Frequency.Control = new LinearFrequencyConverter(440) { Input = LFO };
			Osc2.Frequency.Control = new LinearFrequencyConverter(220) { Input = LFO };
			Osc3.Frequency.Control = new LinearFrequencyConverter(110) { Input = LFO };
			Scale1 = .5f;
			Scale2 = .5f;
			Scale3 = .5f;

			var mixer = new Mixer();
			mixer.Inputs.Add(Osc1);
			mixer.Inputs.Add(Osc2);
			mixer.Inputs.Add(Osc3);

			Amp.Input = mixer;
			Amp.Gain.Control = trigger;
			Amp.Gain.BaseValue = 0;
			trigger.Input = LFO;
			trigger.TriggerThreshold.BaseValue = 1;
			Frequency = 0;
			waveOut.Init(new SampleToWaveProvider16(new SynthToSampleProvider(Amp)));
			waveOut.Play();
		}

		public void Dispose()
		{
			waveOut.Dispose();
		}
	}
}