using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworksObjectControl : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        NetworkServer.RegisterHandler(1001, DoorOpen);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    [Server]
    private void dd(NetworkMessage msg)
    {
        Debug.Log("Network");
    }

    [Server]
    void DoorOpen(NetworkMessage msg)
    {
        OpenBehavior ob = GameObject.Find(msg.ReadMessage<StringMessage>().value).GetComponent<OpenBehavior>();

        ob.DoorOpen();
    }
}
