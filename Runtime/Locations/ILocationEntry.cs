using UnityEngine;

namespace Larje.Core.Locations
{
    public interface ILocationEntry
    {
        int Id { get; }
        Vector3 Position { get; }
        Vector3 Direction { get; }
    }
}
