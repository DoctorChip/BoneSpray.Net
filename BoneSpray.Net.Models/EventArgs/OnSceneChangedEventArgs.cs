using System;

namespace BoneSpray.Net.Models.Events
{
    public class OnSceneChangedEventArgs : EventArgs
    {
        public Type ActiveScene { get; set; }
    }
}