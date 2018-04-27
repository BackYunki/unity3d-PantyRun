using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.HelicopterSimulation
{
  public class FlightPlanController : MonoBehaviour
  {

    [SerializeField]
    [Tooltip("In Nautical Miles. When active waypoint (magenta) gets closer than this distance, the next waypoint becomes the active waypoint.")]
    float distanceToReachWayoint = 0.1f;

    int activeWaypoint = 0;              // active waypoint index
    List<Transform> waypointTransforms;  // list of waypoints composing the flight plan. The order of waypoints in the scene will be the order in the flight plan.

    void Start()
    {
      waypointTransforms = new List<Transform>();
      updateFlightPlan();
    }

    void FixedUpdate()
    {
      // Update active waypoint if minimum distance is reached.
      // Distance is checked only in the X-Z plan.
      if (activeWaypoint < waypointTransforms.Count)
      {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 activeWptPos = new Vector2(waypointTransforms[activeWaypoint].position.x, waypointTransforms[activeWaypoint].position.z);

        float distanceToActiveWaypoint = (currentPos - activeWptPos).magnitude / 1852.0f;
        if (distanceToActiveWaypoint < distanceToReachWayoint)
        {
          activeWaypoint++;
        }
      }
    }

    public int getActiveWaypoint()
    {
      return activeWaypoint;
    }

    // This will find the first object of type "FlightPlan" in the scene and will parse the Transforms children and use them as waypoints.
    public void updateFlightPlan()
    {
      ObjectIdentifiers.FlightPlan wFlightPlan = FindObjectOfType<ObjectIdentifiers.FlightPlan>();

      if (null != wFlightPlan)
      {
        Transform[] list = wFlightPlan.GetComponentsInChildren<Transform>();
        waypointTransforms.Clear();

        for (int i = 1; i < list.Length; i++)
        {
          waypointTransforms.Add(list[i]);
        }
      }
    }
  }
}