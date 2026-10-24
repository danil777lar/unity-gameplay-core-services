using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Larje.Core.Locations
{
    public interface ILocationService
    {
        LocationInfo Current { get; }
        int CurrentEntry { get; }
        bool IsLoaded { get; }
        float Transition { get; }
        LocationsConfig Config { get; }

        event Action Exiting;
        event Action LoadStarted;
        event Action<LocationInfo, int> Loaded;
        event Action<LocationInfo, int> Entered;

        UniTask LoadAsync(LocationId location, int entry = 0, CancellationToken cancellationToken = default);
        UniTask QuitToMenuAsync(CancellationToken cancellationToken = default);
        void AddEntry(ILocationEntry entry);
        void RemoveEntry(ILocationEntry entry);
        bool TryGetEntry(out ILocationEntry entry);
        IDisposable OnEntered(Action callback, LocationFilter filter = null);
    }
}
