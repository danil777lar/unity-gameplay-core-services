using System.Threading;
using Cysharp.Threading.Tasks;

namespace Larje.Core
{
    public interface IAsyncInitializableService
    {
        UniTask InitializeAsync(CancellationToken cancellationToken);
    }
}
