using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SharpSynth;

namespace SynthTest
{
	public class MainWindowViewModel : IDisposable
	{
		private float pitch;
		private readonly Oscillator lfo = new Oscillator { Shape = OscillatorShape.Ramp };
		private readonly Oscillator Osc1 = new Oscillator { Shape = OscillatorShape.Sin };

		private readonly TriggerGenerator trigger = new TriggerGenerator();
		private readonly EnvelopeGenerator envelope = new EnvelopeGenerator();
		private readonly Amplifier Amp = new Amplifier();
		private readonly Filter LowPassFilter = new Filter { FilterType = FilterType.LowPass };
		private readonly Filter HighPassFilter = new Filter { FilterType = FilterType.HighPass };
		private readonly StepSequencer sequencer = new StepSequencer(4);

		private readonly Delay delay = new Delay();

		private readonly Cutoff cutoff = new Cutoff();
		private WaveOut waveOut;

		public ObservableCollection<ISynthComponent> ControllableComponents { get; } = new ObservableCollection<ISynthComponent>();

		public float Frequency
		{
			get { return lfo.Frequency.BaseValue; }
			set
			{
				lfo.Frequency.BaseValue = value;
			}
		}

		//public float Pitch
		//{
		//	get { return pitch; }
		//	set
		//	{
		//		pitch = value;
		//		var pitch2 = (float)Math.Pow(2, pitch);
		//		Osc1.Frequency.BaseValue = 440f * pitch2;
		//	}
		//}

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

		public float Sequencer0
		{
			get { return sequencer.ControlValues[0].BaseValue; }
			set { sequencer.ControlValues[0].BaseValue = value; }
		}

		public float Sequencer1
		{
			get { return sequencer.ControlValues[1].BaseValue; }
			set { sequencer.ControlValues[1].BaseValue = value; }
		}

		public float Sequencer2
		{
			get { return sequencer.ControlValues[2].BaseValue; }
			set { sequencer.ControlValues[2].BaseValue = value; }
		}

		public float Sequencer3
		{
			get { return sequencer.ControlValues[3].BaseValue; }
			set { sequencer.ControlValues[3].BaseValue = value; }
		}

		public float Attack
		{
			get { return envelope.Attack.BaseValue; }
			set { envelope.Attack.BaseValue = value; }
		}

		public float Decay
		{
			get { return envelope.Decay.BaseValue; }
			set { envelope.Decay.BaseValue = value; }
		}

		public float Sustain
		{
			get { return envelope.Sustain.BaseValue; }
			set { envelope.Sustain.BaseValue = value; }
		}
		public float Release
		{
			get { return envelope.Release.BaseValue; }
			set { envelope.Release.BaseValue = value; }
		}

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

			ControllableComponents.Add(lfo);
			ControllableComponents.Add(delay);

			waveOut = new WaveOut();
			waveOut.DesiredLatency = 80;
			waveOut.NumberOfBuffers = 4;

			lfo.Frequency.BaseValue = 5f;
			lfo.Scale.BaseValue = 1f;
			lfo.Level.BaseValue = .5f;

			//Osc1.Frequency.Control = new LinearFrequencyConverter(440) { Input = LFO };
			//Osc2.Frequency.Control = new LinearFrequencyConverter(220) { Input = LFO };
			//Osc3.Frequency.Control = new LinearFrequencyConverter(110) { Input = LFO };
			Shape = 0;
			//Pitch = 0;

			var mixer = new Mixer();
			//mixer.Inputs.Add(Osc1);
			//mixer.Inputs.Add(cutoff);
			//mixer.Inputs.Add(Osc1);
			mixer.Inputs.Add(HighPassFilter);
			mixer.Inputs.Add(LowPassFilter);

			LowPassFilter.Input = Osc1;
			HighPassFilter.Input = Osc1;
			sequencer.TriggerSource.Control = trigger;
			Osc1.Frequency.Control = new LinearFrequencyConverter(110) { Input = sequencer };

			delay.Input = Amp;
			Amp.Input = mixer;
			Amp.Gain.Control = envelope;
			Amp.Gain.BaseValue = 0f;
			trigger.Input = lfo;
			trigger.TriggerThreshold.BaseValue = 1;
			trigger.TriggerLength.BaseValue = .5f;

			envelope.Input = trigger;
			envelope.Attack.BaseValue = 0.03f;
			envelope.Decay.BaseValue = .3f;
			envelope.Sustain.BaseValue = .1f;

			//cutoff.Input = Osc1;
			//cutoff.CutoffThreshold.Control = envelope;

			Frequency = 0;
			waveOut.Init(new SampleToWaveProvider16(new SynthToSampleProvider(delay)));
			waveOut.Play();
		}

		public void Dispose()
		{
			waveOut.Dispose();
		}
	}

	public class EnumDataConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <summary>Converts a value. </summary>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!value.GetType().IsEnum)
				throw new InvalidOperationException("Target type must be an enum");

			return value;
		}

		/// <summary>Converts a value. </summary>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		#endregion
	}

}