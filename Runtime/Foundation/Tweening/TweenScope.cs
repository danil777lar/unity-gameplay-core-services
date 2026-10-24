using System;
using System.Threading;

namespace Larje.Core.Tweening
{
    public sealed class TweenScope : IDisposable
    {
        private CancellationTokenSource _source;
        private readonly CancellationToken _owner;

        public TweenScope(CancellationToken owner = default)
        {
            _owner = owner;
        }

        public CancellationToken Restart()
        {
            Kill();
            _source = _owner.CanBeCanceled
                ? CancellationTokenSource.CreateLinkedTokenSource(_owner)
                : new CancellationTokenSource();
            return _source.Token;
        }

        public void Kill()
        {
            if (_source == null)
            {
                return;
            }

            _source.Cancel();
            _source.Dispose();
            _source = null;
        }

        public void Dispose()
        {
            Kill();
        }
    }
}
