using UnityEngine;

namespace MiniRacing
{
    public class Road : MonoBehaviour
    {
        [SerializeField]
        protected Vector3 localPointToRespawn;
        protected Vector3 halfBounds;
        protected Vector3 center;

        private void Awake()
        {
            TrackController.RegisterRoadBlock(this);
        }

        public Vector3 getPointToRespawn()
        {
            return this.transform.TransformPoint(localPointToRespawn);
        }

        private void Reset()
        {
            MeshCollider collider = this.GetComponent<MeshCollider>();
            halfBounds = collider.bounds.extents;
            center = this.transform.InverseTransformPoint(collider.bounds.center);
            localPointToRespawn = center + new Vector3(0.0f, halfBounds.y, 0.0f);
            //gameObject.layer = 
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(getPointToRespawn(), 0.5f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(this.transform.TransformPoint(center), 2.0f * halfBounds);

        }
    }
}