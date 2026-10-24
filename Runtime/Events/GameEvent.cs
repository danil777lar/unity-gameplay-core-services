namespace Larje.Core.Events
{
    public abstract class GameEvent
    {
        public readonly string Source;

        public virtual bool IsValid => true;

        protected GameEvent(string source)
        {
            Source = source;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
