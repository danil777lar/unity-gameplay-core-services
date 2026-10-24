using Cysharp.Threading.Tasks;

namespace Larje.Core.Zenject
{
    public interface ILarjeReady
    {
        bool IsReady { get; }

        UniTask WaitAsync();
    }
}
