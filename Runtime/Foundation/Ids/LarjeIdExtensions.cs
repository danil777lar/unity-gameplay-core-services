using System.Collections.Generic;

namespace Larje.Core.Ids
{
    public static class LarjeIdExtensions
    {
        public static T FindByKey<T>(this IEnumerable<T> ids, string key) where T : LarjeId
        {
            foreach (T id in ids)
            {
                if (id != null && id.Key == key)
                {
                    return id;
                }
            }

            return null;
        }
    }
}
