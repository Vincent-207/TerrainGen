using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CarController : MonoBehaviour
{
    public enum Axel
    {
        Front,
        Rear
    }
    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public float maxAccel = 30.0f;
    public float brakeAcceleration = 50.0f;

    public List<Wheel> wheels;
    Vector2 input;
    private Rigidbody carRB;
    public InputActionReference move;
    public float turnSensitivity = 1f, maxSteerAngle =30f;
    void Start()
    {
        carRB = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        GetInputs();
        AnimateWheels();
    }
    private void FixedUpdate()
    {
        Move();
        Steer();

    }

    void GetInputs()
    {
        input = move.action.ReadValue<Vector2>();
        // normalize different axis
        input.x = (Math.Abs(input.x) > 0) ? input.x / Math.Abs(input.x) : 0;
        input.y = (Math.Abs(input.y) > 0) ? input.y / Math.Abs(input.y) : 0;
        
    }
    void Steer()
    {
        foreach(var wheel in wheels)
        {
            if(wheel.axel == Axel.Front)
            {
                var _steerAngle = input.x * turnSensitivity * maxSteerAngle;
                float angle = wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
                // wheel.wheelModel.transform.localRotation = Quaternion.Euler(0f, angle, 90f);
            }
        }
    }
    void Move()
    {
        foreach(var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = input.y * maxAccel * Time.fixedDeltaTime;

        }
    }

    void AnimateWheels()
    {
        foreach(var wheel in wheels)
        {
            wheel.wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            Vector3 eulers = rot.eulerAngles;
            // eulers.z = 90f;
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = Quaternion.Euler(eulers);
            Vector3 localRot = wheel.wheelModel.transform.localRotation.eulerAngles;
            localRot.y = 0;
            // wheel.wheelModel.transform.localRotation = Quaternion.Euler(localRot);
            // localRot
        }
    }
 }
