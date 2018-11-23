using System;

namespace BoneSpray.Net.Scenes.Attributes
{
    /// <summary>
    /// A marker to tell the system which scene to launch on start.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class StartupSceneAttribute : Attribute
    {
    }
}