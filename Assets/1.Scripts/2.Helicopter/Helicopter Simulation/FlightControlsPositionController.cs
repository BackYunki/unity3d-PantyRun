using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.HelicopterSimulation
{
  public class FlightControlsPositionController : MonoBehaviour
  {

    // This script moves the flight controls in the cockpit according to the input axes values.

    [Header("Objects to rotate")]
    [SerializeField]
    [Tooltip("An array of Transforms for the left pedals rotation. The Transforms will be rotated around the local X axis.Pedals moving forward is a positive rotation.")]
    Transform[] leftPedals;

    [SerializeField]
    [Tooltip("An array of Transforms for the right pedals rotation. The Transforms will be rotated around the local X axis.Pedals moving forward is a positive rotation.")]
    Transform[] rightPedals;

    [SerializeField]
    [Tooltip("Am array of Transforms for the cyclic sticks rotation. Moving the sticks forward will be shown as a positive rotation around the local X axis.Moving the sticks to the left will be shown as a positive rotation around the local Z axis.")]
    Transform[] cyclicSticks;

    [SerializeField]
    [Tooltip("An array of Transforms for the collective levers rotation. The Transforms will be rotated around the local X axis.Levers moving upward is a positive rotation.")]
    Transform[] collectiveLevers;


    [Header("Settings")]

    [SerializeField]
    [Tooltip("Maximum rotation of Cyclic in X in both directions (in degrees)")]
    float maximumCyclicXRotation = 5.0f;
    [SerializeField]
    [Tooltip("Maximum rotation of Cyclic in Y in both directions (in degrees)")]
    float maximumCyclicYRotation = 5.0f;
    [SerializeField]
    [Tooltip("Maximum rotation of Collective lever in both directions (in degrees)")]
    float maximumCollectiveRotation = 7.0f;
    [SerializeField]
    [Tooltip("Maximum rotation of Pedals in both directions (in degrees)")]
    float maximumPedalsRotation = 30.0f;

    [Header("Gamepad Axes")]
    // All public so they can be changed by script at runtime.
    [SerializeField]
    [Tooltip("The axis from the Input Manager that is used for the collective lever position")]
    public string collectiveAxis = "Collective";
    [SerializeField]
    [Tooltip("The axis from the Input Manager that is used for the cyclic stick X position")]
    public string cyclicXAxis = "Cyclic X";
    [SerializeField]
    [Tooltip("The axis from the Input Manager that is used for the cyclic stick Y position")]
    public string cyclicYAxis = "Cyclic Y";
    [SerializeField]
    [Tooltip("The axis from the Input Manager that is used for the pedals position")]
    public string pedalsAxis = "Pedals";

    // Flight controls positions
    [Range(-1.0f, 1.0f)]
    float pedalsPosition;
    [Range(-1.0f, 1.0f)]
    float cyclicXPosition;
    [Range(-1.0f, 1.0f)]
    float cyclicYPosition;
    [Range(-1.0f, 1.0f)]
    float collectivePosition;

    // Tranforms rotation
    float pedalsRotation;
    float cyclicXRotation;
    float cyclicYRotation;
    float collectiveRotation;

    const float cControlsDamping = 5;

    // Update is called once per frame
    void Update()
    {
      // Get positions from the input axes or AP
      pedalsPosition = Mathf.Lerp(pedalsPosition, Input.GetAxis(pedalsAxis), cControlsDamping * Time.deltaTime);
      collectivePosition = Mathf.Lerp(collectivePosition, Input.GetAxis(collectiveAxis), cControlsDamping * Time.deltaTime);
      cyclicXPosition = Mathf.Lerp(cyclicXPosition, Input.GetAxis(cyclicXAxis), cControlsDamping * Time.deltaTime);
      cyclicYPosition = Mathf.Lerp(cyclicYPosition, Input.GetAxis(cyclicYAxis), cControlsDamping * Time.deltaTime);

      // Compute rotation
      pedalsRotation = Mathf.Clamp(Mathf.Clamp(pedalsPosition, -1f, 1f) * maximumPedalsRotation, -maximumPedalsRotation, maximumPedalsRotation);
      cyclicXRotation = Mathf.Clamp(Mathf.Clamp(cyclicXPosition, -1f, 1f) * maximumCyclicXRotation, -maximumCyclicXRotation, maximumCyclicXRotation);
      cyclicYRotation = Mathf.Clamp(Mathf.Clamp(cyclicYPosition, -1f, 1f) * maximumCyclicYRotation, -maximumCyclicYRotation, maximumCyclicYRotation);
      collectiveRotation = Mathf.Clamp(Mathf.Clamp(collectivePosition, -1f, 1f) * maximumCollectiveRotation, -maximumCollectiveRotation, maximumCollectiveRotation);

      // Apply rotations
      foreach (Transform leftPedal in leftPedals)
      {
        leftPedal.localEulerAngles = new Vector3(-pedalsRotation, leftPedal.localEulerAngles.y, leftPedal.localEulerAngles.z);
      }

      foreach (Transform rightPedal in rightPedals)
      {
        rightPedal.localEulerAngles = new Vector3(pedalsRotation, rightPedal.localEulerAngles.y, rightPedal.localEulerAngles.z);
      }

      foreach (Transform cyclicStick in cyclicSticks)
      {
        cyclicStick.localEulerAngles = new Vector3(cyclicYRotation, cyclicStick.localEulerAngles.y, -cyclicXRotation);
      }

      foreach (Transform collectiveLever in collectiveLevers)
      {
        collectiveLever.localEulerAngles = new Vector3(collectiveRotation, collectiveLever.localEulerAngles.y, collectiveLever.localEulerAngles.z);
      }
    }

    // returns the collective lever position (Range [-1, 1])
    public float collectivePos()
    {
      return collectivePosition;
    }

    // returns the pedals position (Range [-1, 1])
    public float pedalsPos()
    {
      return pedalsPosition;
    }

    // returns the cyclic X position (Range [-1, 1])
    public float cyclicXPos()
    {
      return cyclicXPosition;
    }

    // returns the cyclic Y position (Range [-1, 1])
    public float cyclicYPos()
    {
      return cyclicYPosition;
    }
  }
}