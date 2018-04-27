using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.HelicopterSimulation
{
  [RequireComponent(typeof(FuelController))]
  public class EngineController : MonoBehaviour
  {

    // This script is responsible of the engine simulation. Some public functions are also availble and
    // described below.

    [SerializeField]
    [Tooltip("Factor used to filter the engine speed.")]
    float engineSpeedDamping = 1.0f;

    [SerializeField]
    [Tooltip("Set to TRUE if you want to avoid needing to start the engine in game.")]
    bool engineRunningAtLoad = true;

    [SerializeField]
    [Tooltip("The time in seconds it takes for the engine to reach its nominal speed from stop.")]
    float timeForFullEngineSpeed = 30.0f;

    bool engineRunning = true;
    float calculatedEngineSpeed = 0.0f;
    float actualEngineSpeed = 0.0f;
    bool engineIsStarting = false; // True while engine is reaching its nominal speed

    FuelController fuelController;

    // Use this for initialization
    void Start()
    {
      // Find required component
      fuelController = GetComponent<FuelController>();

      // Initialize engine speed
      engineRunning = engineRunningAtLoad;
      calculatedEngineSpeed = engineRunningAtLoad ? 100.0f : 0.0f;

      engineIsStarting = false;

    }

    void FixedUpdate()
    {
      // Check if there is fuel remaining and stop engine if there is no more fuel to burn
      if (0 >= fuelController.totalFuelRemaining())
      {
        engineRunning = false;
        calculatedEngineSpeed = 0.0f;
      }

      // Compute engine speed
      if (engineIsStarting)
      {
        calculatedEngineSpeed += 100 / timeForFullEngineSpeed * Time.fixedDeltaTime;

        if (100 <= calculatedEngineSpeed)
        {
          calculatedEngineSpeed = 100;
          engineIsStarting = false;
        }
      }
      else if (engineRunning)
      {
        calculatedEngineSpeed = 100;
      }
      else
      {
        calculatedEngineSpeed = 0;
      }

      // Apply filter on engine speed
      actualEngineSpeed = Mathf.Lerp(actualEngineSpeed, calculatedEngineSpeed, engineSpeedDamping * Time.fixedDeltaTime);
    }

    // Returns true if the engine is running.
    public bool isEngineRunning()
    {
      return engineRunning;
    }

    // Starts the engine (taking time to reach full speed)
    public void startEngine()
    {
      if (!engineRunning)
      {
        engineIsStarting = true;
        engineRunning = true;
        calculatedEngineSpeed = 0.0f;
      }
    }

    // Stops the engine (taking time to reach a full stop)
    public void stopEngine()
    {
      engineRunning = false;
      engineIsStarting = false;
    }

    // Returns the speed of the engine in percent.
    public float getEngineSpeed()
    {
      return actualEngineSpeed;
    }

    // Allows to change instantly if engine is running or not.
    public void setEngineRunning(bool iRunning)
    {
      engineRunning = iRunning;
    }
  }
}