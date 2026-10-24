using UnityEngine;

namespace Larje.Core.Effects
{
    public sealed class FixedRotation : MonoBehaviour
    {
        [SerializeField] private Vector3 rotation;
        [SerializeField] private Space space = Space.World;
        [SerializeField] private UpdateMode updateMode = UpdateMode.Late;

        private void Update()
        {
            if (updateMode == UpdateMode.Normal)
            {
                Apply();
            }
        }

        private void FixedUpdate()
        {
            if (updateMode == UpdateMode.Fixed)
            {
                Apply();
            }
        }

        private void LateUpdate()
        {
            if (updateMode == UpdateMode.Late)
            {
                Apply();
            }
        }

        private void Apply()
        {
            if (space == Space.World)
            {
                transform.rotation = Quaternion.Euler(rotation);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(rotation);
            }
        }

        private enum UpdateMode
        {
            Normal,
            Fixed,
            Late
        }
    }
}
