using System;
using Larje.Core.UI;
using UnityEngine;

namespace Larje.Core.Input
{
    public interface IPointerControlService
    {
        event Action<PointerCommand> Command;

        void SetCamera(Camera camera, Transform rig);
        void SetControlRect(RectTransformEvents rect);
    }
}
