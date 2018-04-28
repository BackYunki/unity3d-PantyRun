using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LanternBehavior : NetworkBehaviour {

    private bool isActive = false;
	
    void Start()
    {

    }

	// Update is called once per frame
	void Update () {
        if (isLocalPlayer && Input.GetKeyDown("e"))
        {
            isActive = !isActive;
            GetComponentInChildren<Light>().enabled = isActive;
        }
    }
}
