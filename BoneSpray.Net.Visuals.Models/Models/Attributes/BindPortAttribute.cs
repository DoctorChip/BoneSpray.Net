using JackSharp.Ports;
using System;

namespace BoneSpray.Net.Visuals.Models.Models.Attributes
{
    /// <summary>
    /// Sigals which ports the Visual Scene would like to register to, and the Callback it would like
    /// the data passed to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BindPortAttribute : Attribute
    {
        public PortType Type { get; }

        public Type Scene { get; }

        public string PortName { get; }

        public string Callback { get; }

        public BindPortAttribute(PortType Type, Type Scene, string PortName, string Callback)
        {
            this.Type = Type;
            this.Scene = Scene;
            this.PortName = PortName;
            this.Callback = Callback;
        }
    }
}