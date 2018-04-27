using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.HelicopterSimulation
{
  [RequireComponent(typeof(EngineController))]
  public class RotorRotator : MonoBehaviour
  {

    // This script will rotate the main and tail rotors according to engine speed. A filter is also
    // applied so transition between different speeds is smooth.

    [SerializeField]
    [Tooltip("The main rotor transform that will be rotated")]
    Transform mainRotor;

    [SerializeField]
    [Tooltip("The reference axis around which the main rotor transform will be rotated")]
    HelicopterSetup.axes mainRotorRotationAxis = HelicopterSetup.axes.Y;

    [SerializeField]
    [Tooltip("The speed of the main rotor at full speed (in degrees per second)")]
    float hundredPercentMainRotorSpeed = -1000.0f;

    [SerializeField]
    [Tooltip("The tail rotor transform that will be rotated")]
    Transform tailRotor;

    [SerializeField]
    [Tooltip("The reference axis around which the tail rotor transform will be rotated")]
    HelicopterSetup.axes tailRotorRotationAxis = HelicopterSetup.axes.X;

    [SerializeField]
    [Tooltip("The speed of the tail rotor at full speed (in degrees per second)")]
    float hundredPercentTailRotorSpeed = -500.0f;

    [SerializeField]
    [Tooltip("Filtering factor for the increasing rotor speed")]
    float rotorSpeedIncreadingDamping = 1.0f;

    [SerializeField]
    [Tooltip("Filtering factor for the decreasing rotor speed")]
    float rotorSpeedDecreasingDamping = 0.1f;

    float calculatedRotorSpeed;
    float actualRotorSpeed;

    EngineController engineController;

    void Start()
    {
      // Find required component
      engineController = GetComponent<EngineController>();
    }

    void Update()
    {
      // Raw rotor speed is engine speed (in percent)
      calculatedRotorSpeed = engineController.getEngineSpeed();

      bool increasingSpeed = calculatedRotorSpeed > actualRotorSpeed;

      // Apply filter
      if (increasingSpeed)
      {
        actualRotorSpeed = Mathf.Lerp(actualRotorSpeed, calculatedRotorSpeed, Time.deltaTime * rotorSpeedIncreadingDamping);
      }
      else
      {
        actualRotorSpeed = Mathf.Lerp(actualRotorSpeed, calculatedRotorSpeed, Time.deltaTime * rotorSpeedDecreasingDamping);
      }

      // Rotate transforms
      if (null != mainRotor)
      {
        mainRotor.Rotate(HelicopterSetup.axes.X == mainRotorRotationAxis ? actualRotorSpeed / 100f * hundredPercentMainRotorSpeed * Time.deltaTime : 0f,
                         HelicopterSetup.axes.Y == mainRotorRotationAxis ? actualRotorSpeed / 100f * hundredPercentMainRotorSpeed * Time.deltaTime : 0f,
                         HelicopterSetup.axes.Z == mainRotorRotationAxis ? actualRotorSpeed / 100f * hundredPercentMainRotorSpeed * Time.deltaTime : 0f,
                         Space.Self);
      }

      if (null != tailRotor)
      {
        tailRotor.Rotate(HelicopterSetup.axes.X == tailRotorRotationAxis ? actualRotorSpeed / 100f * hundredPercentTailRotorSpeed * Time.deltaTime : 0f,
                         HelicopterSetup.axes.Y == tailRotorRotationAxis ? actualRotorSpeed / 100f * hundredPercentTailRotorSpeed * Time.deltaTime : 0f,
                         HelicopterSetup.axes.Z == tailRotorRotationAxis ? actualRotorSpeed / 100f * hundredPercentTailRotorSpeed * Time.deltaTime : 0f,
                         Space.Self);
      }
    }

    // Returns the rotor speed (in percent)
    public float getRotorSpeed()
    {
      return actualRotorSpeed;
    }
  }
}