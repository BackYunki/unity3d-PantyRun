using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OFBehavior : NetworkBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer)
		{
			gameObject.GetComponentInChildren<MoveBehaviour>().enabled = true;
			gameObject.GetComponentInChildren<BasicBehaviour>().enabled = true;
			gameObject.GetComponentInChildren<LanternBehavior>().enabled = true;
            gameObject.GetComponentInChildren<AudioListener>().enabled = true;
            GameObject.FindWithTag("NetworkManager").GetComponentInChildren<AudioSource>().enabled = true;
        }
		else
		{
			return;
		}
	}
}
