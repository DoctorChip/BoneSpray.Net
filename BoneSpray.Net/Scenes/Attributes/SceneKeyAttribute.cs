using System;

namespace BoneSpray.Net.Scenes.Attributes
{
    class SceneKeyAttribute : Attribute
    {
        public string Key;

        public SceneKeyAttribute(string Key)
        {
            this.Key = Key;
        }
    }
}