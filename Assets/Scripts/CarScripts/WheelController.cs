using System.Collections;
using UnityEngine;
using System;


namespace MiniRacing.CarScripts
{
    /// <summary>
    /// Functionality for syncronizing wheel mesh with wheel collider
    /// </summary>
    public class WheelController : BaseComponent
    {
        [SerializeField]
        WheelCollider wheelCollider;
        //-------------------------------------------------------------------------------------------------
        [Header("FX instances")]
        [SerializeField]
        protected GameObject FX_slipOnAsphalt;
        protected ParticleSystem fx_slipOnAsphalt;
        //-------------------------------------------------------------------------------------------------
        [Header("Sound effect instances")]
        [SerializeField]
        protected AudioClip Sound_slipOnAsphalt;
        protected AudioSource as_slipOnAsphalt;
        //-------------------------------------------------------------------------------------------------
        [Header("Tire slip trace parameters")]
        protected TireSlipTrail tireTrailObject;
        [SerializeField]
        protected Material tireSlipTraceMaterial;
        [SerializeField]
        [Range(0.0f, 1.0f)]
        protected float tireTraceWidth = 0.2f;
        [SerializeField]
        protected float traceDisappearingTime = 60;
        [SerializeField]
        protected float distanceBetweenTracePoints = 0.2f;
        [SerializeField]
        [Tooltip("Units to draw trace beyonf floor. For resolving co-overlying effect")]
        protected float unitsBeyondFloor = 0.01f;
        //-------------------------------------------------------------------------------------------------

        Vector3 initPosition;   // Initial position of the wheel mesh. Necessary for relational positioning of wheel.
        Quaternion initRotation; // Initial position of the wheel mesh. Necessary for relational positioning of wheel.

        Quaternion rollingRot;
        Quaternion steeringRot;

        void SetupTireSlipEffects()
        {
            if (FX_slipOnAsphalt != null)
            {
                if (!FX_slipOnAsphalt.TryGetComponent<ParticleSystem>(out fx_slipOnAsphalt))
                    throw new System.Exception("FX_smokeOnSlip doesn't have <ParticleSystem> component!");
            }

            if (Sound_slipOnAsphalt != null)
            {
                as_slipOnAsphalt = gameObject.AddComponent<AudioSource>();
                as_slipOnAsphalt.enabled = true;
                as_slipOnAsphalt.loop = true;
                as_slipOnAsphalt.clip = Sound_slipOnAsphalt;
                as_slipOnAsphalt.volume = 0.0f;
                as_slipOnAsphalt.Play();
            }

            tireTrailObject = gameObject.AddComponent<TireSlipTrail>();
            tireTrailObject.Material = tireSlipTraceMaterial;
            tireTrailObject.TrailWidth = tireTraceWidth;
            tireTrailObject.TraceDisappearTime = traceDisappearingTime;
            tireTrailObject.DistanceBetweenTracePoints = distanceBetweenTracePoints;
            tireTrailObject.UnitsBeyondSurface = unitsBeyondFloor;
            tireTrailObject.StopEmitting();
        }

        void Start()
        {
            if (wheelCollider == null)
            {
                enabled = false;
                throw new System.Exception("Wheel collider not set");
            }
            initPosition = this.transform.localPosition;
            initRotation = this.transform.localRotation;
            rollingRot = new Quaternion(0, 0, 0, 1.0f);
            steeringRot = new Quaternion(0, 0, 0, 1.0f);
            wheelCollider.ConfigureVehicleSubsteps(5, 12, 15);

            SetupTireSlipEffects();
        }

        bool voluneIsChanging = false;
        IEnumerator changeVolume(float volume, Action onEnd = null)
        {
            if (!voluneIsChanging)
            {
                voluneIsChanging = true;
                float a = as_slipOnAsphalt.volume;
                float b = volume;
                float raiseTime = 0.15f;
                float t = 0;
                while (t < 1.0f)
                {
                    t += Time.deltaTime / raiseTime;
                    as_slipOnAsphalt.volume = Mathf.Lerp(a, b, t);
                    yield return null;
                }

                if (onEnd != null)
                    onEnd();

                voluneIsChanging = false;
                yield return null;
            }
        }

        /// <summary>
        /// Processes wheel sliding.
        /// </summary>
        void controlWheelSlidingEffect()
        {
            if (FX_slipOnAsphalt != null 
                && Sound_slipOnAsphalt != null 
                && tireTrailObject != null)
            {
                WheelHit wh;
                if (wheelCollider.GetGroundHit(out wh))
                {
                    if (Mathf.Abs(wh.forwardSlip) > 0.5f || Mathf.Abs(wh.sidewaysSlip) > 0.5f)
                    {
                        float summarySlip = Mathf.Abs(wh.forwardSlip) + Mathf.Abs(wh.sidewaysSlip);
                        //Debug.Log("Sliding: { forward: " + wh.forwardSlip + ", sideways: " + wh.sidewaysSlip + "}");
                        //---
                        Instantiate(FX_slipOnAsphalt, wh.point, wheelCollider.transform.rotation); // FX --- smoke of the tire
                        //---
                        StartCoroutine(changeVolume(Mathf.Clamp01(summarySlip)));
                        //---
                        if(!tireTrailObject.isEmitting())
                            tireTrailObject.StartEmitting();

                    }
                    else
                    {
                        StartCoroutine(changeVolume(0f));
                        if(tireTrailObject.isEmitting())
                            tireTrailObject.StopEmitting();
                    }
                    tireTrailObject.SetPosition(wh.point);
                }
                else
                {
                    StartCoroutine(changeVolume(0f));
                    tireTrailObject.SetPosition(Vector3.ProjectOnPlane(transform.position, Vector3.up));
                    if (tireTrailObject.isEmitting())
                        tireTrailObject.StopEmitting();
                }    
            }
        }

        /// <summary>
        /// Sets correct position and rotation to the mesh of the wheel according to wheelCollider component.
        /// </summary>
        void SyncronizeWheelMesh()
        {
            Vector3 wheelPose;
            Quaternion wheelRotation;
            wheelCollider.GetWorldPose(out wheelPose, out wheelRotation);
            Vector3 localPose = wheelCollider.transform.InverseTransformPoint(wheelPose);
            Vector3 offset = localPose;// - (wheelCollider.transform.localPosition + wheelCollider.center);

            float offsetValue = Vector3.Dot(offset, Vector3.up);

            this.transform.localPosition = initPosition + offsetValue * Vector3.up;

            rollingRot *= Quaternion.AngleAxis(wheelCollider.rpm / 60 * 360 * Time.deltaTime, Vector3.right);
            steeringRot = initRotation * Quaternion.AngleAxis(wheelCollider.steerAngle, Vector3.up);

            this.transform.localRotation = steeringRot;
            this.transform.localRotation *= rollingRot;
        }

        private void FixedUpdate()
        {
            controlWheelSlidingEffect();
        }

        void LateUpdate()
        {
            SyncronizeWheelMesh();
        }
    }
}