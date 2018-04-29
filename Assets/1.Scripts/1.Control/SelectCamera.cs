using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SelectCamera : NetworkBehaviour
{

	// Use this for initialization
	void Start()
	{
		if (isLocalPlayer)
		{
			this.gameObject.GetComponentInChildren<Camera>().enabled = true;
		}
	}

}
