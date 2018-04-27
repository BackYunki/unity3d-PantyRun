using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HeliSimPack.HelicopterSimulation
{
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(FuelController))]
  [RequireComponent(typeof(FlightControlsPhysicsController))]
  public class RigidBodyController : MonoBehaviour
  {
    // This script is responsible of overriding Unity's Rigidbody parameters including its mass and center
    // of mass.It also applies a more physically accurate drag.

    [SerializeField]
    [Tooltip("Weight of the helicopter body alone (not including fuel, cargo or crew). In pounds.")]
    float emptyWeight = 3208.0f;

    [SerializeField]
    [Tooltip("Darg information in the relative Z axis")]
    HelicopterSetup.dragInfo dragInfoFront = new HelicopterSetup.dragInfo(0.15f, 12.5f);

    [SerializeField]
    [Tooltip("Darg information in the relative X axis")]
    HelicopterSetup.dragInfo dragInfoSide = new HelicopterSetup.dragInfo(0.47f, 40.4f);

    [SerializeField]
    [Tooltip("Darg information in the relative Y axis")]
    HelicopterSetup.dragInfo dragInfoTop = new HelicopterSetup.dragInfo(4.0f, 30.75f);

    [SerializeField]
    [Tooltip("For keel effect. Darg information in the relative X axis for the tail only")]
    HelicopterSetup.dragInfo dragInfoTail = new HelicopterSetup.dragInfo(0.8f, 2.0f);

    [SerializeField]
    [Tooltip("Same as Unity's Rigidbody property")]
    float angularDrag = 2.0f;

    [SerializeField]
    [Tooltip("Inertia tensor of the helicopter")]
    Vector3 inertiaTensor = new Vector3(10000, 10000, 10000);

    [SerializeField]
    [Tooltip("Rotation of the inertia tensor of the helicopter")]
    Quaternion inertiaTensorRotation = Quaternion.identity;

    [SerializeField]
    [Tooltip("Same as Unity's Rigidbody property")]
    RigidbodyInterpolation interpolation = RigidbodyInterpolation.Extrapolate;

    [SerializeField]
    [Tooltip("Same as Unity's Rigidbody property")]
    CollisionDetectionMode collisionDetection = CollisionDetectionMode.Discrete;

    [SerializeField]
    [Tooltip("Position of the center of mass of the Helicopter only (without fuel, pilots or cargo) (used to compute the actual center of mass)")]
    Transform emptyCenterOfMass;

    [SerializeField]
    [Tooltip("Position of pilot (used to compute the actual center of mass)")]
    Transform pilotPosition;

    [SerializeField]
    [Tooltip("in pounds (used to compute the actual center of mass)")]
    float pilotWeight = 175.0f;


    [SerializeField]
    [Tooltip("Position of copilot (used to compute the actual center of mass)")]
    Transform copilotPosition;

    [SerializeField]
    [Tooltip("in pounds (used to compute the actual center of mass)")]
    float copilotWeight = 175.0f;

    [SerializeField]
    [Tooltip("Position of cargo (used to compute the actual center of mass)")]
    Transform cargoPosition;

    [SerializeField]
    [Tooltip("In pounds (used to compute the actual center of mass)")]
    float cargoWeight = 500.0f;

    Rigidbody rb;
    FuelController fuelController;
    FlightControlsPhysicsController fcPhysicsController;

    float previousUpdate = 0.0f;

    const float kilograms2pounds = 2.2046226218f;

    Vector3 lastVelocity = new Vector3(0, 0, 0);
    Vector3 acceleration;

    void Start()
    {
      // Find required components
      rb = gameObject.GetComponent<Rigidbody>();
      fuelController = GetComponent<FuelController>();
      fcPhysicsController = GetComponent<FlightControlsPhysicsController>();

      // Initialize rigidbody's parameters
      rb.useGravity = true;
      rb.isKinematic = false;
      rb.drag = 0f;
      rb.angularDrag = angularDrag;
      rb.interpolation = interpolation;
      rb.collisionDetectionMode = collisionDetection;
      rb.constraints = RigidbodyConstraints.None;

      float totalWeight = emptyWeight + pilotWeight + copilotWeight + cargoWeight + fuelController.totalFuelRemaining();
      rb.mass = totalWeight / kilograms2pounds;

      rb.inertiaTensor = inertiaTensor;
      rb.inertiaTensorRotation = inertiaTensorRotation;

      // Compute actual center of mass
      computeCenterOfMass();
    }

    void FixedUpdate()
    {
      // no need to update center of mass more frequently than once per second
      if (Time.time - previousUpdate >= 1.0f)
      {
        computeCenterOfMass();
        previousUpdate = Time.time;
      }

      applyRealDrag();

      acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
      lastVelocity = rb.velocity;
    }

    void computeCenterOfMass()
    {
      // empty + pilot + copilot + all fuel tanks + cargo

      float totalWeight = emptyWeight + pilotWeight + copilotWeight + cargoWeight + fuelController.totalFuelRemaining();
      rb.mass = totalWeight / kilograms2pounds;

      Vector3 emptyCenterOfMassPos = transform.position;
      if (null != emptyCenterOfMass)
      {
        emptyCenterOfMassPos = emptyCenterOfMass.position;
      }

      Vector3 pilotPos = transform.position;
      if (null != pilotPosition)
      {
        pilotPos = pilotPosition.position;
      }

      Vector3 copilotPos = transform.position;
      if (null != copilotPosition)
      {
        copilotPos = copilotPosition.position;
      }

      Vector3 cargoPos = transform.position;
      if (null != cargoPosition)
      {
        cargoPos = cargoPosition.position;
      }

      // Actual center of mass is the average of all centers of mass
      Vector3 centerOfMass = transform.InverseTransformPoint(emptyCenterOfMassPos) * emptyWeight;
      centerOfMass += transform.InverseTransformPoint(pilotPos) * pilotWeight;
      centerOfMass += transform.InverseTransformPoint(copilotPos) * copilotWeight;
      centerOfMass += transform.InverseTransformPoint(cargoPos) * cargoWeight;

      foreach (HelicopterSetup.fuelTank tank in fuelController.fuelTanks)
      {
        centerOfMass += transform.InverseTransformPoint(tank.fuelTankPosition.position) * tank.fuelQuantity;
      }

      centerOfMass /= totalWeight;

      rb.centerOfMass = centerOfMass;
    }

    void applyRealDrag()
    {
      // Apply real drag based on velocity

      Vector3 relativeVelocity = transform.InverseTransformDirection(rb.velocity);

      float frontDrag = Mathf.Approximately(relativeVelocity.z, 0f) ? 0f : dragForce(relativeVelocity.z, dragInfoFront, rb.position.y) * relativeVelocity.z / Mathf.Abs(relativeVelocity.z);
      float sideDrag = Mathf.Approximately(relativeVelocity.x, 0f) ? 0f : dragForce(relativeVelocity.x, dragInfoSide, rb.position.y) * relativeVelocity.x / Mathf.Abs(relativeVelocity.x);
      float topDrag = Mathf.Approximately(relativeVelocity.y, 0f) ? 0f : dragForce(relativeVelocity.y, dragInfoTop, rb.position.y) * relativeVelocity.y / Mathf.Abs(relativeVelocity.y);
      float tailDrag = Mathf.Approximately(relativeVelocity.x, 0f) ? 0f : dragForce(relativeVelocity.x, dragInfoTail, rb.position.y) * relativeVelocity.x / Mathf.Abs(relativeVelocity.x);

      rb.AddForce(-transform.forward * frontDrag);
      rb.AddForce(-transform.right * sideDrag);
      rb.AddForce(-transform.up * topDrag);
      rb.AddForceAtPosition(-transform.right * tailDrag, fcPhysicsController.getTailRotorPosition());
    }

    float dragForce(float iVelocity, HelicopterSetup.dragInfo iDragInfo, float iAltitude)
    {
      // using average air density
      return 0.5f * iDragInfo.dragCoefficient * 1.225f * iDragInfo.sectionArea * iVelocity * iVelocity;
    }

    public Vector3 getAcceleration()
    {
      return acceleration;
    }
  }
}