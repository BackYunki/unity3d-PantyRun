using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.HelicopterSimulation
{
  [RequireComponent(typeof(TawsImageController))]
  public class MfdRangeController : MonoBehaviour
  {

    [SerializeField]
    [Tooltip("In Nautical Miles. All available ranges from smallest to biggest")]
    float[] availableRanges = new float[] { 0.1f, 0.5f, 1.0f, 2.5f, 5.0f, 10.0f };

    [SerializeField]
    [Tooltip("In Nautical Miles")]
    float defaultRange = 1.0f;

    int selectedRange; // index of selected range in the availableRanges array

    [SerializeField]
    [Tooltip("Key used to increase displayed range")]
    KeyCode increaseRangeKey = KeyCode.KeypadPlus;

    [SerializeField]
    [Tooltip("Key used to decrease displayed range")]
    KeyCode decreaseRangeKey = KeyCode.KeypadMinus;

    TawsImageController taws;

    // Use this for initialization
    void Start()
    {
      taws = GetComponent<TawsImageController>();

      // select range closest to defaultRange
      float diff = 9999 / 9f;
      for (int i = 0; i < availableRanges.Length; ++i)
      {
        if (Mathf.Abs(availableRanges[i] - defaultRange) < diff)
        {
          diff = Mathf.Abs(availableRanges[i] - defaultRange);
          selectedRange = i;
        }
      }

      // update range on taws
      taws.changeRange(availableRanges[selectedRange]);
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown(increaseRangeKey)
        && selectedRange < availableRanges.Length - 1)
      {
        selectedRange++;
        taws.changeRange(availableRanges[selectedRange]);
      }
      else if (Input.GetKeyDown(decreaseRangeKey)
        && selectedRange > 0)
      {
        selectedRange--;
        taws.changeRange(availableRanges[selectedRange]);
      }
    }

    public float getRange()
    {
      return availableRanges[selectedRange];
    }
  }
}