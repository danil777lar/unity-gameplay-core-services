using System.Collections.Generic;
using Newtonsoft.Json;

namespace Larje.Core.Localization
{
    public static class GTableParser
    {
        private const string KEY_COLUMN = "key";

        public static void Parse(string json, Dictionary<string, Dictionary<string, string>> target)
        {
            List<Dictionary<string, string>> rows = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
            if (rows == null)
            {
                return;
            }

            foreach (Dictionary<string, string> row in rows)
            {
                if (row.TryGetValue(KEY_COLUMN, out string key) && !string.IsNullOrEmpty(key) && key.ToLowerInvariant() != KEY_COLUMN)
                {
                    AddRow(key, row, target);
                }
            }
        }

        private static void AddRow(string key, Dictionary<string, string> row, Dictionary<string, Dictionary<string, string>> target)
        {
            if (!target.TryGetValue(key, out Dictionary<string, string> values))
            {
                values = new Dictionary<string, string>();
                target[key] = values;
            }

            foreach (KeyValuePair<string, string> cell in row)
            {
                if (cell.Key != KEY_COLUMN)
                {
                    values[cell.Key.ToLowerInvariant()] = cell.Value;
                }
            }
        }
    }
}
