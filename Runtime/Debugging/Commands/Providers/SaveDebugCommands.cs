using System.Text;
using Larje.Core.Saves;

namespace Larje.Core.Debugging
{
    public sealed class SaveDebugCommands : IDebugCommandProvider
    {
        private readonly ISaveService _saves;

        public SaveDebugCommands(ISaveService saves)
        {
            _saves = saves;
        }

        [DebugCommand("Saves", "Save current game to slot (empty = current)")]
        public string Save(string slot = "")
        {
            _saves.SaveGame(slot);
            return "Saved " + _saves.CurrentSlot;
        }

        [DebugCommand("Saves", "Load game from slot (empty = current)")]
        public string Load(string slot = "")
        {
            return _saves.LoadGame(slot) ? "Loaded " + _saves.CurrentSlot : "Slot not found";
        }

        [DebugCommand("Saves", "Start a new empty game in slot")]
        public string NewGame(string slot = "")
        {
            _saves.NewGame(slot);
            return "New game " + _saves.CurrentSlot;
        }

        [DebugCommand("Saves", "Delete all saves and settings")]
        public string DeleteAll()
        {
            _saves.DeleteAll();
            return "All saves deleted";
        }

        [DebugCommand("Saves", "List save slots")]
        public string ListSlots()
        {
            StringBuilder builder = new StringBuilder();
            foreach (SaveMeta meta in _saves.GetSaves())
            {
                builder.Append(meta.Slot).Append(" (").Append(meta.Date).Append(")\n");
            }

            return builder.Length > 0 ? builder.ToString() : "No saves";
        }

        [DebugCommand("Saves", "Print a save section as JSON")]
        public string ExportSection(string key, SaveScope scope = SaveScope.Game)
        {
            return _saves.ExportSection(scope, key) ?? "Section not found";
        }

        [DebugCommand("Saves", "Replace a save section with JSON")]
        public string ImportSection(string key, string json, SaveScope scope = SaveScope.Game)
        {
            return _saves.ImportSection(scope, key, json) ? "Imported " + key : "Invalid JSON";
        }
    }
}
