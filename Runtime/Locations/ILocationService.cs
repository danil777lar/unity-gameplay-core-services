using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Larje.Core.Locations
{
    public interface ILocationService
    {
        LocationDefinition Current { get; }
        int CurrentEntry { get; }
        bool IsLoaded { get; }
        float Transition { get; }
        LocationsConfig Config { get; }

        event Action Exiting;
        event Action LoadStarted;
        event Action<LocationDefinition, int> Loaded;
        event Action<LocationDefinition, int> Entered;

        UniTask LoadAsync(LocationId location, int entry = 0, CancellationToken cancellationToken = default);
        UniTask QuitToMenuAsync(CancellationToken cancellationToken = default);
        void AddEntry(ILocationEntry entry);
        void RemoveEntry(ILocationEntry entry);
        bool TryGetEntry(out ILocationEntry entry);
        IDisposable OnEntered(Action callback, LocationFilter filter = null);
    }
}
