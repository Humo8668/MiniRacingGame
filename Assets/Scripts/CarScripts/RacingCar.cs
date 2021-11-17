using UnityEngine;

namespace MiniRacing.CarScripts
{
    public class RacingCar : BaseComponent
    {
        public enum DriveType
        {
            FrontWheelDrive,
            BackWheelDrive,
            AllWheelDrive
        }

        [SerializeField]
        protected float maxMotorTorque = 200f;
        [SerializeField]
        protected float maxBrakeTorque = 200f;
        [SerializeField]
        protected float maxHandBrakeTorque = 200f;
        [SerializeField]
        protected float maxSteeringAngle = 30f;
        [SerializeField]
        protected float steeringSpeed = 1.5f;   // Speed of steering, measured in degrees
        [SerializeField]
        DriveType driveType = DriveType.BackWheelDrive;

        [SerializeField]
        protected WheelCollider FrontLeftWheel;
        [SerializeField]
        protected WheelCollider FrontRightWheel;
        [SerializeField]
        protected WheelCollider BackLeftWheel;
        [SerializeField]
        protected WheelCollider BackRightWheel;
        [Header("Forward vector in local positioning")]
        [SerializeField]
        protected Vector3 forwardVector;


        [HideInInspector]
        public new Rigidbody rigidbody;
        const string CENTER_OF_MASS_TAGNAME = "CenterOfMass";
        const float ENGINE_SOUND_BASE_PITCH = 0.25f;
        bool handBrakePulled = false;
        AudioSource audioSrcComponent;


        private void Awake()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            if (this.rigidbody == null)
                throw new System.Exception("RigidBody is required!");
            audioSrcComponent = GetComponent<AudioSource>();
            if (this.rigidbody == null)
                throw new System.Exception("AudioSource is required!");
        }

        void Start()
        {
            foreach (Transform t in this.transform)
            {
                if (CENTER_OF_MASS_TAGNAME.Equals(t.tag))
                    this.rigidbody.centerOfMass = t.localPosition;
            }
        }

        /// <summary>
        /// Procedure to accelerate and brake the car. If accelerating vector of the car is opposite to moving vector of the car, then brakes the wheels.
        /// </summary>
        /// <param name="scale"></param>
        public void Move(float scale)
        {
            float averageRpm = (FrontLeftWheel.rpm + FrontRightWheel.rpm) / 2.0f;

            if (Mathf.Abs(averageRpm) < 0.01)
            {
                Brake(0.0f);
                Accelerate(scale);
                return;
            }
            if (Mathf.Abs(scale) < 0.01)
            {
                Accelerate(0.0f);
                Brake(0.0f);
                return;
            }

            if (averageRpm * scale > 0)
                Accelerate(scale);
            else
                Brake(Mathf.Abs(scale));
        }

        /// <summary>
        /// Procedure to give motor torque to the wheels instantly. 
        /// </summary>
        /// <param name="scale">Positive is forward accelerating, negative is backward.</param>
        public void Accelerate(float scale = 1.0f)
        {
            switch (driveType)
            {
                case DriveType.BackWheelDrive:
                    BackLeftWheel.motorTorque = scale * maxMotorTorque / 2;
                    BackRightWheel.motorTorque = scale * maxMotorTorque / 2;
                    break;
                case DriveType.FrontWheelDrive:
                    FrontLeftWheel.motorTorque = scale * maxMotorTorque / 2;
                    FrontRightWheel.motorTorque = scale * maxMotorTorque / 2;
                    break;
                case DriveType.AllWheelDrive:
                    BackLeftWheel.motorTorque = scale * maxMotorTorque / 4;
                    BackRightWheel.motorTorque = scale * maxMotorTorque / 4;
                    FrontLeftWheel.motorTorque = scale * maxMotorTorque / 4;
                    FrontRightWheel.motorTorque = scale * maxMotorTorque / 4;
                    break;
            }
        }

