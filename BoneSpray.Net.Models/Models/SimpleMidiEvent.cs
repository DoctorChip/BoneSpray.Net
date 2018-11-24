namespace BoneSpray.Net.Models
{
    public class SimpleMidiEvent
    {
        public MidiEventType Type { get; set; }

        public int Note { get; set; }

        public int Velocity { get; set; }
    }

    public enum MidiEventType
    {
        ON  = 1,
        OFF = 0,
    }
}
