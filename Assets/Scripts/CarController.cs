using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniRacing
{
    public class CarController : BaseComponent
    {


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
        }
    }
}