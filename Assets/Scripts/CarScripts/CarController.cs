using System.Collections;
using UnityEngine;
using System;

namespace MiniRacing.CarScripts
{
    public class CarController : BaseComponent
    {
        const float RETURN_TO_TRACK_TIME_SEC = 0.7f;
        Boolean canReturnToTrack = true;
        IEnumerator ReturningToTrack(Action callback = null)
        {
            Vector3 currentPos = car.transform.position;
            Vector3 desiredPos = TrackController.GetNearestTrackPoint(car.transform.position) + Vector3.up;
            
            Quaternion currentRotation = car.transform.rotation;
            Quaternion desiredRotation = Quaternion.FromToRotation(car.transform.up, Vector3.up) * currentRotation;

            float t = 0.0f;
            
            while(t < 1.0f)//(Quaternion.Angle(currentRotation, desiredRotation) >= 0.01 || Vector3.Distance(currentPos, desiredPos) >= 0.01)
            {
                t += (1 / RETURN_TO_TRACK_TIME_SEC) * Time.deltaTime;
                car.transform.rotation = Quaternion.Slerp(currentRotation, desiredRotation, t);
                currentRotation = car.transform.rotation;

                car.transform.position = Vector3.Lerp(currentPos, desiredPos, t);
                currentPos = car.transform.position;
                yield return Time.deltaTime;
            }


            if (callback != null)
                callback();

            yield return null;
        }

        public void ReturnCarToTrack()
        {
            if (!canReturnToTrack)
                return;
            canReturnToTrack = false;

            car.rigidbody.velocity = Vector3.zero;
            car.rigidbody.angularVelocity = Vector3.zero;
            car.DisableCollisions(true);
            car.DisableRigidbodies(true);
            
            StartCoroutine(
                ReturningToTrack(() => {
                    car.DisableRigidbodies(false);
                    car.DisableCollisions(false);
                    canReturnToTrack = true;
                })
            );
        }

        [SerializeField]
        RacingCar car;
        void Start()
        {

        }

        void Update()
        {
            float moveScale = Input.GetAxis("Vertical");
            float steerScale = Input.GetAxis("Horizontal");

            car.Move(moveScale);
            car.Steer(steerScale);

            if (Input.GetKey(KeyCode.Space))
                car.HandBrakePull();
            else
                car.HandBrakeRelease();

            if (Input.GetKey(KeyCode.R))
                ReturnCarToTrack();

        }
    }
}