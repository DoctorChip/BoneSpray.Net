using JackSharp.Ports;
using System;

namespace BoneSpray.Net.Scenes.Attributes
{
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