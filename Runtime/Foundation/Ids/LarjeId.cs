using UnityEngine;

namespace Larje.Core.Ids
{
    public abstract class LarjeId : ScriptableObject
    {
        [SerializeField] private string key;

        public string Key => string.IsNullOrEmpty(key) ? name : key;

        public override string ToString()
        {
            return name;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(key))
            {
                key = name;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
