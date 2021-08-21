using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniRacing
{
    public class ThirdPersonCamera : BaseComponent
    {
        public enum FollowMode
        {
            FixedSmooth,
            FixedHard,
            AlwaysBehindSmooth,
            AlwaysBehindHard
        }

        public Transform target;
        public FollowMode followMode;
        [HideInInspector]
        [SerializeField]
        float distanceFromTarget = 20.0f;    // For AlwaysBehind Type
        [HideInInspector]
        [SerializeField]
        float lookAngle = 45.0f;                // For AlwaysBehind Type
        [SerializeField]
        [HideInInspector]
        Vector3 offsetFromTarget;               // For Fixed Type
        [SerializeField]
        float smoothness = 0.2f;                // For All Types

        Camera cam;
        Transform camTransform;

        Vector3 getSmoothedPos(Vector3 current, Vector3 target)
        {
            Vector3 curVel = Vector3.zero;
            return Vector3.SmoothDamp(current, target, ref curVel, smoothness * Time.deltaTime);
            //return Vector3.Lerp(current, target, smoothness * Time.fixedDeltaTime);
        }

        void FixedSmooth()
        {
            Vector3 desiredPos = target.position + offsetFromTarget;
            Vector3 smoothedPos = getSmoothedPos(camTransform.position, desiredPos);

            this.camTransform.position = smoothedPos;
            this.camTransform.LookAt(target);
        }
        void FixedHard()
        {
            Vector3 desiredPos = target.position + offsetFromTarget;
            this.camTransform.position = desiredPos;
            this.camTransform.LookAt(target);
        }
        void AlwaysBehindSmooth()
        {
            Vector3 targetForward = Vector3.ProjectOnPlane(target.TransformDirection(Vector3.forward).normalized, Vector3.up);
            Vector3 leftSideFromTarget = -Vector3.Cross(targetForward, Vector3.up);

            Quaternion lookAngleQuatern = Quaternion.AngleAxis(lookAngle, leftSideFromTarget);//target.TransformDirection(Vector3.right));
            Vector3 backDistanceVect = (-distanceFromTarget) * targetForward;
            Vector3 fromTargetToCamera = lookAngleQuatern * backDistanceVect;
            Vector3 desiredPos = target.position + fromTargetToCamera;
            Debug.DrawLine(target.position, target.position + fromTargetToCamera, Color.red);

            Vector3 smoothedPos = getSmoothedPos(camTransform.position, desiredPos);
            this.camTransform.position = smoothedPos;
            this.camTransform.LookAt(target);
        }
        void AlwaysBehindHard()
        {
            Vector3 targetForward = Vector3.ProjectOnPlane(target.TransformDirection(Vector3.forward).normalized, Vector3.up);
            Vector3 leftSideFromTarget = -Vector3.Cross(targetForward, Vector3.up);

            Quaternion lookAngleQuatern = Quaternion.AngleAxis(lookAngle, leftSideFromTarget);
            Vector3 backDistanceVect = (-distanceFromTarget) * targetForward;
            Vector3 fromTargetToCamera = lookAngleQuatern * backDistanceVect;
            Vector3 desiredPos = target.position + fromTargetToCamera;
            //Debug.DrawLine(target.position, target.position + target.TransformDirection(Vector3.forward), Color.green);
            //Debug.DrawLine(target.position, target.position + desiredPos, Color.red);

            this.camTransform.position = desiredPos;
            this.camTransform.LookAt(target);
        }

        void Start()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
                throw new System.Exception("NO_CAMERA_FOUND");

            camTransform = cam.transform;
        }

        void LateUpdate()
        {
            switch (followMode)
            {
                case FollowMode.FixedHard:
                    FixedHard();
                    break;

                case FollowMode.FixedSmooth:
                    FixedSmooth();
                    break;

                case FollowMode.AlwaysBehindHard:
                    AlwaysBehindHard();
                    break;

                case FollowMode.AlwaysBehindSmooth:
                    AlwaysBehindSmooth();
                    break;
            }
        }

    }
}