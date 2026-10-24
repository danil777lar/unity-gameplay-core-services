using System;
using System.Collections.Generic;

namespace Larje.Core.Tasks
{
    public abstract class SimpleTaskStep : TaskStep
    {
        private TaskStep _next;

        public override int CurrentIndex => IsCompleted && _next != null ? _next.CurrentIndex : RawIndex;

        protected override TaskStep Next => _next;
        protected override IReadOnlyList<TaskStep> Children => _next == null ? Array.Empty<TaskStep>() : new[] { _next };

        protected SimpleTaskStep(int index, string id) : base(index, id)
        {
        }

        public SimpleTaskStep SetNext(TaskStep next)
        {
            _next = next;
            return this;
        }
    }
}
