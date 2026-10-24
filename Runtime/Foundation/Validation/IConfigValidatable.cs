using System.Collections.Generic;

namespace Larje.Core
{
    public interface IConfigValidatable
    {
        void Validate(List<string> problems);
    }
}
