using System.Collections.Generic;

namespace Larje.Core.Monetization
{
    public interface IAnalyticsService
    {
        void Send(string eventName);
        void Send(string eventName, IReadOnlyDictionary<string, object> parameters);
    }
}
