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

		private readonly PoliLfo lfo = new PoliLfo();
		private readonly Vco vco1 = new Vco();
		private readonly Vco vco2 = new Vco();
		private readonly PoliMixer mixer = new PoliMixer();
		private readonly PoliFilter filter = new PoliFilter();
		private readonly PoliAmp amp = new PoliAmp();

		private readonly Triggerizer triggerizer = new Triggerizer();

		private readonly Oscillator oscillator = new Oscillator();
		private readonly TriggerGenerator trigger = new TriggerGenerator();

		private readonly StepSequencer sequencer = new StepSequencer(8);

		private readonly Delay delay = new Delay();

		private WaveOut waveOut;

		public ObservableCollection<object> ControllableComponents { get; } = new ObservableCollection<object>();

		public float TriggerLength
		{
			get { return trigger.TriggerLength.BaseValue; }
			set { trigger.TriggerLength.BaseValue = value; }
		}

		private float[] scale = { 0, 2 / 12f, 4 / 12f, 5 / 12f, 7 / 12f, 9 / 12f, 11 / 12f };
		private int[] seq = new int[8];


		private void SetSequencer(int index, int value)
		{
			seq[index] = value;
			var neg = value < 0;
			var diff = Math.Abs(value);
			var oct = diff / 7;
			var note = diff % 7;
			sequencer.ControlValues[index].BaseValue = neg ? -(oct + scale[note]) : oct + scale[note];
		}

		public int Sequencer0
		{
			get { return seq[0]; }
			set { SetSequencer(0, value); }
		}

		public int Sequencer1
		{
			get { return seq[1]; }
			set { SetSequencer(1, value); }
		}

		public int Sequencer2
		{
			get { return seq[2]; }
			set { SetSequencer(2, value); }
		}

		public int Sequencer3
		{
			get { return seq[3]; }
			set { SetSequencer(3, value); }
		}

		public int Sequencer4
		{
			get { return seq[4]; }
			set { SetSequencer(4, value); }
		}

		public int Sequencer5
		{
			get { return seq[5]; }
			set { SetSequencer(5, value); }
		}

		public int Sequencer6
		{
			get { return seq[6]; }
			set { SetSequencer(6, value); }
		}

		public int Sequencer7
		{
			get { return seq[7]; }
			set { SetSequencer(7, value); }
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