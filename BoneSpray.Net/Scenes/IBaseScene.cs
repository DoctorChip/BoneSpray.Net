using JackSharp.Processing;
using System;
using System.Collections.Generic;

namespace BoneSpray.Net.Scenes
{
    public interface IBaseScene
    {
        string GetKey();

        Dictionary<string, Action<ProcessBuffer>> RequiredMidiPortNames { get; }
        Dictionary<string, Action<ProcessBuffer>> RequiredAudioPortNames { get; }
    }
}