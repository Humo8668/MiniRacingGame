using UnityEngine;

namespace MiniRacing
{
    public class BaseComponent : MonoBehaviour
    {
        protected Transform thisTransform;
        private void Awake()
        {
            thisTransform = this.transform;
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}