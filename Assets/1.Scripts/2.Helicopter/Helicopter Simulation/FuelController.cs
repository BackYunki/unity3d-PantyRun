using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.HelicopterSimulation
{
  [RequireComponent(typeof(EngineController))]
  public class FuelController : MonoBehaviour
  {

    // This script is used for fuel consumption, weight and center of mass computation. If no fuel tank is
    // defined, infinite fuel will be used.


    [SerializeField]
    [Tooltip("Define all your fuel tanks here")]
    public HelicopterSetup.fuelTank[] fuelTanks;

    [SerializeField]
    [Tooltip("The burn rate of fuel when engine are running (in pounds per hour)")]
    public float burnRate = 429.0f;

    EngineController engineController;

    // Use this for initialization
    void Start()
    {
      // Find required component
      engineController = GetComponent<EngineController>();
    }

    void FixedUpdate()
    {
      if (engineController.isEngineRunning())
      {
        // burn fuel from the first tank that is not empty
        for (int i = 0; i < fuelTanks.Length; ++i)
        {
          if (0 < fuelTanks[i].fuelQuantity)
          {
            fuelTanks[i].fuelQuantity = Mathf.Clamp(fuelTanks[i].fuelQuantity - burnRate / 3600.0f * Time.fixedDeltaTime, 0, fuelTanks[i].fuelQuantity);
            break;
          }
        }
      }
    }

    // Returns the total fuel remaining (in pounds)
    public float totalFuelRemaining()
    {
      if (fuelTanks.Length > 0)
      {
        // Compute actual fuel remaining
        float totalFuel = 0f;

        foreach (HelicopterSetup.fuelTank tank in fuelTanks)
        {
          totalFuel += tank.fuelQuantity;
        }

        return totalFuel;
      }
      else
      {
        // if no fuel tank is defined, fake that there is always fuel remaining
        return 1000.0f;
      }
    }

    // Returns the total fuel capacity (in pounds)
    public float totalFuelCapacity()
    {
      if (fuelTanks.Length > 0)
      {
        // Comput acutal capacity
        float totalCapacity = 0f;

        foreach (HelicopterSetup.fuelTank tank in fuelTanks)
        {
          totalCapacity += tank.fuelCapacity;
        }

        return totalCapacity;
      }
      else
      {
        // if no fuel tank is defined, fake that there capacity is 2000
        // (so it is always filled at half capacity)
        return 2000.0f;
      }
    }
  }
}