using System;

namespace BoneSpray.Net.Models.Attributes
{
    /// <summary>
    /// Required to inform the system which key should set a scene to Active.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class KeybindAttribute : Attribute
    {
        public int Keybind { get; set; }

        public KeybindAttribute(int keybind)
        {
            Keybind = keybind;
        }
    }
}