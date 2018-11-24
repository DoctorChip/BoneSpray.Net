using System;

namespace BoneSpray.Net.Models.Attributes
{
    /// <summary>
    /// A marker to tell the system which scene to launch on start.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StartupSceneAttribute : Attribute
    {
    }
}