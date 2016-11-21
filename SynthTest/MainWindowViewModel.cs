using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SharpSynth;

namespace SynthTest
{
	public class MainWindowViewModel : IDisposable
	{
		private float pitch;
		private readonly Oscillator LFO = new Oscillator { Shape = OscillatorShape.Ramp };
		private readonly Oscillator Osc1 = new Oscillator { Shape = OscillatorShape.Sin };

		private readonly TriggerGenerator trigger = new TriggerGenerator();
		//private readonly EnvelopeGenerator envelope = new EnvelopeGenerator();
		private readonly Amplifier Amp = new Amplifier();
		private readonly Filter LowPassFilter = new Filter { FilterType = FilterType.LowPass };
		private readonly Filter HighPassFilter = new Filter { FilterType = FilterType.HighPass };

		private readonly Cutoff cutoff = new Cutoff();
		private WaveOut waveOut;

		public float Frequency
		{
			get { return LFO.Frequency.BaseValue; }
			set
			{
				LFO.Frequency.BaseValue = value;
			}
		}

		public float Pitch
		{
			get { return pitch; }
			set
			{
				pitch = value;
				var pitch2 = (float)Math.Pow(2, pitch);
				Osc1.Frequency.BaseValue = 440f * pitch2;
			}
		}

		public int Shape
		{
			get { return (int)Osc1.Shape; }
			set { Osc1.Shape = (OscillatorShape)value; }
		}

		public float TriggerLength
		{
			get { return trigger.TriggerLength.BaseValue; }
			set { trigger.TriggerLength.BaseValue = value; }
		}

		//public float Attack
		//{
		//	get { return envelope.Attack.BaseValue; }
		//	set { envelope.Attack.BaseValue = value; }
		//}

		//public float Decay
		//{
		//	get { return envelope.Decay.BaseValue; }
		//	set { envelope.Decay.BaseValue = value; }
		//}

		//public float Sustain
		//{
		//	get { return envelope.Sustain.BaseValue; }
		//	set { envelope.Sustain.BaseValue = value; }
		//}
		//public float Release
		//{
		//	get { return envelope.Release.BaseValue; }
		//	set { envelope.Release.BaseValue = value; }
		//}

		public float LowPassCutoff
		{
			get { return LowPassFilter.CutoffFrequency.BaseValue; }
			set { LowPassFilter.CutoffFrequency.BaseValue = value; }
		}

		public float HighPassCutoff
		{
			get { return HighPassFilter.CutoffFrequency.BaseValue; }
			set { HighPassFilter.CutoffFrequency.BaseValue = value; }
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

			//Osc1.Frequency.Control = new LinearFrequencyConverter(440) { Input = LFO };
			//Osc2.Frequency.Control = new LinearFrequencyConverter(220) { Input = LFO };
			//Osc3.Frequency.Control = new LinearFrequencyConverter(110) { Input = LFO };
			Shape = 0;
			Pitch = 0;

			var mixer = new Mixer();
			//mixer.Inputs.Add(Osc1);
			//mixer.Inputs.Add(cutoff);
			mixer.Inputs.Add(HighPassFilter);
			mixer.Inputs.Add(LowPassFilter);

			LowPassFilter.Input = Osc1;
			HighPassFilter.Input = Osc1;

			Amp.Input = mixer;
			//Amp.Gain.Control = envelope;
			Amp.Gain.BaseValue = .5f;
			trigger.Input = LFO;
			trigger.TriggerThreshold.BaseValue = 1;
			trigger.TriggerLength.BaseValue = .5f;
			//envelope.Input = trigger;
			//envelope.Attack.BaseValue = 0.03f;
			//envelope.Decay.BaseValue = .3f;
			//envelope.Sustain.BaseValue = .1f;

			//cutoff.Input = Osc1;
			//cutoff.CutoffThreshold.Control = envelope;

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