using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Larje.Core.Monetization
{
    public sealed class LogAnalyticsService : IAnalyticsService
    {
        public void Send(string eventName)
        {
            Debug.Log($"Analytics: {eventName}");
        }

        public void Send(string eventName, IReadOnlyDictionary<string, object> parameters)
        {
            StringBuilder builder = new StringBuilder("Analytics: ").Append(eventName);
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                builder.Append(' ').Append(parameter.Key).Append('=').Append(parameter.Value);
            }

            Debug.Log(builder.ToString());
        }
    }
}
