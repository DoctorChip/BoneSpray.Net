using System;

namespace BoneSpray.Net.Models.Attributes
{
    /// <summary>
    /// Allows us to mark a BaseScene with a Key, to keep track of them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SceneKeyAttribute : Attribute
    {
        public string Key;

        public SceneKeyAttribute(string Key)
        {
            this.Key = Key;
        }
    }
}