using System.Collections.Generic;
using Larje.Core.Saves;
using UnityEngine;

namespace Larje.Core.LevelObjects
{
    [RequireComponent(typeof(GuidHolder))]
    public sealed class LevelObjectDataController : MonoBehaviour
    {
        private string _key;
        private ISaveService _saves;
        private readonly List<ILevelObjectDataUser> _users = new List<ILevelObjectDataUser>();

#if LARJE_ZENJECT
        [Zenject.Inject]
#endif
        public void Construct(ISaveService saves, ILevelObjectKeyProvider keyProvider)
        {
            _saves = saves;
            GuidHolder holder = GetComponent<GuidHolder>();
            if (string.IsNullOrEmpty(holder.Guid))
            {
                Debug.LogWarning($"LevelObjectDataController: empty guid on {name}", this);
                return;
            }

            _key = keyProvider.GetKeyPrefix() + "_" + holder.Guid;
            CollectUsers();
            Inject();
            _saves.Saving += Save;
        }

        private void OnDestroy()
        {
            if (_saves != null)
            {
                _saves.Saving -= Save;
            }
        }

        private void CollectUsers()
        {
            _users.Clear();
            foreach (ILevelObjectDataUser user in GetComponentsInChildren<ILevelObjectDataUser>(true))
            {
                if (user.GameObject.GetComponentInParent<LevelObjectDataController>() == this)
                {
                    _users.Add(user);
                }
            }
        }

        private void Inject()
        {
            if (!_saves.Get<LevelObjectsSaveData>().Objects.TryGetValue(_key, out Dictionary<string, string> stored))
            {
                return;
            }

            foreach (ILevelObjectDataUser user in _users)
            {
                if (stored.TryGetValue(user.DataType.FullName, out string json))
                {
                    user.InjectData(JsonUtility.FromJson(json, user.DataType));
                }
            }
        }

        private void Save()
        {
            Dictionary<string, Dictionary<string, string>> objects = _saves.Get<LevelObjectsSaveData>().Objects;
            if (!objects.TryGetValue(_key, out Dictionary<string, string> stored))
            {
                stored = new Dictionary<string, string>();
                objects[_key] = stored;
            }

            foreach (ILevelObjectDataUser user in _users)
            {
                stored[user.DataType.FullName] = JsonUtility.ToJson(user.ReadData());
            }
        }
    }
}
