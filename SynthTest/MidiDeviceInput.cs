using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NAudio.Midi;
using SharpSynth;

namespace SynthTest
{
	public class MidiDeviceInput
	{
		private MidiIn midi;
		private List<int> pressedNotes = new List<int>();

		public ISynthComponent GateOutput { get; }
		public ISynthComponent ControlOutput { get; }

		public event Action<int, float> ControlEvent;

		public MidiDeviceInput()
		{
			if (MidiIn.NumberOfDevices > 0)
			{
				midi = new MidiIn(0);
				midi.MessageReceived += MidiOnMessageReceived;
				midi.Start();
			}

			GateOutput = new ControlInput { BaseValue = 0 };
			ControlOutput = new ControlInput { BaseValue = 0 };
		}

		private void MidiOnMessageReceived(object sender, MidiInMessageEventArgs e)
		{
			Debug.WriteLine("MIDI Event: {0} ({1})", e.MidiEvent.CommandCode, e.Timestamp);

			switch (e.MidiEvent.CommandCode)
			{
				case MidiCommandCode.NoteOn:
					var noteOn = (NoteOnEvent)e.MidiEvent;
					if (!pressedNotes.Contains(noteOn.NoteNumber))
						pressedNotes.Add(noteOn.NoteNumber);

					break;
				case MidiCommandCode.NoteOff:
					var note = (NoteEvent)e.MidiEvent;
					pressedNotes.RemoveAll(i => i == note.NoteNumber);
					break;

				case MidiCommandCode.ControlChange:
					var cc = (ControlChangeEvent)e.MidiEvent;
					ControlEvent?.Invoke((int)cc.Controller, cc.ControllerValue / 127f);
					break;
			}

			if (pressedNotes.Any())
			{
				((ControlInput)ControlOutput).BaseValue = ((float)pressedNotes.Average() - 57) / 12f;
				((ControlInput)GateOutput).BaseValue = 1;
			}
			else
			{
				((ControlInput)GateOutput).BaseValue = 0;
			}
		}
	}
}