using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.Utilities
{

  public class GameObjectSwitcher : MonoBehaviour
  {

    // This script is used to switch from one GameObject to another at runtime.
    // It could be used to switch between 2 or more cameras for example.

    [SerializeField]
    [Tooltip("Useful reminder for why the script is used (this property is unused at runtime)")]
    string comment;

    [Space(20)]

    [SerializeField]
    GameObject[] objects;

    [SerializeField]
    int selectionAtLoad = 0;
    int selection = 0;

    [SerializeField]
    KeyCode switchKey = KeyCode.C;

    // Use this for initialization
    void Start()
    {
      if (selectionAtLoad < objects.Length)
      {
        selection = selectionAtLoad;
        selectObject(selection);
      }
      else if (objects.Length > 0)
      {
        Debug.LogError("selection at load is out of bounds");

        //selection = 0;
        //selectObject(selection);
      }
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown(switchKey)
        && 1 < objects.Length)
      {
        selection++;
        if (selection >= objects.Length)
        {
          selection = 0;
        }

        selectObject(selection);
      }
      else if (Input.GetKeyDown(switchKey)
        && 1 == objects.Length)
      {
        objects[0].SetActive(!objects[0].activeInHierarchy);
      }
    }

    void selectObject(int sel)
    {
      for (int i = 0; i < objects.Length; ++i)
      {
        objects[i].SetActive(i == selection);
      }
    }
  }
}