using System.Diagnostics;
using NAudio.Midi;
using SharpSynth;

namespace SynthTest
{
	public class MidiDeviceInput
	{
		private MidiIn midi;
		private int? pressedNode;

		public ISynthComponent GateOutput { get; }
		public ISynthComponent ControlOutput { get; }

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
					var noteOn = e.MidiEvent as NoteOnEvent;
					if (pressedNode == null)
					{
						pressedNode = noteOn.NoteNumber;
						((ControlInput)ControlOutput).BaseValue = (pressedNode.Value - 50) / 12f;
						((ControlInput)GateOutput).BaseValue = 1f;
						//pressedNode 
					}
					break;
				case MidiCommandCode.NoteOff:
					var note = e.MidiEvent as NoteEvent;
					if (pressedNode == note.NoteNumber)
					{
						pressedNode = null;
						((ControlInput)GateOutput).BaseValue = 0f;
						//pressedNode 
					}
					break;
			}
			//if (e.)
			//Debug.WriteLine("Note {0}: {1}");

		}
	}
}