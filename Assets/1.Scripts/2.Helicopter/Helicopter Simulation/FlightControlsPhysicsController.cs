using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.HelicopterSimulation
{
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(FlightControlsPositionController))]
  [RequireComponent(typeof(EngineController))]
  public class FlightControlsPhysicsController : MonoBehaviour
  {
    // This script applies the flight controls forces to the helicopter body.

    Rigidbody rb;
    FlightControlsPositionController flightControlsPosition;
    EngineController engineController;

    [SerializeField]
    [Tooltip("The Transform of the main rotor. This is used to define where the forces from the collective lever and from the cyclic stick are applied.")]
    Transform mainRotorTransform;

    [SerializeField]
    [Tooltip("Diameter of the main rotor (in meters). Thi is used to define where the forces from the cyclic stick are applied.")]
    float mainRotorDiameter = 10.2f;

    [SerializeField]
    [Tooltip("The Transform of the tail rotor. This is used to define where the forces from the pedals are applied.")]
    Transform tailRotorTransform;

    [SerializeField]
    [Tooltip("Percentge of lift that is also used to pitch and roll the helicopter. A value of zero would mean it is impossible to change the helicopter attitude and a value of 100 would make the cyclic stick the most sensitive possible.")]
    [Range(0, 100)]
    float liftCyclicPercentage = 50.0f;

    [SerializeField]
    [Tooltip("The lift (in Newtons) given by the main rotor when the collective lever is at its minimum position.")]
    float minimumLift = 13000;

    [SerializeField]
    [Tooltip("The lift (in Newtons) given by the main rotor when the collective lever is at its maximum position.")]
    float maximumLift = 26500;

    [SerializeField]
    [Tooltip("Factor for a filter applied to the collective lever so the helicopter does not react instantly to a collective input.")]
    float collectiveDamping = 1.0f;

    [SerializeField]
    [Tooltip("Maximum force (in Newtons) given by the tail rotor.")]
    float maximumTailRotorForce = 1000.0f;

    [SerializeField]
    [Tooltip("When TRUE, the torque from the main rotor rotation is simulated.")]
    bool rotorTorqueSimulated = true;

    [SerializeField]
    [Tooltip("Used only if the rotor torque is simulated. Maximum torque (in Newtons-Meters) caused by the main rotor rotation.")]
    float maxRotorTorque = 250;

    [SerializeField]
    [Tooltip("How strong the pendulum effect is. A value of 0 means the helicopter will not come by itself to horizontal attitude if cyclic stick is at its neutral position in flight.A value of 1 means it is difficult to change the helicopter's attitude.")]
    [Range(0, 1)]
    float pendulumEffectFactor = 0.25f;

    [SerializeField]
    [Tooltip("How much the collective pitch affects the rotor speed because of air resistance and change in power required.For a real collective lever this parameter would be 1.For a gamepad joystick 0.2 gives realistic results.")]
    float rotorSpeedChangeFactor = 0.2f;

    [SerializeField]
    [Tooltip("Factor for filter applied to the rotor speed change.")]
    float rotorSpeedDamping = 1.0f;

    [SerializeField]
    [Tooltip("Ground effect parameters")]
    HelicopterSetup.GroundEffectParameters groundEffectParameters = new HelicopterSetup.GroundEffectParameters(47.85154697f, -4.2666996f);

    [SerializeField]
    [Tooltip("Layers for terrain collision check (excluding player layer) used for ground effect")]
    LayerMask terrainLayers = 0;

    [SerializeField]
    [Tooltip("Paramters for Vortex Ring State simulation")]
    HelicopterSetup.VortexRingStatePerameters vrsParameters = new HelicopterSetup.VortexRingStatePerameters(-700, 30, 0.75f, 0.5f);


    float actualLift = 0;
    float desiredLift = 0;

    float actualRotorSpeed = 0;

    void Start()
    {
      // Find required components
      rb = GetComponent<Rigidbody>();
      flightControlsPosition = GetComponent<FlightControlsPositionController>();
      engineController = GetComponent<EngineController>();
    }

    void FixedUpdate()
    {
      // Compute and apply forces
      processMainRotorForces(rotorEfficiency());
      processPedalsForces();
      processPendulumTorques();

      // Compute rotor speed (displayed on PFD)
      float desiredRotorSpeed = 0.0f;

      if (desiredLift - actualLift >= 0.0f && !Mathf.Approximately(0, desiredLift))
      {
        // If lift increases, the rotor needs more power and its speed will momentarily be reduced.
        desiredRotorSpeed = (engineController.getEngineSpeed() / 100.0f) * (1 - (desiredLift - actualLift) / desiredLift * rotorSpeedChangeFactor) * 100.0f;
      }
      else if (!Mathf.Approximately(0, actualLift))
      {
        // If lift decreases, the rotor needs less power and its speed will momentarily be increased.
        desiredRotorSpeed = (engineController.getEngineSpeed() / 100.0f) * (1 - (desiredLift - actualLift) / actualLift * rotorSpeedChangeFactor) * 100.0f;
      }

      // Apply filter on rotor speed
      actualRotorSpeed = Mathf.Lerp(actualRotorSpeed, desiredRotorSpeed, rotorSpeedDamping * Time.fixedDeltaTime);
    }

    float rotorEfficiency()
    {
      // Check if we are in VRS
      float vSpeed = rb.velocity.y * 196.85f; // feet/min
      float hSpeed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z) * 1.94384f; // Knots

      float efficiency = 1;
      if (vSpeed < vrsParameters.verticalSpeed && hSpeed < vrsParameters.horizontalSpeed)
      {
        // NOTE : Could not find anywhere online how bad VRS actually affects main rotor efficiency.
        //        The following is a guess at best...

        if (vSpeed < 4 * vrsParameters.verticalSpeed)
        {
          float vSpeedEfficiency = vrsParameters.E4;
          float hSpeedEfficiency = (0.5f + (hSpeed) / vrsParameters.horizontalSpeed * 0.5f);
          efficiency = vSpeedEfficiency * hSpeedEfficiency;
        }
        else if (vSpeed < 2 * vrsParameters.verticalSpeed)
        {
          float vSpeedEfficiency = ((vrsParameters.E4 - vrsParameters.E2) / (2 * vrsParameters.verticalSpeed) * vSpeed + 2 * vrsParameters.E2 - vrsParameters.E4);
          float hSpeedEfficiency = (0.5f + (hSpeed) / vrsParameters.horizontalSpeed * 0.5f);
          efficiency = vSpeedEfficiency * hSpeedEfficiency;
        }
        else // if (vSpeed < vrsParameters.vrsVerticalSpeed)
        {
          float vSpeedEfficiency = ((vrsParameters.E2 - 1) / vrsParameters.verticalSpeed * vSpeed + 2 - vrsParameters.E2);
          float hSpeedEfficiency = (0.5f + (hSpeed) / vrsParameters.horizontalSpeed * 0.5f);
          efficiency = vSpeedEfficiency * hSpeedEfficiency;
        }
      }

      return efficiency;
    }

    void processMainRotorForces(float iEfficiency)
    {
      desiredLift = iEfficiency * ((flightControlsPosition.collectivePos() + 1) / 2 * (maximumLift - minimumLift) + minimumLift) * (engineController.getEngineSpeed() / 100.0f);

      // Apply filter on lift
      actualLift = Mathf.Lerp(actualLift, desiredLift, collectiveDamping * Time.fixedDeltaTime);

      // How much lift is also used to change the helicopter attitude
      float liftOnly = actualLift * (100.0f - liftCyclicPercentage) / 100.0f;
      float cyclicLift = actualLift * liftCyclicPercentage / 100.0f;

      // The lift force used to change helicopter attitude is evenly distibuted for pitch (Y) and roll (X) change
      float forceX = 0.5f * cyclicLift;
      float forceY = 0.5f * cyclicLift;

      // Compute forces applied on the left and right sides of the main rotor
      float forceXleft = (flightControlsPosition.cyclicXPos() + 1.0f) / 2.0f * forceX;
      float forceXright = (-flightControlsPosition.cyclicXPos() + 1.0f) / 2.0f * forceX;

      // Compute forces applied on the front and back of the main rotor
      float forceYfront = (-flightControlsPosition.cyclicYPos() + 1.0f) / 2.0f * forceY;
      float forceYrear = (flightControlsPosition.cyclicYPos() + 1.0f) / 2.0f * forceY;

      if (null != mainRotorTransform)
      {
        // Apply forces

        // Attitude changing forces are applied away from the main rotor center at a distance of half rotor radius
        rb.AddForceAtPosition(mainRotorTransform.up * forceXright, mainRotorTransform.position + mainRotorTransform.right * mainRotorDiameter / 4.0f);
        rb.AddForceAtPosition(mainRotorTransform.up * forceXleft, mainRotorTransform.position - mainRotorTransform.right * mainRotorDiameter / 4.0f);
        rb.AddForceAtPosition(mainRotorTransform.up * forceYfront, mainRotorTransform.position + mainRotorTransform.forward * mainRotorDiameter / 4.0f);
        rb.AddForceAtPosition(mainRotorTransform.up * forceYrear, mainRotorTransform.position - mainRotorTransform.forward * mainRotorDiameter / 4.0f);

        // Lift only force is applied at main rotor center
        rb.AddForceAtPosition(mainRotorTransform.up * liftOnly, mainRotorTransform.position);
      }
      else
      {
        // if we dont know where the main rotor is, apply force around center of mass
        rb.AddForceAtPosition(transform.up * forceXright, transform.TransformPoint(rb.centerOfMass) + transform.right * mainRotorDiameter / 4.0f);
        rb.AddForceAtPosition(transform.up * forceXleft, transform.TransformPoint(rb.centerOfMass) - transform.right * mainRotorDiameter / 4.0f);
        rb.AddForceAtPosition(transform.up * forceYfront, transform.TransformPoint(rb.centerOfMass) + transform.forward * mainRotorDiameter / 4.0f);
        rb.AddForceAtPosition(transform.up * forceYrear, transform.TransformPoint(rb.centerOfMass) - transform.forward * mainRotorDiameter / 4.0f);
        rb.AddForceAtPosition(transform.up * liftOnly, transform.TransformPoint(rb.centerOfMass));
      }

      if (rotorTorqueSimulated)
      {
        // Simulate rotor torque if desired
        float torque = (actualLift - minimumLift) / (maximumLift - minimumLift) * maxRotorTorque;
        rb.AddRelativeTorque(0, torque, 0);
      }

      processGroundEffect(actualLift);
    }

    void processPedalsForces()
    {
      if (null != tailRotorTransform)
      {
        // Compute and apply tail rotor induced torque on body
        float tailRotorForce = -flightControlsPosition.pedalsPos() * maximumTailRotorForce * (engineController.getEngineSpeed() / 100.0f);
        rb.AddForceAtPosition(tailRotorTransform.right * tailRotorForce, tailRotorTransform.position);
      }
      else
      {
        // if we don't know where the tail rotor is, assume it is 10 meters behind the center of mass as an approximation
        float tailRotorForce = -flightControlsPosition.pedalsPos() * maximumTailRotorForce * (engineController.getEngineSpeed() / 100.0f);
        float tailRotorTorque = -tailRotorForce * 10.0f;
        rb.AddRelativeTorque(0, tailRotorTorque, 0);
      }
    }

    void processPendulumTorques()
    {
      // Compute pendulum effect. This effect will automatically bring the helicopter back to horizontal poisition
      // after some time if the cyclic stick is at its neutral position.

      // Need to know pitch and roll
      float pitch = Mathf.DeltaAngle(0, -transform.rotation.eulerAngles.x);
      float roll = Mathf.DeltaAngle(0, -transform.rotation.eulerAngles.z);

      if (null != mainRotorTransform)
      {
        // compute and apply torques
        float TorqueX = pendulumEffectFactor * actualLift * Mathf.Sin((pitch - mainRotorTransform.localEulerAngles.x) * Mathf.Deg2Rad);
        float TorqueZ = pendulumEffectFactor * actualLift * Mathf.Sin(roll * Mathf.Deg2Rad);
        rb.AddRelativeTorque(TorqueX, 0f, TorqueZ);
      }
      else
      {
        // if we don't know the main rotor transform, assume the main rotor is flat
        // (i.e. no angle relative to the ground when H/C is on ground)
        float TorqueX = pendulumEffectFactor * actualLift * Mathf.Sin(pitch * Mathf.Deg2Rad);
        float TorqueZ = pendulumEffectFactor * actualLift * Mathf.Sin(roll * Mathf.Deg2Rad);
        rb.AddRelativeTorque(TorqueX, 0f, TorqueZ);
      }
    }

    void processGroundEffect(float iLift)
    {
      if (null != mainRotorTransform)
      {
        float mainRotorHeightAboveGroundInMeters = 0;

        RaycastHit hit;
        if (Physics.Raycast(mainRotorTransform.position, new Vector3(0, -1, 0), out hit, mainRotorDiameter * 2, terrainLayers)) // above twice the rotor diameter, the ground effect is negelctable
        {
          mainRotorHeightAboveGroundInMeters = hit.distance;

          float mainRotorHeightInRotorDiameters = mainRotorHeightAboveGroundInMeters / mainRotorDiameter;

          float groundEffectPercentageGain = groundEffectParameters.a * Mathf.Exp(groundEffectParameters.b * mainRotorHeightInRotorDiameters);
          float increasedLift = iLift * groundEffectPercentageGain / 100;

          // Apply force at center of gravity
          rb.AddForce(Vector3.up * increasedLift);
        }
      }
      else
      {
        float mainRotorHeightAboveGroundInMeters = 0;
        RaycastHit hit;
        if (Physics.Raycast(transform.TransformPoint(rb.centerOfMass), new Vector3(0, -1, 0), out hit, mainRotorDiameter * 2, terrainLayers)) // above twice the rotor diameter, the ground effect is negelctable
        {
          mainRotorHeightAboveGroundInMeters = 3.5f + hit.distance;

          float mainRotorHeightInRotorDiameters = mainRotorHeightAboveGroundInMeters / mainRotorDiameter;

          float groundEffectPercentageGain = groundEffectParameters.a * Mathf.Exp(groundEffectParameters.b * mainRotorHeightInRotorDiameters);
          float increasedLift = iLift * groundEffectPercentageGain / 100;

          // Apply force at center of gravity
          rb.AddForce(Vector3.up * increasedLift);
        }
      }
    }

    // Returns the rotor speed (affected by power demand changes)
    public float getRotorSpeed()
    {
      return actualRotorSpeed;
    }

    public Vector3 getTailRotorPosition()
    {
      // if no tailRotorTransform is defined, assume tail rotor is 5 meters behind transform's position
      return null != tailRotorTransform ? tailRotorTransform.position : transform.position - transform.forward * 5;
    }
  }
}