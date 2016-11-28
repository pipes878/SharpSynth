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

		private readonly Lfo lfo = new Lfo();
		private readonly Vco vco1 = new Vco();
		private readonly Vco vco2 = new Vco();
		private readonly PoliMixer mixer = new PoliMixer();
		private readonly PoliFilter filter = new PoliFilter();
		private readonly PoliAmp amp = new PoliAmp();

		private readonly Triggerizer triggerizer = new Triggerizer();

		private readonly Oscillator oscillator = new Oscillator();
		private readonly TriggerGenerator trigger = new TriggerGenerator();

		private readonly StepSequencer sequencer = new StepSequencer(4);

		private readonly Delay delay = new Delay();

		private WaveOut waveOut;

		public ObservableCollection<object> ControllableComponents { get; } = new ObservableCollection<object>();

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

		public void Play()
		{
			if (waveOut != null)
				return;

			waveOut = new WaveOut();
			waveOut.DesiredLatency = 80;
			waveOut.NumberOfBuffers = 4;

			ControllableComponents.Add(lfo);
			ControllableComponents.Add(vco1);
			ControllableComponents.Add(vco2);
			ControllableComponents.Add(mixer);
			ControllableComponents.Add(triggerizer);
			ControllableComponents.Add(filter);
			ControllableComponents.Add(amp);

			ControllableComponents.Add(delay);
			ControllableComponents.Add(oscillator);

			vco1.LfoInput = lfo.Output;
			vco1.XModInput = vco2.Output;
			vco2.LfoInput = lfo.Output;

			mixer.Osc1 = vco1.Output;
			mixer.Osc2 = vco2.Output;

			filter.Input = mixer.Output;
			filter.TriggerInput = trigger;
			filter.LfoInput = lfo.Output;

			amp.LfoInput = lfo.Output;
			amp.TriggerInput = trigger;
			amp.Input = filter.Output;

			sequencer.TriggerSource.Control = trigger;

			vco1.ControlInput = sequencer;
			vco2.ControlInput = sequencer;

			delay.Input = amp.Output;
			trigger.Input = oscillator;
			trigger.TriggerThreshold.BaseValue = 1;
			trigger.TriggerLength.BaseValue = .5f;
			oscillator.Level.BaseValue = 1;

			//cutoff.Input = Osc1;
			//cutoff.CutoffThreshold.Control = envelope;

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