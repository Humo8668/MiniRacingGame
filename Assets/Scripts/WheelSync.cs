using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniRacing
{
    /// <summary>
    /// Functionality for syncronizing wheel mesh with wheel collider
    /// </summary>
    public class WheelSync : BaseComponent
    {
        [SerializeField]
        WheelCollider wheelCollider;
        Vector3 initPosition;   // Initial position of the wheel mesh. Necessary for relational positioning of wheel.
        Quaternion initRotation; // Initial position of the wheel mesh. Necessary for relational positioning of wheel.

        Quaternion rollingRot;
        Quaternion steeringRot;

        void Start()
        {
            if (wheelCollider == null)
            {
                enabled = false;
                throw new System.Exception("Wheel collider not set");
            }
            initPosition = this.transform.localPosition;
            initRotation = this.transform.localRotation;
            wheelCollider.ConfigureVehicleSubsteps(5, 12, 15);

            rollingRot = new Quaternion(0,0,0,1.0f);
            steeringRot = new Quaternion(0, 0, 0, 1.0f);
        }

        void LateUpdate()
        {
            Vector3 wheelPose;
            Quaternion wheelRotation;
            //wheelCollider.motorTorque = 100;
            wheelCollider.GetWorldPose(out wheelPose, out wheelRotation);
            Vector3 localPose = wheelCollider.transform.InverseTransformPoint(wheelPose);
            Vector3 offset = localPose - wheelCollider.center;

            float offsetValue = Vector3.Dot(offset, Vector3.up);

            this.transform.localPosition = initPosition + offsetValue * Vector3.up;

            rollingRot *= Quaternion.AngleAxis(wheelCollider.rpm / 60 * 360 * Time.deltaTime, Vector3.right);
            steeringRot = initRotation * Quaternion.AngleAxis(wheelCollider.steerAngle, Vector3.up);

            this.transform.localRotation = steeringRot;
            this.transform.localRotation *= rollingRot;

            //this.transform.localRotation = initRotation *  wheelRotation;
            /*this.transform.localRotation = Quaternion.Euler(new Vector3(
                this.transform.localRotation.eulerAngles.x + wheelCollider.rpm / 60 * 360 * Time.deltaTime,  // x
                (initRotation * Quaternion.AngleAxis(wheelCollider.steerAngle, Vector3.up)).eulerAngles.y, // y
                0.0f) // z 
            );*/
        }
    }
}