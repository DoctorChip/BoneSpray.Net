using JackSharp.Ports;
using System;

namespace BoneSpray.Net.Models.Attributes
{
    /// <summary>
    /// Allows a Scene to request a port to be wired up to the specified Callback.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequiredPortAttribute : Attribute
    {
        public PortType Type { get; set; }

        public string PortName { get; set; }

        public string CallbackName { get; set; }

        public RequiredPortAttribute(PortType Type, string PortName, string CallbackName)
        {
            this.Type = Type;
            this.PortName = PortName;
            this.CallbackName = CallbackName;
        }
    }
}