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
		}
		else
		{
			return;
		}
	}
}
