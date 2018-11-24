using BoneSpray.Net.Models;
using System.Collections.Generic;

namespace BoneSpray.Net.Scenes
{
    public interface IBaseScene
    {
        List<OutPortContainer> OutPorts { get; set; }
    }
}