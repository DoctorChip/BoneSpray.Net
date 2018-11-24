using System;

namespace BoneSpray.Net.Models.Attributes
{
    /// <summary>
    /// A descriptor to tell our startup which <see cref="IBaseScene"/>
    /// this renderer will listen to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SceneSourceAttribute : Attribute
    {
        public string Scene { get; }

        public Type Type { get; }

        public SceneSourceAttribute(string Scene)
        {
            this.Scene = Scene;
        }

        public SceneSourceAttribute(Type Scene)
        {
            this.Type = Scene;
        }
    }
}