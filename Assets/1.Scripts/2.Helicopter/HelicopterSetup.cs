using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack
{
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(HelicopterSimulation.RigidBodyController))]
  [RequireComponent(typeof(HelicopterSimulation.SoundController))]
  [RequireComponent(typeof(HelicopterSimulation.RotorRotator))]
  [RequireComponent(typeof(HelicopterSimulation.FuelController))]
  [RequireComponent(typeof(HelicopterSimulation.EngineController))]
  [RequireComponent(typeof(HelicopterSimulation.FlightControlsPhysicsController))]
  [RequireComponent(typeof(HelicopterSimulation.FlightControlsPositionController))]
  [RequireComponent(typeof(HelicopterSimulation.FlightPlanController))]
  [RequireComponent(typeof(HelicopterSimulation.MfdRangeController))]
  [RequireComponent(typeof(HelicopterSimulation.TawsImageController))]
  public class HelicopterSetup : MonoBehaviour {

    // The purpose of this script is to automatically add the required scripts to the GameObject


    // Classes used in other scripts

    [System.Serializable]
    public class dragInfo
    {
      [Tooltip("For drag equation")]
      public float dragCoefficient;
      [Tooltip("in meters square")]
      public float sectionArea;

      public dragInfo(float coef, float area)
      {
        dragCoefficient = coef;
        sectionArea = area;
      }
    }

    public enum axes
    {
      X,
      Y,
      Z
    }

    [System.Serializable]
    public class fuelTank
    {
      [Tooltip("The actual quantity of fuel in the tank (in pounds)")]
      public float fuelQuantity;
      [Tooltip("The maximum capacity of the fuel tank (in pounds)")]
      public float fuelCapacity;
      [Tooltip("Position of the fuel tank center of mass (used to compute the actual center of mass of the helicopter)")]
      public Transform fuelTankPosition;
    }

    [System.Serializable]
    public class GroundEffectParameters
    {
      [Tooltip("The \"a\" parameter in the ground effect equation \"f(h) = a * exp(b * h)\" where \"h\" is the rotor height above ground in rotor diameters and \"f(h)\" is the increased rotor thrust in percentage")]
      public float a;

      [SerializeField]
      [Tooltip("The \"b\" parameter in the ground effect equation \"f(h) = a * exp(b * h)\" where \"h\" is the rotor height above ground in rotor diameters and \"f(h)\" is the increased rotor thrust in percentage")]
      public float b;

      public GroundEffectParameters(float iA, float iB)
      {
        a = iA;
        b = iB;
      }
    }

    [System.Serializable]
    public class VortexRingStatePerameters
    {
      [Tooltip("Vertical Speed (in feet/min) at which the H/C will enter Vortex Ring State if its horizontal speed is below \"Horizontal Speed\"")]
      public float verticalSpeed;

      [SerializeField]
      [Tooltip("Horizontal Speed (in Knots) at which the H/C will enter Vortex Ring State if its vertical speed is below \"Vertical Speed\"")]
      public float horizontalSpeed;

      [SerializeField]
      [Tooltip("Efficiency at 2 * Vertical Speed")]
      public float E2;

      [SerializeField]
      [Tooltip("Efficiency at 4 * Vertical Speed")]
      public float E4;

      public VortexRingStatePerameters(float iVspeed, float iHSpeed, float iE2, float iE4)
      {
        verticalSpeed = iVspeed;
        horizontalSpeed = iHSpeed;
        E2 = iE2;
        E4 = iE4;
      }
    }
  }
}