        /// <summary>
        /// Procedure to immitate brake of car. Only rear wheels work.
        /// </summary>
        /// <param name="scale"></param>
        public void Brake(float scale)
        {
            scale = Mathf.Clamp(scale, 0.0f, 1.0f);
            FrontLeftWheel.brakeTorque = scale * this.maxBrakeTorque / 2;
            FrontRightWheel.brakeTorque = scale * this.maxBrakeTorque / 2;
        }

        /// <summary>
        /// Procedure to immitate of pulling car's handbrake. Works in pair with |HandBrakeRelease| function.
        /// </summary>
        public void HandBrakePull()
        {
            if (handBrakePulled)
                return;
            BackLeftWheel.brakeTorque = maxHandBrakeTorque;
            BackRightWheel.brakeTorque = maxHandBrakeTorque;
            handBrakePulled = true;
        }

        /// <summary>
        /// Procedure to immitate of releasing car's handbrake. Works in pair with |HandBrakePull| function.
        /// </summary>
        public void HandBrakeRelease()
        {
            if (!handBrakePulled)
                return;
            BackLeftWheel.brakeTorque = 0.0f;
            BackRightWheel.brakeTorque = 0.0f;
            handBrakePulled = false;
        }

        /// <summary>
        /// Function to steer wheels of car. Negative scale is left side and vice versa.
        /// </summary>
        /// <param name="scale"></param>
        public void Steer(float scale)
        {
            scale = Mathf.Clamp(scale, -1.0f, 1.0f);
            float desiredSteering = scale * maxSteeringAngle;
            float currentSteering = FrontLeftWheel.steerAngle;
            float steeringAngle = 0.0f;
            if (Mathf.Abs(currentSteering - desiredSteering) < 0.001f)
                steeringAngle = currentSteering;
            else if (currentSteering < desiredSteering)
                steeringAngle = currentSteering + Mathf.Min(Time.deltaTime * steeringSpeed, Mathf.Abs(desiredSteering - currentSteering));
            else if (currentSteering > desiredSteering)
                steeringAngle = currentSteering - Mathf.Min(Time.deltaTime * steeringSpeed, Mathf.Abs(desiredSteering - currentSteering));

            if (Mathf.Abs(steeringAngle) < 0.001f)
                steeringAngle = 0.0f;

            FrontLeftWheel.steerAngle = steeringAngle;
            FrontRightWheel.steerAngle = steeringAngle;
        }

        /// <summary>
        /// Function to turn off collisions for all tags and layers. Runs by set of collisions of the car and sets parameter "isTrigger" to true.
        /// </summary>
        /// <param name="flag">True for disable, False for enable</param>
        public void DisableCollisions(bool flag)
        {
            Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.isTrigger = flag;
            }
        }

        /// <summary>
        /// Sets rigidbody's and childs rigidbodies |isKinematic| parameter to true if |flag| argument is true and vice-versa otherwise.
        /// </summary>
        /// <param name="flag"></param>
        public void DisableRigidbodies(bool flag)
        {
            Rigidbody[] bodies = this.gameObject.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody body in bodies)
            {
                body.isKinematic = flag;
                if(flag)
                {
                    body.velocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                }
            }
        }

        


        void FixedUpdate()
        {
        }

        void ControlEngineSound()
        {
            float pitch = 0.0f;

            switch (driveType)
            {
                case DriveType.FrontWheelDrive:
                    pitch = FrontLeftWheel.rpm / 2000;
                    break;
                case DriveType.BackWheelDrive:
                    pitch = BackLeftWheel.rpm / 2000;
                    break;
                case DriveType.AllWheelDrive:
                    pitch = Mathf.Max(FrontLeftWheel.rpm, BackLeftWheel.rpm) / 2000;
                    break;
            }
            audioSrcComponent.pitch = Mathf.Abs(pitch) + ENGINE_SOUND_BASE_PITCH;
        }

        private void Update()
        {
            ControlEngineSound();
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.TransformPoint(forwardVector), 0.1f);
            Gizmos.DrawLine(
                this.transform.TransformPoint(Vector3.zero),
                this.transform.TransformPoint(forwardVector));
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                this.transform.TransformPoint(Vector3.zero),
                this.transform.TransformPoint(Vector3.up) );
        }
    }
}