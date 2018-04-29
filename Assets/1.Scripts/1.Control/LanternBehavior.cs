using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LanternBehavior : NetworkBehaviour {

    private bool isActive = false;
    public AudioClip toggleSfx;

    void Start()
    {
        if(isLocalPlayer)
        {
            this.gameObject.GetComponentInChildren<Camera>().enabled = true;
        }
        else
        {
            this.gameObject.GetComponentInChildren<Camera>().enabled = false;
        }
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
