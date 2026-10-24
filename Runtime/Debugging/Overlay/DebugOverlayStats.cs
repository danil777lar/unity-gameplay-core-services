using System;
using System.Globalization;
using System.Text;
using Unity.Profiling;
using UnityEngine;

namespace Larje.Core.Debugging
{
    public sealed class DebugOverlayStats : IDisposable
    {
        private int _frames;
        private float _elapsed;
        private float _fps;
        private ProfilerRecorder _batches;
        private ProfilerRecorder _triangles;
        private readonly StringBuilder _builder = new StringBuilder();

        public DebugOverlayStats()
        {
            _batches = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
            _triangles = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        }

        public void Dispose()
        {
            _batches.Dispose();
            _triangles.Dispose();
        }

        public string Build(float unscaledDeltaTime)
        {
            _frames++;
            _elapsed += unscaledDeltaTime;
            if (_elapsed >= 0.5f)
            {
                _fps = _frames / _elapsed;
                _frames = 0;
                _elapsed = 0f;
            }

            _builder.Clear();
            _builder.Append("FPS: ").Append(Mathf.RoundToInt(_fps)).Append('\n');
            _builder.Append("BATCHES: ").Append(_batches.Valid ? _batches.LastValue : 0).Append('\n');
            _builder.Append("TRIANGLES: ").Append(FormatCount(_triangles.Valid ? _triangles.LastValue : 0)).Append('\n');

            foreach (string group in DebugOverlayLog.Groups)
            {
                if (DebugOverlayLog.IsVisible(group))
                {
                    _builder.Append('[').Append(group).Append("]\n");
                    foreach (string message in DebugOverlayLog.Consume(group))
                    {
                        _builder.Append("  ").Append(message).Append('\n');
                    }
                }
            }

            return _builder.ToString();
        }

        private string FormatCount(long count)
        {
            if (count >= 1_000_000)
            {
                return (count / 1_000_000f).ToString("F2", CultureInfo.InvariantCulture) + "M";
            }

            if (count >= 1_000)
            {
                return (count / 1_000f).ToString("F2", CultureInfo.InvariantCulture) + "K";
            }

            return count.ToString(CultureInfo.InvariantCulture);
        }
    }
}
