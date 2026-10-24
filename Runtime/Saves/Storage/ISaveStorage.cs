using System.Collections.Generic;

namespace Larje.Core.Saves
{
    public interface ISaveStorage
    {
        bool TryRead(string file, out string content);
        void Write(string file, string content);
        void Delete(string file);
        void DeleteAll();
        IReadOnlyList<string> GetFiles();
    }
}
