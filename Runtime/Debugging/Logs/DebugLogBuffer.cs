using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Larje.Core.Debugging
{
    public sealed class DebugLogBuffer : IInitializableService, IDisposable
    {
        private const int CAPACITY = 500;

        private int _version;
        private readonly object _lock = new object();
        private readonly Stopwatch _clock = Stopwatch.StartNew();
        private readonly Queue<DebugLogEntry> _entries = new Queue<DebugLogEntry>();

        public int Version => _version;

        public void Initialize()
        {
            Application.logMessageReceivedThreaded += OnLog;
        }

        public void Dispose()
        {
            Application.logMessageReceivedThreaded -= OnLog;
        }

        public List<DebugLogEntry> GetEntries(int maxCount = CAPACITY)
        {
            lock (_lock)
            {
                List<DebugLogEntry> result = new List<DebugLogEntry>(_entries);
                int skip = Mathf.Max(0, result.Count - maxCount);
                return result.GetRange(skip, result.Count - skip);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _entries.Clear();
                _version++;
            }
        }

        private void OnLog(string message, string stackTrace, LogType type)
        {
            lock (_lock)
            {
                if (_entries.Count >= CAPACITY)
                {
                    _entries.Dequeue();
                }

                _entries.Enqueue(new DebugLogEntry((float)_clock.Elapsed.TotalSeconds, type, message, stackTrace));
                _version++;
            }
        }
    }
}
