using System;
using UnityEngine;

namespace Larje.Core.Input
{
    public sealed class PointerCommand
    {
        public float MinDistance;
        public float ExecuteDuration;
        public Vector3 TargetPosition;
        public Action<GameObject> Execute;
    }
}
