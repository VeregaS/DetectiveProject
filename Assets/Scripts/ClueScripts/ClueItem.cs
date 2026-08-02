using UnityEngine;

namespace NodeZero.Interaction
{
    public class ClueItem : MonoBehaviour
    {
        [SerializeField] private ClueData _data;

        public ClueData Data => _data;

        public Vector3 OriginalPosition { get; private set; }
        public Quaternion OriginalRotation { get; private set; }

        private void Start()
        {
            OriginalPosition = transform.position;
            OriginalRotation = transform.rotation;
        }
    }
}