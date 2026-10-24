using System;
using UnityEngine;

namespace Larje.Core.LevelObjects
{
    public interface ILevelObjectDataUser
    {
        GameObject GameObject { get; }
        Type DataType { get; }

        void InjectData(object data);
        object ReadData();
    }
}
